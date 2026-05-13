using UnityEngine;

public class PulseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pulsePrefab; // Un prefab avec un visuel de pic de tension
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float pulseSpeed = 5f;

    public void SpawnPulse()
    {
        GameObject pulse = Instantiate(pulsePrefab, spawnPoint.position, Quaternion.identity, transform);
        // On lui donne la même vitesse que les notes pour qu'ils soient synchronisés
        NoteMovement move = pulse.AddComponent<NoteMovement>();
        move.speed = pulseSpeed;
    }
}