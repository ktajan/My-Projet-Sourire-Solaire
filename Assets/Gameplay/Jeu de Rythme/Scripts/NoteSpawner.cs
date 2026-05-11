using UnityEngine;

public class SmartNoteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] buttonPrefabs;
    [SerializeField] private GameObject vibrationPrefab;

    [Header("Réglages de Position")]
    [Range(0f, 1f)]
    [SerializeField] private float verticalPosition = 0.5f; // 0.5 = milieu de l'écran
    [SerializeField] private float spawnOffsetOutside = 2f; // Distance au delà du bord droit

    [Header("Vitesse")]
    [SerializeField] private float moveSpeed = 5f;

    public void SpawnNoteAndVibration(int index)
    {
        // 1. Calculer la position x au bord droit de l'écran
        float xPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + spawnOffsetOutside;
        // 2. Calculer la position y selon le réglage de l'inspector
        float yPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, verticalPosition, 0)).y;

        Vector3 spawnPos = new Vector3(xPosition, yPosition, 0);

        // Apparition du bouton
        if (index < buttonPrefabs.Length)
        {
            GameObject note = Instantiate(buttonPrefabs[index], spawnPos, Quaternion.identity, transform);
            note.GetComponent<NoteMovement>().speed = moveSpeed;
        }

        // Apparition de la vibration (même position)
        GameObject vibe = Instantiate(vibrationPrefab, spawnPos, Quaternion.identity, transform);
        vibe.GetComponent<NoteMovement>().speed = moveSpeed;
    }
}