using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OffScreenMoodDisplayerクラスは、
/// 画面外にいるお客のムードをUIで表示するためのクラスです。
/// </summary>
public class OffScreenMoodDisplayer : MonoBehaviour
{
    [SerializeField] private Transform _moodDisplayUIParent;
    [SerializeField] private GameObject _moodDisplayUIPrefab;
    [SerializeField] private float _moodDisplayUIOffset = 30f;

    private Camera _mainCam;
    private List<MoodSetter> _moodSetters = new();
    private Dictionary<MoodSetter, GameObject> _moodDisplays = new();   // maps mood setters to its display ui objects

    void OnEnable()
    {
        Customer.OnCustomerSpawn += FetchAllMoodSetters;
        CustomerDestroyer.OnCustomerDestroy += RemoveMoodSetter;
    }

    void OnDisable()
    {
        Customer.OnCustomerSpawn -= FetchAllMoodSetters;
        CustomerDestroyer.OnCustomerDestroy -= RemoveMoodSetter;
    }

    private void Start() => _mainCam = Camera.main;

    // カメラとお客の移動後に気分表示UIを更新する
    private void LateUpdate() => UpdateMoodDisplays();

    private void UpdateMoodDisplays()
    {
        foreach (MoodSetter moodSetter in _moodSetters)
        {
            // 気分が設定されていない場合は削除する
            if (moodSetter.CurrentMood == CustomerMood.None)
            {
                DestroyMoodDisplayAndRemoveFromDic(moodSetter);
                continue;
            }

            Vector3 viewportPos = _mainCam.WorldToViewportPoint(moodSetter.transform.position);
            bool isInView = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;

            if (isInView)
            {
                // 生成されていない場合はスキップする
                DestroyMoodDisplayAndRemoveFromDic(moodSetter);
                continue;
            }

            // 気分UI画像を生成し、お客の方向を示すように画面端へ配置する
            GameObject moodDisplay;
            if (!_moodDisplays.ContainsKey(moodSetter))
            {
                // 生成する
                moodDisplay = Instantiate(_moodDisplayUIPrefab, _moodDisplayUIParent);
                _moodDisplays.Add(moodSetter, moodDisplay);
            }
            else moodDisplay = _moodDisplays[moodSetter];

            // 画像を設定する
            if (moodDisplay.TryGetComponent(out Image moodDisplayImage) &&
                moodSetter.TryGetComponent(out SpriteRenderer moodSetterSR))
                moodDisplayImage.sprite = moodSetterSR.sprite;

            // 位置を設定する
            Vector3 screenPos = _mainCam.WorldToScreenPoint(moodSetter.transform.position);
            moodDisplay.transform.position = new(Mathf.Clamp(screenPos.x, _moodDisplayUIOffset, Screen.width - _moodDisplayUIOffset), Mathf.Clamp(screenPos.y, _moodDisplayUIOffset, Screen.height - _moodDisplayUIOffset), 0);
        }
    }

    private void FetchAllMoodSetters(MoodSetter newCustomerMoodSetter)
    {
        _moodSetters.Add(newCustomerMoodSetter);
    }

    private void RemoveMoodSetter(MoodSetter removedCustomerMoodSetter)
    {
        if (_moodSetters.Contains(removedCustomerMoodSetter))
            _moodSetters.Remove(removedCustomerMoodSetter);

        DestroyMoodDisplayAndRemoveFromDic(removedCustomerMoodSetter);
    }

    private void DestroyMoodDisplayAndRemoveFromDic(MoodSetter moodSetterToRemove)
    {
        if (!_moodDisplays.ContainsKey(moodSetterToRemove)) return;

        Destroy(_moodDisplays[moodSetterToRemove]);
        _moodDisplays.Remove(moodSetterToRemove);
    }
}