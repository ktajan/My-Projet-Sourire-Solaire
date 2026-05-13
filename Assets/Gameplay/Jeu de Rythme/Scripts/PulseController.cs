using UnityEngine;
using System.Collections.Generic;

public class PulseController : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Synchronisation Physique")]
    [Tooltip("Doit correspondre EXACTEMENT à la 'speed' du bouton")]
    [SerializeField] private float vitesseBoutons = 5f;
    [Tooltip("Délai (en secondes) avant l'apparition de l'onde")]
    [SerializeField] private float delaiSpawn = 0.1f;

    [Header("Paramètres du Zigzag Aléatoire")]
    [SerializeField] private float vibrationPower = 1.5f;
    [SerializeField] private float echoFrequency = 20f;
    [Tooltip("Vitesse de baisse d'énergie globale")]
    [SerializeField] private float echoDecay = 5f;
    [SerializeField] private float pulseDuration = 1.5f;

    private Vector3[] _positions;
    private List<float> _activePulses = new List<float>();

    public void TriggerVibration()
    {
        _activePulses.Add(-delaiSpawn);
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

        for (int i = _activePulses.Count - 1; i >= 0; i--)
        {
            _activePulses[i] += Time.deltaTime;
            if (_activePulses[i] > dureeDeVieMax)
            {
                _activePulses.RemoveAt(i);
            }
        }

        for (int i = 0; i < pointCount; i++)
        {
            float distanceDepuisOrigine = origineX - _positions[i].x;
            float totalY = 0f;

            foreach (float pulseAge in _activePulses)
            {
                if (pulseAge <= 0f) continue;

                float tempsLocal = pulseAge - (distanceDepuisOrigine / vitesseBoutons);

                if (tempsLocal >= 0f && tempsLocal <= pulseDuration)
                {
                    // --- 1. LE ZIGZAG (Aucune courbe, angles vifs) ---
                    float cycleTime = tempsLocal * echoFrequency;
                    int cycleIndex = Mathf.FloorToInt(cycleTime);
                    float timeInCycle = cycleTime - cycleIndex; // De 0 à 1

                    float wave = 0f;
                    if (timeInCycle < 0.25f)
                        wave = timeInCycle * 4f; // Monte en flèche (0 à 1)
                    else if (timeInCycle < 0.75f)
                        wave = 1f - ((timeInCycle - 0.25f) * 4f); // Descend brutalement (1 à -1)
                    else
                        wave = -1f + ((timeInCycle - 0.75f) * 4f); // Remonte à zéro (-1 à 0)

                    // --- 2. HAUTEUR ALÉATOIRE SÉCURISÉE ---
                    // On récupère l'instant de naissance de l'onde pour créer un "ID" unique
                    float pulseId = Mathf.Round((Time.time - pulseAge) * 100f);

                    // On crée une graine mathématique (Seed) unique pour CE pic précis
                    float seed = pulseId + cycleIndex;

                    // Formule magique qui génère un chiffre aléatoire fixe (entre 0 et 1)
                    float randomHash = Mathf.Abs(Mathf.Sin(seed * 12.9898f) * 43758.5453f);
                    float randomFactor = randomHash - Mathf.Floor(randomHash);

                    // On calcule la baisse de base, puis on l'écrase avec notre aléatoire
                    float dampingGlobal = Mathf.Exp(-tempsLocal * echoDecay);

                    // Le pic fera entre 10% et 100% de la taille qu'il devrait faire
                    float amplitudeAleatoire = dampingGlobal * Mathf.Lerp(0.1f, 1.0f, randomFactor);

                    // On additionne le résultat à la ligne
                    totalY += wave * vibrationPower * amplitudeAleatoire;
                }
            }

            _positions[i].y = totalY;
        }

        lineRenderer.SetPositions(_positions);
    }
}