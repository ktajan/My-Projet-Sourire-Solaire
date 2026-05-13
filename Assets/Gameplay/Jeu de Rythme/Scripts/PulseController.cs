using UnityEngine;
using System.Collections.Generic;

public class PulseController : MonoBehaviour
{
    // Petite classe interne pour stocker les infos de chaque onde active
    private class PulseData
    {
        public float age;
        public float multiplier;

        public PulseData(float startAge, float mult)
        {
            age = startAge;
            multiplier = mult;
        }
    }

    [Header("Composants")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Synchronisation")]
    [SerializeField] private float vitesseBoutons = 5f;
    [SerializeField] private float delaiSpawn = 0.1f;

    [Header("Multiplicateurs par Bouton")]
    [SerializeField] private float puissanceA = 1.0f;
    [SerializeField] private float puissanceB = 1.0f;
    [SerializeField] private float puissanceX = 1.0f;
    [SerializeField] private float puissanceY = 1.0f;

    [Header("Paramètres de l'Onde (ECG)")]
    [SerializeField] private float vibrationPower = 2.0f;
    [Range(0f, 1f)]
    [SerializeField] private float echoMaxPercentage = 0.5f;
    [SerializeField] private float echoMinRandom = 0.2f;
    [SerializeField] private float echoMaxRandom = 1.0f;

    [Space]
    [SerializeField] private float echoFrequency = 20f;
    [SerializeField] private float echoDecay = 3f;
    [SerializeField] private float pulseDuration = 1.5f;

    private Vector3[] _positions;
    private List<PulseData> _activePulses = new List<PulseData>();

    // --- MODIFICATION : On accepte maintenant le type de bouton ---
    public void TriggerVibration(TypeBouton type)
    {
        float mult = 1f;

        // On choisit le multiplicateur de l'inspector selon le bouton
        switch (type)
        {
            case TypeBouton.BoutonA: mult = puissanceA; break;
            case TypeBouton.BoutonB: mult = puissanceB; break;
            case TypeBouton.BoutonX: mult = puissanceX; break;
            case TypeBouton.BoutonY: mult = puissanceY; break;
        }

        // On ajoute une nouvelle onde avec son propre multiplicateur
        _activePulses.Add(new PulseData(-delaiSpawn, mult));
    }

    void Update()
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2) return;

        int pointCount = lineRenderer.positionCount;
        if (_positions == null || _positions.Length != pointCount)
        {
            _positions = new Vector3[pointCount];
            lineRenderer.GetPositions(_positions);
        }

        float origineX = _positions[pointCount - 1].x;
        float distanceTotale = Mathf.Abs(_positions[0].x - origineX);
        float dureeDeVieMax = (distanceTotale / vitesseBoutons) + pulseDuration;

        // Mise à jour des ages
        for (int i = _activePulses.Count - 1; i >= 0; i--)
        {
            _activePulses[i].age += Time.deltaTime;
            if (_activePulses[i].age > dureeDeVieMax) _activePulses.RemoveAt(i);
        }

        for (int i = 0; i < pointCount; i++)
        {
            float distanceDepuisOrigine = origineX - _positions[i].x;
            float totalY = 0f;

            foreach (PulseData pulse in _activePulses)
            {
                if (pulse.age <= 0f) continue;

                float tempsLocal = pulse.age - (distanceDepuisOrigine / vitesseBoutons);

                if (tempsLocal >= 0f && tempsLocal <= pulseDuration)
                {
                    // 1. ZIGZAG
                    float cycleTime = tempsLocal * echoFrequency;
                    int cycleIndex = Mathf.FloorToInt(cycleTime);
                    float timeInCycle = cycleTime - cycleIndex;

                    float wave = 0f;
                    if (timeInCycle < 0.25f) wave = timeInCycle * 4f;
                    else if (timeInCycle < 0.75f) wave = 1f - ((timeInCycle - 0.25f) * 4f);
                    else wave = -1f + ((timeInCycle - 0.75f) * 4f);

                    // 2. AMPLITUDE (Multipliée par le réglage du bouton)
                    float currentVibrationPower = vibrationPower * pulse.multiplier;
                    float finalAmplitude = currentVibrationPower;

                    if (cycleIndex > 0)
                    {
                        float pulseId = Mathf.Round((Time.time - pulse.age) * 100f);
                        float seed = pulseId + cycleIndex;
                        float randomHash = (Mathf.Sin(seed * 12.9898f) * 43758.5453f) % 1;
                        float randomFactor = Mathf.Abs(randomHash);

                        float baseEchoPower = currentVibrationPower * echoMaxPercentage;
                        float variation = Mathf.Lerp(echoMinRandom, echoMaxRandom, randomFactor);
                        float damping = Mathf.Exp(-tempsLocal * echoDecay);

                        finalAmplitude = baseEchoPower * variation * damping;
                    }

                    totalY += wave * finalAmplitude;
                }
            }
            _positions[i].y = totalY;
        }
        lineRenderer.SetPositions(_positions);
    }
}