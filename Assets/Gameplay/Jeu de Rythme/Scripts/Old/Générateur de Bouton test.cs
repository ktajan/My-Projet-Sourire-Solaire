using UnityEngine;

public class GenerateurTest : MonoBehaviour
{
    public Camera cam;
    public GameObject[] prefabsXbox;
    public CardiogrammeRenderer scriptECG; // Liaison vers l'autre script

    public float intervalle = 2.0f;
    public float vitesse = 5f;
    [Range(0, 1)] public float hauteurJeu = 0.5f;

    private float timer;

    void Start()
    {
        timer = 2.0f; // On attend 2s pour laisser Naninovel s'installer
        if (cam == null) cam = Camera.main;
        timer = intervalle;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            CreerNote();
            timer = intervalle;
        }
    }

    void CreerNote()
    {
        // Calcul du spawn
        Vector3 posWorld = cam.ViewportToWorldPoint(new Vector3(1.1f, hauteurJeu, 10f));
        posWorld.z = 0;

        // 1. On spawn la note physiquement
        int index = Random.Range(0, prefabsXbox.Length);
        GameObject note = Instantiate(prefabsXbox[index], posWorld, Quaternion.identity);

        // On lui donne le script de mouvement simple (ou on le gère ici)
        note.AddComponent<DeplacementSimple>().vitesse = vitesse;

        // 2. ON ENVOIE LE SIGNAL À L'ECG
        if (scriptECG != null)
        {
            scriptECG.AjouterVibration(posWorld.x);
        }
    }
}

// Petit script d'appoint pour que la note bouge toute seule
public class DeplacementSimple : MonoBehaviour
{
    public float vitesse;
    void Update()
    {
        transform.Translate(Vector3.left * vitesse * Time.deltaTime);
        if (Camera.main.WorldToViewportPoint(transform.position).x < -0.2f) Destroy(gameObject);
    }
}