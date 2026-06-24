using Naninovel;
using UnityEngine;
using System;

[Serializable, Alias("attendreScoreMax")]
public class CommandeAttendreScore : Command
{
    [Alias("cible")]
    public IntegerParameter ScoreCible = 100;

    public override async Awaitable Execute(ExecutionContext ctx)
    {
        // On récupère TOUS les scripts actifs dans la scène, pas juste un seul
        var objets = UnityEngine.Object.FindObjectsByType<ScriptCoeurEtTrigger>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        if (objets.Length == 0)
        {
            Debug.LogError("Le script ScriptCoeurEtTrigger n'a pas été trouvé !");
            return;
        }

        // S'il y a plus d'un script, on déclenche une alerte dans la console
        if (objets.Length > 1)
        {
            Debug.LogWarning($"[Alerte Architecte] Attention, il y a {objets.Length} objets avec le ScriptCoeurEtTrigger en même temps dans la scène !");
        }

        // On prend le premier qu'on trouve
        var jeuRythme = objets[0];
        int cible = ScoreCible.HasValue ? ScoreCible.Value : 100;

        Debug.Log($"[Naninovel] Je me connecte à l'objet nommé : {jeuRythme.gameObject.name}. Score lu : {jeuRythme.ScoreActuel}");

        int compteurFrames = 0;

        while (jeuRythme != null && jeuRythme.ScoreActuel < cible)
        {
            compteurFrames++;

            // Toutes les 60 frames (environ 1 seconde), il nous dit ce qu'il voit
            if (compteurFrames % 60 == 0)
            {
                Debug.Log($"[Naninovel Surveille] L'objet '{jeuRythme.gameObject.name}' a actuellement {jeuRythme.ScoreActuel} points.");
            }

            await Awaitable.NextFrameAsync();
        }

        Debug.Log($"[Naninovel] Cible atteinte sur {jeuRythme.gameObject.name} ! Reprise du texte.");
    }
}