using UnityEngine;

public class PulseController : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Paramètres de la Ligne")]
    [SerializeField] private int resolution = 100;
    [SerializeField] private float vibrationPower = 1.5f;
    [SerializeField] private float scrollSpeed = 0.5f; // Vitesse de défilement
    [SerializeField] private float zPosition = 0f;    // Ta demande pour le Z

    [Header("Paramètres de l'Écho (ECG)")]
    [SerializeField] private float echoFrequency = 20f; // Vitesse de l'alternance +/-
    [SerializeField] private float echoDecay = 5f;      // Vitesse d'extinction (plus c'est haut, plus c'est court)
    [SerializeField] private float pulseDuration = 1f;  // Durée totale de la vibration

    private float[] _yPositions;
    private Vector3[] _vertexPositions;
    private float _width;
    private float _pulseTimer = 100f; // Initialisé haut pour ne pas pulser au départ

    private void OnEnable() => SmartNoteSpawner.OnNoteSpawned += HandleNoteSpawn;
    private void OnDisable() => SmartNoteSpawner.OnNoteSpawned -= HandleNoteSpawn;

    private void HandleNoteSpawn(float xPos)
    {
        TriggerVibration();
    }

    void Start()
    {
        _yPositions = new float[resolution];
        _vertexPositions = new Vector3[resolution];
        lineRenderer.positionCount = resolution;

        // Calcul de la largeur de l'écran pour la ligne
        if (Camera.main != null)
            _width = Camera.main.orthographicSize * 2 * Camera.main.aspect * 1.1f;
    }

    // Cette fonction lance la vibration "intelligente"
    public void TriggerVibration()
    {
        _pulseTimer = 0f; // On remet le timer à zéro pour commencer l'onde
    }

    void Update()
    {
        // 1. Décalage des positions (Défilement de droite à gauche)
        // On déplace les valeurs de l'index i+1 vers i
        for (int i = 0; i < resolution - 1; i++)
        {
            _yPositions[i] = _yPositions[i + 1];
        }

        // 2. Calcul de la nouvelle valeur à l'entrée (tout à droite)
        if (_pulseTimer < pulseDuration)
        {
            // Formule mathématique d'onde amortie :
            // Amplitude * Cosinus(Fréquence) * Exponentielle Négative(Amortissement)
            float wave = Mathf.Cos(_pulseTimer * echoFrequency) * vibrationPower;
            float damping = Mathf.Exp(-_pulseTimer * echoDecay);

            _yPositions[resolution - 1] = wave * damping;
            _pulseTimer += Time.deltaTime * scrollSpeed * 10f;
        }
        else
        {
            // Retour au calme
            _yPositions[resolution - 1] = Mathf.Lerp(_yPositions[resolution - 1], 0, Time.deltaTime * 10f);
        }

        // 3. Mise à jour physique des points du LineRenderer
        for (int i = 0; i < resolution; i++)
        {
            // On centre la ligne sur l'objet et on applique le Z de l'inspector
            float xPos = (i - resolution / 2f) * (_width / resolution);
            _vertexPositions[i] = transform.position + new Vector3(xPos, _yPositions[i], zPosition);
        }

        lineRenderer.SetPositions(_vertexPositions);
    }
}