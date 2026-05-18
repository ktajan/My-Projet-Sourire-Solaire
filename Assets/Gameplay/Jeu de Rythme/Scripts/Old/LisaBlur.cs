using UnityEngine;
using UnityEngine.UI;
using Naninovel;
using Naninovel.UI;

public class LisaBlur : MonoBehaviour
{
    [Header("Réglages")]
    [Range(0f, 1f)] public float intensiteFlou = 0f;
    [SerializeField] private Material blurMaterial; // Glisse ton Mat_UI_Blur ici

    private Image _blurOverlay;
    private GameObject _dialogueZone;

    void Update()
    {
        if (!Engine.Initialized) return;

        if (_dialogueZone == null)
        {
            TrouverZoneEtCreerFlou();
        }
        else
        {
            GererIntensite();
        }
    }

    private void TrouverZoneEtCreerFlou()
    {
        // On cherche le panel de Naninovel
        var panel = Object.FindAnyObjectByType<RevealableTextPrinterPanel>();
        if (panel != null)
        {
            _dialogueZone = panel.gameObject;

            // On crée un objet Image pour le flou
            GameObject blurObj = new GameObject("Lisa_Blur_Overlay");
            blurObj.transform.SetParent(_dialogueZone.transform, false);

            // On le place devant le texte dans la hiérarchie
            blurObj.transform.SetAsLastSibling();

            _blurOverlay = blurObj.AddComponent<Image>();
            _blurOverlay.material = blurMaterial;

            // On force l'image à prendre TOUTE la place de la zone de dialogue
            RectTransform rect = blurObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            Debug.Log("<color=green>LisaBlur : Zone de flou créée sur " + _dialogueZone.name + "</color>");
        }
    }

    private void GererIntensite()
    {
        if (_blurOverlay == null) return;

        // On active le panneau seulement si on a besoin de flou
        bool doitEtreActif = intensiteFlou > 0.01f;
        if (_blurOverlay.enabled != doitEtreActif) _blurOverlay.enabled = doitEtreActif;

        if (doitEtreActif)
        {
            // On ajuste la variable du shader (souvent nomm/_Size ou _BlurAmount)
            // Vérifie le nom de la variable dans ton shader !
            _blurOverlay.material.SetFloat("_Size", intensiteFlou * 10f);

            // On peut aussi jouer sur l'opacité pour renforcer l'effet
            Color c = _blurOverlay.color;
            c.a = intensiteFlou;
            _blurOverlay.color = c;
        }
    }
}