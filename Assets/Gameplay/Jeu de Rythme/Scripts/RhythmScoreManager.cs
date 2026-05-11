using UnityEngine;
using UnityEngine.Events;

public class RhythmScoreManager : MonoBehaviour
{
    [Header("Score actuel")]
    public float currentScore = 10f;

    [Header("Pénalités")]
    [SerializeField] private float penaltyMiss = 0.5f;
    [SerializeField] private float penaltyWrongKey = 1.0f;

    [Header("Événements")]
    public UnityEvent OnScoreChanged;

    public void LosePointsMiss()
    {
        currentScore -= penaltyMiss;
        UpdateUI();
    }

    public void LosePointsWrongKey()
    {
        currentScore -= penaltyWrongKey;
        UpdateUI();
    }

    public void AddPointsSuccess()
    {
        currentScore += 1.0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentScore < 0) currentScore = 0;

        // Notification pour l'inspecteur ou l'UI
        if (OnScoreChanged != null) OnScoreChanged.Invoke();

        Debug.Log("Score actuel : " + currentScore);
    }
}