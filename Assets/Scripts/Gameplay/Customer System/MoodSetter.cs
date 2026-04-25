using System;
using UnityEngine;

[Serializable]
public struct MoodSpritePair
{
    [SerializeField] private Sprite _happyMoodSprite;
    [SerializeField] private Sprite _sadMoodSprite;
    [SerializeField] private Sprite _angryMoodSprite;

    public readonly Sprite HappyMoodSprite => _happyMoodSprite;
    public readonly Sprite SadMoodSprite => _sadMoodSprite;
    public readonly Sprite AngryMoodSprite => _angryMoodSprite;
}

public enum CustomerMood
{
    None,
    Happy,
    Sad,
    Angry
}

[RequireComponent(typeof(SpriteRenderer))]
public class MoodSetter : MonoBehaviour
{
    [SerializeField] private MoodSpritePair _moodSpritePair;

    private CustomerMood _currentMood = CustomerMood.None;
    private SpriteRenderer _spriteRenderer;
    private Camera _mainCam;

    public CustomerMood CurrentMood => _currentMood;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // reset (remove mood sprite) when start
        SetMood(CustomerMood.None);
        _mainCam = Camera.main;
    }

    public void SetMood(CustomerMood mood = CustomerMood.None)
    {
        switch (mood)
        {
            case CustomerMood.Happy:
                _currentMood = CustomerMood.Happy;
                _spriteRenderer.sprite = _moodSpritePair.HappyMoodSprite;
                break;
            case CustomerMood.Sad:
                _currentMood = CustomerMood.Sad;
                _spriteRenderer.sprite = _moodSpritePair.SadMoodSprite;
                break;
            case CustomerMood.Angry:
                _currentMood = CustomerMood.Angry;
                _spriteRenderer.sprite = _moodSpritePair.AngryMoodSprite;
                break;
            default:
                _currentMood = CustomerMood.None;
                _spriteRenderer.sprite = null;
                break;
        }
    }
}