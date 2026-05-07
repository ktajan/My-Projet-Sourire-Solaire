using UnityEngine;

public class DeplacementNote : MonoBehaviour
{
    [Header("Réglages")]
    [Tooltip("Vitesse à laquelle la note fonce vers la gauche")]
    public float vitesse = 5f;

    [Tooltip("Multiplicateur de taille (1 = normale, 2 = double, 0.5 = moitié)")]
    public float taille = 1f;

    // La fonction Start est lue UNE SEULE FOIS, au moment exact où la note apparaît à l'écran.
    // C'est l'endroit parfait pour faire les réglages initiaux comme la taille.
    void Start()
    {
        // On modifie l'échelle (Scale) locale de l'objet. 
        // On applique ta variable "taille" sur les axes X (largeur) et Y (hauteur). 
        // On laisse Z à 1 car on est en 2D.
        transform.localScale = new Vector3(taille, taille, 1f);
    }

    // La fonction Update est lue en boucle, à chaque image (frame) du jeu.
    // C'est ici qu'on gère l'action continue, donc le mouvement.
    void Update()
    {
        transform.Translate(Vector3.left * vitesse * Time.deltaTime);
    }
}