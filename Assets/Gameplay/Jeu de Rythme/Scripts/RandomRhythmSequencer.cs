using UnityEngine;

public class RandomRhythmSequencer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int numberOfButtonTypes = 4;

    [Header("References")]
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private PulseSpawner pulseSpawner;

    private float _timer;
    private float _nextSpawnTime;

    void Start() => SetNextTime();

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _nextSpawnTime)
        {
            int randomIndex = Random.Range(0, numberOfButtonTypes);

            // On envoie l'ordre aux spawners
            noteSpawner.SpawnNote(randomIndex);
            pulseSpawner.SpawnPulse();

            _timer = 0;
            SetNextTime();
        }
    }

    void SetNextTime() => _nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
}