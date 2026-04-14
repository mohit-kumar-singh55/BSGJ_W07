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

    [Space(10)]
    [Header("Always show on screen settings")]
    [SerializeField] private float _zOffsetFromCamera = 0.1f; // z offset from the camera to avoid being culled by the camera
    [SerializeField] private Vector3 _scaleWhenNotInView = new(0.5f, 0.5f, 0.5f); // scale of the mood sprite when not in view (to make it smaller and less obtrusive)

    private CustomerMood _currentMood = CustomerMood.None;
    private SpriteRenderer _spriteRenderer;
    private Transform _originalTransform; // Store the original transform of the mood sprite
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
        _originalTransform = transform;
        _mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_currentMood == CustomerMood.None) return;

        // check if this object is in the camera view
        Vector3 viewportPos = _mainCam.WorldToViewportPoint(transform.position);
        bool isInView = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;

        // TODO: instead of doing all this, just create multiple mood ui images as per the customers & loop through them and for those which are not in the camera view, show them in the ui image
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