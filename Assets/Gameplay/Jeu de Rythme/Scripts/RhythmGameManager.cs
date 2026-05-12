using Naninovel;
using UnityEngine;

public class RhythmGameManager : MonoBehaviour
{
    [Header("Réglages du Cœur")]
    [SerializeField] private GameObject heartObject;
    [SerializeField] private Vector3 heartPosition = Vector3.zero;
    [SerializeField] private Vector3 heartScale = Vector3.one;

    [Header("Réglages de la Ligne")]
    [SerializeField] private GameObject cardiogramLine;
    [SerializeField] private Vector3 linePosition = Vector3.zero;
    [Tooltip("Laissez à 0 pour que la ligne fasse automatiquement la largeur de l'écran")]
    [SerializeField] private float lineLength = 0f;

    void Start()
    {
        SetupHeart();
        SetupLine();
    }

    private void SetupHeart()
    {
        if (heartObject != null)
        {
            
            heartObject.transform.localPosition = heartPosition;
            heartObject.transform.localScale = heartScale;
            heartObject.SetActive(true);

            // Debug pour vérifier dans la console si le code s'exécute bien
            Debug.Log($"Cœur initialisé à l'échelle : {heartObject.transform.localScale}");
        }
    }

    private void SetupLine()
    {
        if (cardiogramLine != null)
        {
            cardiogramLine.transform.localPosition = linePosition;
            cardiogramLine.transform.localScale = Vector3.one;

            float finalWidth = lineLength;
            if (finalWidth <= 0)
            {
                // On cherche l'objet "MainCamera" directement dans la hiérarchie de la scène
                GameObject naniCamObj = GameObject.Find("MainCamera");

                if (naniCamObj != null)
                {
                    // On récupère son composant Camera
                    Camera naniCamera = naniCamObj.GetComponent<Camera>();

                    // Calcul de la largeur avec la caméra trouvée
                    finalWidth = naniCamera.orthographicSize * 2 * naniCamera.aspect;
                }
                else
                {
                    Debug.LogError("Impossible de trouver un objet nommé 'MainCamera' dans la scène !");
                    return; // On stoppe tout pour éviter une division par zéro ou une erreur plus loin
                }
            }

            LineRenderer lr = cardiogramLine.GetComponent<LineRenderer>();
            if (lr != null)
            {
                // On s'assure que les points suivent l'objet et non le centre du monde
                lr.useWorldSpace = false;
                int pointCount = lr.positionCount;

                if (pointCount > 1 && finalWidth > 0)
                {
                    // Calcul de la répartition des points
                    float startX = -finalWidth / 2f;
                    float xStep = finalWidth / (pointCount - 1);

                    // Placement des points de gauche à droite
                    for (int i = 0; i < pointCount; i++)
                    {
                        lr.SetPosition(i, new Vector3(startX + (i * xStep), 0, 0));
                    }
                    Debug.Log($"Succès : Ligne de {finalWidth} unités créée avec {pointCount} points.");
                }
            }
            cardiogramLine.SetActive(true);
        }
    }

    // Utilisé pour arrêter proprement le jeu depuis Naninovel
    public void StopGame()
    {
        Destroy(gameObject);
    }
}