using UnityEngine;
using TMPro;
using Naninovel;
using System.Collections.Generic;

public class LisaBlur : MonoBehaviour
{
    [Header("Réglages de la Dérive")]
    [Range(0f, 1f)] public float intensiteEparpillement = 0f;
    [SerializeField] private float vitesseDeriveBase = 30f; // Augmenté pour mieux voir
    [SerializeField] private float distanceMax = 500f;
    [Range(0f, 1f)]
    [SerializeField] private float courbureTrajectoire = 0.3f;

    private TMP_Text _tmpComponent;
    private List<CharacterDriftState> _characterStates = new List<CharacterDriftState>();
    private int _lastProcessedCharCount = 0;

    private class CharacterDriftState
    {
        public Vector2 directionInitiale;
        public Vector2 positionActuelle;
        public float noiseSeedAngle;
        public float speedMultiplier;

        public CharacterDriftState()
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2f);
            directionInitiale = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            positionActuelle = Vector2.zero;
            noiseSeedAngle = Random.Range(0f, 1000f);
            speedMultiplier = Random.Range(0.8f, 1.2f);
        }
    }

    // Changement ici : LateUpdate au lieu de Update
    void LateUpdate()
    {
        if (!Engine.Initialized) return;

        if (_tmpComponent == null)
        {
            ChercherComposantNaninovel();
        }
        else if (_tmpComponent.gameObject.activeInHierarchy)
        {
            // On applique même si c'est faible pour voir le mouvement
            AppliquerDeriveApesanteur();
        }
    }

    private void ChercherComposantNaninovel()
    {
        var panel = Object.FindAnyObjectByType<Naninovel.UI.RevealableTextPrinterPanel>();
        if (panel != null)
        {
            _tmpComponent = panel.GetComponentInChildren<TMP_Text>();
            if (_tmpComponent != null)
            {
                Debug.Log("<color=orange>LisaBlur : Texte trouvé sur " + _tmpComponent.gameObject.name + "</color>");
            }
        }
    }

    private void AppliquerDeriveApesanteur()
    {
        // On force TMP à calculer ses données internes d'abord
        _tmpComponent.ForceMeshUpdate();

        var textInfo = _tmpComponent.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        if (characterCount != _lastProcessedCharCount)
        {
            ActualiserEtatsPersonnages(characterCount);
        }

        float deltaTime = Time.deltaTime;
        float tempsLent = Time.time * 0.5f;
        float vitesseActuelleBase = vitesseDeriveBase * intensiteEparpillement;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // On récupère le tableau de sommets pour ce matériel spécifique
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            CharacterDriftState state = _characterStates[i];

            // Calcul de la dérive
            float noiseAngleMod = (Mathf.PerlinNoise(state.noiseSeedAngle, tempsLent) - 0.5f) * courbureTrajectoire * Mathf.PI;
            float angleBase = Mathf.Atan2(state.directionInitiale.y, state.directionInitiale.x);
            Vector2 velocity = new Vector2(Mathf.Cos(angleBase + noiseAngleMod), Mathf.Sin(angleBase + noiseAngleMod));

            state.positionActuelle += velocity * vitesseActuelleBase * state.speedMultiplier * deltaTime;
            state.positionActuelle = Vector2.ClampMagnitude(state.positionActuelle, distanceMax);

            Vector3 offset = new Vector3(state.positionActuelle.x, state.positionActuelle.y, 0);

            // Application directe sur les sommets
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        // IMPORTANT : On dit à TMP de mettre à jour le rendu final
        _tmpComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void ActualiserEtatsPersonnages(int count)
    {
        if (count < _lastProcessedCharCount) _characterStates.Clear();
        while (_characterStates.Count < count) _characterStates.Add(new CharacterDriftState());
        _lastProcessedCharCount = count;
    }

    public void ResetDerives()
    {
        _characterStates.Clear();
        _lastProcessedCharCount = 0;
        if (_tmpComponent != null) _tmpComponent.ForceMeshUpdate();
    }
}