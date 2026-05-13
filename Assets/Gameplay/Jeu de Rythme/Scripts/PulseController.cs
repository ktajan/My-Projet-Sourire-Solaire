using UnityEngine;

public class PulseController : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Synchronisation Physique")]
    [Tooltip("Même valeur que la 'speed' du bouton (NoteMovement)")]
    [SerializeField] private float vitesseBoutons = 5f;
    [Tooltip("Ajustement manuel du timing (en secondes)")]
    [SerializeField] private float delaiSpawn = 0.1f;

    [Header("Paramètres de l'Onde")]
    [SerializeField] private float vibrationPower = 1.5f;
    [SerializeField] private float echoFrequency = 20f;
    [SerializeField] private float echoDecay = 5f;
    [SerializeField] private float pulseDuration = 1f;

    private Vector3[] _positions;
    private float[] _yPositions;
    private float _distanceAccumulateur = 0f;
    private float _pulseTimer = 100f;

    private float _delayTimer = 0f;
    private bool _isDelayed = false;

    // --- Gestion du Spawn (Trigger) ---
    public void TriggerVibration()
    {
        if (delaiSpawn > 0f)
        {
            _isDelayed = true;
            _delayTimer = delaiSpawn;
        }
        else
        {
            _pulseTimer = 0f;
        }
    }

    void Update()
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2) return;

        int pointCount = lineRenderer.positionCount;

        // Initialisation des tableaux si nécessaire
        if (_positions == null || _positions.Length != pointCount)
        {
            _positions = new Vector3[pointCount];
            _yPositions = new float[pointCount];
            lineRenderer.GetPositions(_positions);
            // On mémorise les Y de départ
            for (int i = 0; i < pointCount; i++) _yPositions[i] = _positions[i].y;
        }

        // 1. Calcul de la distance entre deux points (en unités Unity)
        // On le recalcule au cas où la ligne change, mais c'est l'écart physique X.
        float distanceEntrePoints = Mathf.Abs(_positions[1].x - _positions[0].x);

        // 2. Gestion du délai
        if (_isDelayed)
        {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f) { _isDelayed = false; _pulseTimer = 0f; }
        }

        // 3. L'ACCUMULATEUR DE DISTANCE (Le coeur de la Solution 1)
        // On regarde de combien les boutons ont avancé pendant cette frame
        _distanceAccumulateur += vitesseBoutons * Time.deltaTime;

        // Tant que l'accumulateur dépasse la distance entre deux points de la ligne
        while (_distanceAccumulateur >= distanceEntrePoints)
        {
            _distanceAccumulateur -= distanceEntrePoints;

            // On décale les valeurs Y du tableau vers la gauche
            for (int i = 0; i < pointCount - 1; i++)
            {
                _yPositions[i] = _yPositions[i + 1];
            }

            // On calcule la nouvelle valeur de l'onde à injecter à droite
            float newY = 0f;
            if (_pulseTimer < pulseDuration)
            {
                float wave = Mathf.Cos(_pulseTimer * echoFrequency) * vibrationPower;
                float damping = Mathf.Exp(-_pulseTimer * echoDecay);
                newY = wave * damping;

                // On avance le timer de l'onde proportionnellement à la vitesse
                _pulseTimer += (distanceEntrePoints / vitesseBoutons) * 10f;
            }
            _yPositions[pointCount - 1] = newY;
        }

        // 4. Mise à jour visuelle (Interpolation pour la fluidité)
        float t = _distanceAccumulateur / distanceEntrePoints;
        for (int i = 0; i < pointCount; i++)
        {
            float lerpedY = _yPositions[i];
            // On lisse entre la position actuelle et la suivante pour éviter les saccades
            if (i < pointCount - 1)
            {
                lerpedY = Mathf.Lerp(_yPositions[i], _yPositions[i + 1], t);
            }

            _positions[i].y = lerpedY;
        }

        lineRenderer.SetPositions(_positions);
    }
}