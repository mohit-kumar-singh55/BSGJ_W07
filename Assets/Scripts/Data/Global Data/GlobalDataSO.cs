using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalDataSO", menuName = "GlobalDataSO", order = 0)]
public class GlobalDataSO : ScriptableObject
{
    public FeverModeSettings feverModeSettings;

    [Space(10)]
    public CustomerSettings customerSettings;
}

[Serializable]
public struct FeverModeSettings
{
    [Header("Conditions To Activate Fever Mode")]
    [Tooltip("Number of consecutive perfect 萌えキュン to activate fever mode")]
    public int perfectCountToActivateFever;
    [Tooltip("Minimum score to count as perfect 萌えキュン out of 200 (Voice: 100, Hand: 100, not including the Paint)")]
    public int minScoreToCountAsPerfect;

    [Header("Fever Settings")]
    public float feverDuration;
    public float feverScoreMultiplier;
}

[Serializable]
public struct CustomerSettings
{
    [Tooltip("Customer's Waiting Time")]
    public float waitingTime;
    [Tooltip("Score deduction value when customer leaves due to waiting times up")]
    public int scoreToDeductOnTimesUp;

    [Space(10)]
    public CustomerMoodSettings customerMoodSettings;
}

[Serializable]
public struct CustomerMoodSettings
{
    [Tooltip("Minimum score required out of 200 (Voice: 100, Hand: 100, not including the Paint) when in bad mood to multiply the bad mood score multiplier to the score")]
    public float minScoreRequiredInBadMood;
    [Tooltip("Score to be multiplied when customer is in bad mood")]
    public float badMoodScoreMultiplier;
}