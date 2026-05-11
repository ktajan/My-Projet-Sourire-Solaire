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
            int randomIndex = Random.Range(0, 4);

            // On appelle les deux scripts séparément
            if (buttonSpawner != null)
                buttonSpawner.SpawnButtonOnly(randomIndex);

            if (lineController != null)
                lineController.TriggerVibration();

            _timer = 0;
            SetNextTime();
        }
    }

    private void SetNextTime()
    {
        _nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}