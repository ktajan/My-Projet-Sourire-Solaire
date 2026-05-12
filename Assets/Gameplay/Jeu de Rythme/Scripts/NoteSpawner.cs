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

    public float spawnZPosition = 0f; // Modifiable depuis l'Inspector

    public static System.Action<float> OnNoteSpawned;

    private Camera cam;

    // J'ai renommé la fonction pour plus de clarté, elle ne fait que le bouton

    void Start()
    {
        // On cherche l'objet "MainCamera" directement dans la hiérarchie de la scène
        GameObject naniCamObj = GameObject.Find("MainCamera");

        if (naniCamObj != null)
        {
            // On récupère son composant Camera
            Camera naniCamera = naniCamObj.GetComponent<Camera>();

            // Calcul de la largeur avec la caméra trouvée
            float finalWidth = naniCamera.orthographicSize * 2 * naniCamera.aspect;

            cam = naniCamObj.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("Impossible de trouver un objet nommé 'MainCamera' dans la scène !");
            return; // On stoppe tout pour éviter une division par zéro ou une erreur plus loin
        }

    }
    public void SpawnButtonOnly(int index)
    {
        // Calcul de la position à droite avec ta méthode
        float worldWidth = cam.orthographicSize * 2 * cam.aspect;
        float xPosition = (worldWidth / 2) + spawnOffsetOutside;

        float screenHeight = cam.orthographicSize * 2;
        float yPosition = (verticalPosition * screenHeight) - (screenHeight / 2);

        Vector3 spawnPos = new Vector3(xPosition, yPosition, spawnZPosition);

        if (index < buttonPrefabs.Length)
        {
            GameObject note = Instantiate(buttonPrefabs[index], spawnPos, Quaternion.identity, transform);
            note.GetComponent<NoteMovement>().speed = moveSpeed;
        }
    }
}