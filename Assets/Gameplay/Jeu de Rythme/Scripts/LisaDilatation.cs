using UnityEngine;
using TMPro;
using Naninovel;

public class LisaShaderDilatation : MonoBehaviour
{
    [Header("Lien Gameplay")]
    [Tooltip("Glisse ici l'objet qui contient le ScriptCoeurEtTrigger")]
    public ScriptCoeurEtTrigger scriptCoeur;

    [Header("Réglages du Shader (Lisa)")]
    public float dilateMax = 0f; // Épaisseur normale (quand tout va bien)
    [Range(-1f, 0f)] public float dilateMin = -0.5f; // Épaisseur quand le score est à 0

    [Header("Animation")]
    [Tooltip("Vitesse à laquelle le texte s'affine ou regonfle")]
    public float vitesseTransition = 3f;

    private TMP_Text _tmpComponent;
    private Material _materialInstance;

    // Variables internes pour le lissage et la logique
    private float _lastAppliedDilatation = -999f;
    private float _currentDilatation = 0f;
    private float _scoreEffectif = 100f; // C'est notre "verrou"

    void Start()
    {
        _currentDilatation = dilateMax;
    }

    void LateUpdate()
    {
        if (!Engine.Initialized) return;

        if (_tmpComponent == null)
        {
            ChercherComposantNaninovel();
        }
        else
        {
            CalculerEtAppliquerDilatation();
        }
    }

    private void ChercherComposantNaninovel()
    {
        var panel = Object.FindAnyObjectByType<Naninovel.UI.RevealableTextPrinterPanel>();
        if (panel != null)
        {
            _tmpComponent = panel.GetComponentInChildren<TMP_Text>();

            // LA SÉCURITÉ : On s'assure que le composant ET le material source sont bien réels
            if (_tmpComponent != null && _tmpComponent.fontSharedMaterial != null)
            {
                _materialInstance = new Material(_tmpComponent.fontSharedMaterial);
                _tmpComponent.fontSharedMaterial = _materialInstance;
            }
            else
            {
                // Si le texte est là mais que son material n'est pas encore prêt (ou détruit),
                // on annule la découverte pour forcer le LateUpdate à retenter le coup plus tard.
                _tmpComponent = null;
            }
        }
    }

    private void CalculerEtAppliquerDilatation()
    {
        if (scriptCoeur == null) return;

        // --- 1. LOGIQUE DU VERROU ---
        // On lit le score via la propriété publique qu'on vient de créer
        float scoreReel = scriptCoeur.ScoreActuel;

        if (scoreReel >= 100f)
        {
            _scoreEffectif = 100f; // On casse le verrou, Lisa va mieux
        }
        else if (scoreReel < _scoreEffectif)
        {
            _scoreEffectif = scoreReel; // Le score baisse, le verrou s'enfonce
        }

        // --- 2. CALCUL DE LA CIBLE (Mapping) ---
        float ratio = Mathf.Clamp01(_scoreEffectif / 100f);
        float targetDilatation = Mathf.Lerp(dilateMin, dilateMax, ratio);

        // --- 3. LISSAGE FLUIDE ---
        _currentDilatation = Mathf.Lerp(_currentDilatation, targetDilatation, Time.deltaTime * vitesseTransition);

        // --- 4. APPLICATION AU SHADER ---
        if (_materialInstance != null && Mathf.Abs(_currentDilatation - _lastAppliedDilatation) > 0.001f)
        {
            _materialInstance.SetFloat(ShaderUtilities.ID_FaceDilate, _currentDilatation);

            _tmpComponent.UpdateMeshPadding();
            _tmpComponent.SetAllDirty();
            _tmpComponent.ForceMeshUpdate();

            _lastAppliedDilatation = _currentDilatation;
        }
    }

    void OnDestroy()
    {
        if (_materialInstance != null) Destroy(_materialInstance);
    }
}