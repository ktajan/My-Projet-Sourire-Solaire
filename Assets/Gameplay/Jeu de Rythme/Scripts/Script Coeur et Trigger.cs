using UnityEngine;
using UnityEngine.InputSystem;

public class ScriptCoeurEtTrigger : MonoBehaviour
{
    [Header("Réglages des Touches")]
    public InputActionReference actionA;
    public InputActionReference actionB;
    public InputActionReference actionX;
    public InputActionReference actionY;

    [Header("Paramètres de Jeu")]
    public float margeParfaite = 0.5f;

    [Header("Système de Score")]
    [SerializeField] private int pointsNormal = 10;
    [SerializeField] private int pointsParfait = 25;
    [SerializeField] private int penaliteRate = 15;          // Note qui sort sans être frappée
    [SerializeField] private int penaliteMauvaiseTouche = 10; // Mauvaise touche sur une note
    [SerializeField] private int penaliteDansLeVide = 5;      // Appuyer quand il n'y a rien
    [SerializeField] private int scoreMax = 100;
    [SerializeField] private int scoreDepart = 50;

    [Header("Animation Coeur")]
    public Animator animatorPersonnage;

    // --- LE CORRECTIF EST ICI ---
    private int _scoreActuel = 0; // La vraie variable (qui avait disparu)
    public int ScoreActuel => _scoreActuel; // La vitrine pour que Lisa puisse lire le score
    // ----------------------------

    private GameObject noteActuelle = null;
    private InfoNote infoNoteActuelle = null;
    private bool peutFrapper = false;

    void Start()
    {
        _scoreActuel = scoreDepart;
    }

    void OnEnable()
    {
        if (actionA != null) actionA.action.Enable();
        if (actionB != null) actionB.action.Enable();
        if (actionX != null) actionX.action.Enable();
        if (actionY != null) actionY.action.Enable();
    }

    void Update()
    {
        // On vérifie chaque touche individuellement pour pouvoir gérer les erreurs
        if (actionA.action.WasPressedThisFrame()) GererEntree(TypeBouton.BoutonA);
        if (actionB.action.WasPressedThisFrame()) GererEntree(TypeBouton.BoutonB);
        if (actionX.action.WasPressedThisFrame()) GererEntree(TypeBouton.BoutonX);
        if (actionY.action.WasPressedThisFrame()) GererEntree(TypeBouton.BoutonY);
    }

    private void GererEntree(TypeBouton boutonPresse)
    {
        if (peutFrapper && infoNoteActuelle != null)
        {
            // CAS 1 : C'est la bonne touche !
            if (boutonPresse == infoNoteActuelle.typeDeCetteNote)
            {
                EvaluerFrappe();
            }
            // CAS 2 : C'est une touche, mais pas la bonne (Mauvaise Note)
            else
            {
                Debug.Log("Mauvaise Touche !");
                ModifierScore(-penaliteMauvaiseTouche);
                Destroy(noteActuelle);
                NettoyerNote();
            }
        }
        // CAS 3 : On appuie alors qu'il n'y a pas de note (Dans le vide)
        else
        {
            Debug.Log("Appuyé dans le vide !");
            ModifierScore(-penaliteDansLeVide);
        }
    }

    void OnTriggerEnter2D(Collider2D autre)
    {
        if (autre.CompareTag("Notes"))
        {
            noteActuelle = autre.gameObject;
            infoNoteActuelle = autre.GetComponent<InfoNote>();
            peutFrapper = true;
        }
    }

    void OnTriggerExit2D(Collider2D autre)
    {
        if (autre.CompareTag("Notes") && autre.gameObject == noteActuelle)
        {
            // CAS 4 : La note s'en va sans avoir été touchée (Raté)
            Debug.Log("Raté !");
            ModifierScore(-penaliteRate);
            NettoyerNote();
        }
    }

    void EvaluerFrappe()
    {
        float distance = Vector2.Distance(transform.position, noteActuelle.transform.position);

        if (distance <= margeParfaite)
        {
            Debug.Log("PARFAIT !");
            ModifierScore(pointsParfait);

            // NOUVEAU : On déclenche l'animation ici !
            if (animatorPersonnage != null)
            {
                animatorPersonnage.SetTrigger("FaitUnParfait");
            }
        }
        else
        {
            Debug.Log("BIEN !");
            ModifierScore(pointsNormal);
        }

        Destroy(noteActuelle);
        NettoyerNote();
    }

    public void ModifierScore(int valeur)
    {
        _scoreActuel += valeur;
        _scoreActuel = Mathf.Clamp(_scoreActuel, 0, scoreMax);

        Debug.Log("Score actuel : " + _scoreActuel);
    }

    void NettoyerNote()
    {
        noteActuelle = null;
        infoNoteActuelle = null;
        peutFrapper = false;
    }
}