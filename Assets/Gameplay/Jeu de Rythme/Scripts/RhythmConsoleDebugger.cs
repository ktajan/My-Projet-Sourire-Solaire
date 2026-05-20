using UnityEngine;
using RhythmTool;

public class RhythmNoteSequencer : MonoBehaviour
{
    [Header("Scripts à piloter")]
    [SerializeField] private SmartNoteSpawner buttonSpawner;
    [SerializeField] private PulseController lineController;

    [Header("Configuration Rhythm Tool")]
    [SerializeField] private RhythmEventProvider eventProvider;

    void Start()
    {
        if (eventProvider != null)
        {
            // On connecte nos fonctions aux pistes "X" et "Y"
            eventProvider.Register<Onset>(OnBoutonX, "X");
            eventProvider.Register<Onset>(OnBoutonY, "Y");
        }
    }

    private void OnBoutonX(Onset onset)
    {
        // La piste X correspond au bouton d'index 0
        TriggerNote(0);
    }

    private void OnBoutonY(Onset onset)
    {
        // La piste Y correspond au bouton d'index 1
        TriggerNote(1);
    }

    // Fonction centralisée pour spawner la note et déclencher la vibration
    private void TriggerNote(int index)
    {
        if (buttonSpawner != null)
            buttonSpawner.SpawnButtonOnly(index);

        if (lineController != null)
            lineController.TriggerVibration((TypeBouton)index);
    }

    void OnDestroy()
    {
        // Désinscription obligatoire pour éviter les erreurs quand on quitte la scène
        if (eventProvider != null)
        {
            eventProvider.Unregister<Onset>(OnBoutonX, "X");
            eventProvider.Unregister<Onset>(OnBoutonY, "Y");
        }
    }
}