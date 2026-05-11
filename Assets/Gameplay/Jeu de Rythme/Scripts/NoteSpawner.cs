using UnityEngine;

public class SmartNoteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] buttonPrefabs;

    [Header("Réglages de Position")]
    [Range(0f, 1f)]
    [SerializeField] private float verticalPosition = 0.5f;
    [SerializeField] private float spawnOffsetOutside = 2f;

    [Header("Vitesse")]
    [SerializeField] private float moveSpeed = 5f;

    // J'ai renommé la fonction pour plus de clarté, elle ne fait que le bouton
    public void SpawnButtonOnly(int index)
    {
        float xPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + spawnOffsetOutside;
        float yPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, verticalPosition, 0)).y;

        Vector3 spawnPos = new Vector3(xPosition, yPosition, 0);

        if (index < buttonPrefabs.Length)
        {
            GameObject note = Instantiate(buttonPrefabs[index], spawnPos, Quaternion.identity, transform);
            note.GetComponent<NoteMovement>().speed = moveSpeed;
        }
    }
}