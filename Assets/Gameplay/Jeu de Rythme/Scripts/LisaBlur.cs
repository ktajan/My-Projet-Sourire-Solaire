using UnityEngine;
using TMPro;
using Naninovel;
using Naninovel.UI;

public class LisaBlur : MonoBehaviour
{
    [Header("Réglages")]
    [Range(0f, 1f)] public float intensiteFlou = 0f;

    private TextMeshProUGUI _tmpComponent;
    private Material _materialInstance;

    private int _faceSoftnessID;
    private int _outlineSoftnessID;

    void Awake()
    {
        _faceSoftnessID = Shader.PropertyToID("_FaceSoftness");
        _outlineSoftnessID = Shader.PropertyToID("_OutlineSoftness");
    }

    void Update()
    {
        if (!Engine.Initialized) return;

        if (_tmpComponent == null)
        {
            TryGetNaninovelText();
        }
        else
        {
            ApplyBlurEffect();
        }
    }

    private void TryGetNaninovelText()
    {
        var uiManager = Engine.GetService<IUIManager>();
        if (uiManager == null) return;

        // On récupère l'UI générique
        IManagedUI printer = uiManager.GetUI("TMProDialogue");

        if (printer != null)
        {
            // ASTUCE : On "cast" l'interface en MonoBehaviour pour 
            // accéder enfin au GameObject et aux composants
            if (printer is MonoBehaviour printerComponent)
            {
                _tmpComponent = printerComponent.GetComponentInChildren<TextMeshProUGUI>();

                if (_tmpComponent != null)
                {
                    // On crée une instance unique du matériel
                    _materialInstance = _tmpComponent.fontMaterial;
                }
            }
        }
    }

    private void ApplyBlurEffect()
    {
        if (_materialInstance != null)
        {
            _materialInstance.SetFloat(_outlineSoftnessID, intensiteFlou);
            _materialInstance.SetFloat(_faceSoftnessID, intensiteFlou);
        }
    }

    public void ResetReference()
    {
        _tmpComponent = null;
        _materialInstance = null;
    }
}