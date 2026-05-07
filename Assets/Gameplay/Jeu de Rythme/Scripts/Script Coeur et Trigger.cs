using UnityEngine;
using UnityEngine.InputSystem; // <-- 1. Cette ligne permet d'utiliser le nouveau système

public class ScriptCoeurEtTrigger : MonoBehaviour
{
    [Header("Réglages des Touches")]
    // 2. Ces variables DOIVENT être 'public' pour apparaître dans l'Inspector
    public InputActionReference actionA;
    public InputActionReference actionB;
    public InputActionReference actionX;
    public InputActionReference actionY;

    [Header("Paramètres de Jeu")]
    public float margeParfaite = 0.5f;

    private GameObject noteActuelle = null;
    private InfoNote infoNoteActuelle = null;
    private bool peutFrapper = false;

    // 3. On active les entrées quand le script démarre
    void OnEnable()
    {
        if (actionA != null) actionA.action.Enable();
        if (actionB != null) actionB.action.Enable();
        if (actionX != null) actionX.action.Enable();
        if (actionY != null) actionY.action.Enable();
    }

    void Update()
    {
        if (peutFrapper && infoNoteActuelle != null)
        {
            if (VerifierSiTouchePressee())
            {
                EvaluerFrappe();
            }
        }
    }

    bool VerifierSiTouchePressee()
    {
        // On demande à l'action correspondante au type de la note si elle est activée
            // On remplace .triggered par .WasPressedThisFrame()
            // C'est beaucoup plus fiable pour les jeux de rythme
            switch (infoNoteActuelle.typeDeCetteNote)
            {
                case TypeBouton.BoutonA: return actionA.action.WasPressedThisFrame();
                case TypeBouton.BoutonB: return actionB.action.WasPressedThisFrame();
                case TypeBouton.BoutonX: return actionX.action.WasPressedThisFrame();
                case TypeBouton.BoutonY: return actionY.action.WasPressedThisFrame();
                default: return false;
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
            Debug.Log("Raté !");
            NettoyerNote();
        }
    }

    void EvaluerFrappe()
    {
        float distance = Vector2.Distance(transform.position, noteActuelle.transform.position);

        if (distance <= margeParfaite) Debug.Log("PARFAIT !");
        else Debug.Log("BIEN !");

        Destroy(noteActuelle);
        NettoyerNote();
    }

    void NettoyerNote()
    {
        noteActuelle = null;
        infoNoteActuelle = null;
        peutFrapper = false;
    }
}