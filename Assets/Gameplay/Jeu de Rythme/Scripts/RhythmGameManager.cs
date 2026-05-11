using UnityEngine;
using Naninovel;

public class RhythmGameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject heartObject;
    [SerializeField] private GameObject cardiogramLine;

    // Cette méthode peut être appelée via un Custom Command ou simplement au Start
    void Start()
    {
        heartObject.SetActive(true);
        cardiogramLine.SetActive(true);
        Debug.Log("Jeu de rythme initialisé");
    }

    // Fonction pour arrêter le jeu depuis Naninovel
    public void StopGame()
    {
        Destroy(gameObject);
    }
}