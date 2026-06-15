using UnityEngine;
using System.Collections.Generic; // REQUIS POUR UTILISER LES LISTES

// On définit les types de boutons possibles
public enum TypeBouton { BoutonA, BoutonB, BoutonX, BoutonY }

public class InfoNote : MonoBehaviour
{
    // C'est ici que tu choisiras le type dans l'Inspector d'Unity
    public TypeBouton typeDeCetteNote;

    // LE REGISTRE : Une liste accessible de partout qui contient toutes les notes actives
    public static List<InfoNote> notesActives = new List<InfoNote>();

    void Start()
    {
        // Dès que la note spawn, elle s'inscrit dans la liste
        notesActives.Add(this);
    }

    void OnDestroy()
    {
        // Quand la note est détruite (frappée ou ratée), elle se retire de la liste
        notesActives.Remove(this);
    }

    // Cette fonction nous dira quelle touche clavier correspond au bouton
    // Pratique pour tester sans manette !
    public KeyCode ObtenirToucheClavier()
    {
        switch (typeDeCetteNote)
        {
            case TypeBouton.BoutonA: return KeyCode.A;
            case TypeBouton.BoutonB: return KeyCode.B;
            case TypeBouton.BoutonX: return KeyCode.X;
            case TypeBouton.BoutonY: return KeyCode.Y;
            default: return KeyCode.None;
        }
    }
}