using UnityEngine;

// On définit les types de boutons possibles
public enum TypeBouton { BoutonA, BoutonB, BoutonX, BoutonY }

public class InfoNote : MonoBehaviour
{
    // C'est ici que tu choisiras le type dans l'Inspector d'Unity
    public TypeBouton typeDeCetteNote;

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