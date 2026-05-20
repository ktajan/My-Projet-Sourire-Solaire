using UnityEngine;

public class RandomRhythmSequencer : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float minSpawnDelay = 1.5f;
    [SerializeField] private float maxSpawnDelay = 3.0f;

    [Header("Scripts à piloter")]
    [SerializeField] private SmartNoteSpawner buttonSpawner;
    [SerializeField] private PulseController lineController;

    private float _timer;
    private float _nextSpawnTime;

    void Start() => SetNextTime();

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _nextSpawnTime)
        {
            // On choisit un index entre 0 et 3
            int randomIndex = Random.Range(0, 4);

            // On appelle le spawner de boutons 
            if (buttonSpawner != null)
                buttonSpawner.SpawnButtonOnly(randomIndex);

            // On appelle le contrôleur de ligne en transformant l'index en TypeBouton
            if (lineController != null)
            {
                // On ajoute (TypeBouton) devant randomIndex pour corriger l'erreur
                lineController.TriggerVibration((TypeBouton)randomIndex);
            }

            _timer = 0;
            SetNextTime();
        }
    }

    private void SetNextTime()
    {
        _nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}