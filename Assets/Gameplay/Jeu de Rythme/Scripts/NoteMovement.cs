using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    [Header("Settings")]
    public float speed;
    public string targetKey; // Ex: "a", "z", "e", "r" (en minuscules pour faciliter la comparaison)

    private RhythmScoreManager _scoreManager;
    private bool _wasHit = false;

    void Start()
    {
        // Version moderne d'Unity (remplace FindObjectOfType)
        _scoreManager = Object.FindAnyObjectByType<RhythmScoreManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Si la note sort de l'écran (ajustez -12 selon la taille de votre vue)
        if (transform.position.x < -12f && !_wasHit)
        {
            if (_scoreManager != null) _scoreManager.LosePointsMiss();
            Destroy(gameObject);
        }
    }

    public void HitSuccess()
    {
        _wasHit = true;
        // Ici on pourra ajouter l'effet visuel "Perfect" plus tard
        Destroy(gameObject);
    }
}