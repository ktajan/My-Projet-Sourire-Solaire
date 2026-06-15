using UnityEngine;
using TMPro;
using Naninovel;
using System.Collections.Generic;

public class LisaCouleurTexte : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Glisse ici l'objet qui sert de cible (le Coeur)")]
    public Transform pointDImpact;

    [Header("Réglages des Couleurs")]
    public Color couleurParDefaut = Color.white;
    public Color couleurBoutonA = Color.green;
    public Color couleurBoutonB = Color.red;
    public Color couleurBoutonX = Color.blue;
    public Color couleurBoutonY = Color.yellow;

    [Header("Paramètres de Détection")]
    [Tooltip("Distance à partir de laquelle le texte commence à changer de couleur")]
    public float distanceMax = 5f;

    private TMP_Text _tmpComponent;

    void LateUpdate()
    {
        if (!Engine.Initialized) return;

        // Connexion au texte NaniNovel (même logique que ton script de dilatation)
        if (_tmpComponent == null)
        {
            ChercherComposantNaninovel();
            return;
        }

        InfoNote noteProche = TrouverNoteLaPlusProche();
        AppliquerCouleur(noteProche);
    }

    private void ChercherComposantNaninovel()
    {
        var panel = Object.FindAnyObjectByType<Naninovel.UI.RevealableTextPrinterPanel>();
        if (panel != null)
        {
            _tmpComponent = panel.GetComponentInChildren<TMP_Text>();
        }
    }

    private InfoNote TrouverNoteLaPlusProche()
    {
        InfoNote laPlusProche = null;
        float distanceMin = float.MaxValue;

        // On fouille dans notre fameux registre
        foreach (InfoNote note in InfoNote.notesActives)
        {
            if (note == null) continue; // Sécurité si une note est détruite entre-temps

            float dist = Vector2.Distance(pointDImpact.position, note.transform.position);

            // Si cette note est plus proche que l'ancienne qu'on a testée, elle devient la cible
            if (dist < distanceMin)
            {
                distanceMin = dist;
                laPlusProche = note;
            }
        }

        return laPlusProche;
    }

    private void AppliquerCouleur(InfoNote note)
    {
        // CAS 1 : Aucune note à l'écran, on revient doucement à la couleur de base
        if (note == null)
        {
            _tmpComponent.color = Color.Lerp(_tmpComponent.color, couleurParDefaut, Time.deltaTime * 5f);
            return;
        }

        // CAS 2 : On a une note. On calcule son ratio de distance (0 = loin, 1 = sur le point d'impact)
        float distance = Vector2.Distance(pointDImpact.position, note.transform.position);
        float ratio = 1f - Mathf.Clamp01(distance / distanceMax);

        Color couleurCible = ObtenirCouleurPourBouton(note.typeDeCetteNote);

        // On mélange la couleur par défaut et la couleur cible en fonction du ratio d'approche
        Color couleurFinale = Color.Lerp(couleurParDefaut, couleurCible, ratio);

        // Application fluide de la couleur au composant texte
        _tmpComponent.color = Color.Lerp(_tmpComponent.color, couleurFinale, Time.deltaTime * 10f);
    }

    private Color ObtenirCouleurPourBouton(TypeBouton type)
    {
        switch (type)
        {
            case TypeBouton.BoutonA: return couleurBoutonA;
            case TypeBouton.BoutonB: return couleurBoutonB;
            case TypeBouton.BoutonX: return couleurBoutonX;
            case TypeBouton.BoutonY: return couleurBoutonY;
            default: return couleurParDefaut;
        }
    }
}