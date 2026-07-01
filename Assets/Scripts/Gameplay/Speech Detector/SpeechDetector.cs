using System.Collections.Generic;
using UnityEngine;

public enum PhraseType { MOE, KYUN }

/// <summary>
/// SpeechDetectorクラスは、音声検出プロセスを開始し、検出された音声のタイミングをチェックし、タイミングの精度に基づいてスコアを計算する
/// </summary>
[RequireComponent(typeof(SpeechToText))]
public class SpeechDetector : MonoBehaviour
{
	[Tooltip("音声がこの時間内（秒）に検出されるとGood判定になる。(min, max)形式で設定し、min < maxであること")]
	[SerializeField] private Vector2 _detectionWindow = new(1f, 2f);

	[Space(10)]
	[Header("Key Phrases")]
	[SerializeField] private List<string> _moePhrases = new();
	[SerializeField] private List<string> _kyunPhrases = new();
	[SerializeField] private List<string> _extraPhrases = new();

	private List<string> _keyPhrases = new();   // ! メモリ節約のため、Startメソッド内のローカル変数にできる
	private float _startTime;
	private SpeechToText _speechToText;
	private enum DetectionStage { Early = 80, Good = 100, Late = 60, Miss = 50 }

	public event System.Action<int, string> OnRecordingCompleted = delegate { };
	public static event System.Action<Dictionary<PhraseType, int>> OnFoundPhraseOccurrence = delegate { };

	void OnDestroy()
	{
		_speechToText.OnKeyPhraseDetected -= OnKeyPhraseDetected;
		_speechToText.OnKeyPhraseUnDetected -= OnKeyPhraseUnDetected;
	}

	private void Awake()
	{
		// すべてのフレーズを1つのリストにまとめる
		_keyPhrases.AddRange(_moePhrases);
		_keyPhrases.AddRange(_kyunPhrases);
		_keyPhrases.AddRange(_extraPhrases);
	}

	private void Start()
	{
		_speechToText = FindAnyObjectByType<SpeechToText>();
		if (_speechToText == null)
		{
			Debug.LogError("One or more required components not found in the scene. Please add them.");
			enabled = false;
			return;
		}

		// 検出対象のキーフレーズを設定する
		_speechToText.KeyPhrases = _keyPhrases;

		// キーフレーズが検出されたかどうかを受け取るために登録する
		_speechToText.OnKeyPhraseDetected += OnKeyPhraseDetected;
		_speechToText.OnKeyPhraseUnDetected += OnKeyPhraseUnDetected;
	}

	public void StartDetection()
	{
		// 音声認識を開始する
		_speechToText.StartRecording(_detectionWindow.y);

		// 開始時刻を記録する
		_startTime = Time.time;
	}

	private void OnKeyPhraseDetected(string resultPhrase, int volume)
	{
		float elapsedTime = Time.time - _startTime;

		// 経過時間と音量に応じてスコアを計算する
		DetectionStage stage;

		//  score as Early
		if (elapsedTime < _detectionWindow.x) stage = DetectionStage.Early;
		// score as Good
		else if (elapsedTime <= _detectionWindow.y) stage = DetectionStage.Good;
		// score as Late
		else stage = DetectionStage.Late;

		// スコアを送信する
		OnRecordingCompleted?.Invoke(CalculateScore((int)stage, volume), resultPhrase);

		// UIを更新する
		OnFoundPhraseOccurrence?.Invoke(OrganizeOccurrence(resultPhrase));
	}

	private void OnKeyPhraseUnDetected(string message, int volume)
	{
		// score as Miss
		OnRecordingCompleted?.Invoke(CalculateScore((int)DetectionStage.Miss, volume), message + " Score: Miss");
	}

	private int CalculateScore(int stageScore, int volumeScore) => Mathf.RoundToInt((stageScore + volumeScore) / 2f);

	private Dictionary<PhraseType, int> OrganizeOccurrence(string keyPhrase)
	{
		Dictionary<PhraseType, int> occurredPhrase = new()
		{
			{ PhraseType.MOE, 0 },
			{ PhraseType.KYUN, 0 }
		};

		occurredPhrase[PhraseType.MOE] = CountPhraseOccurrence(keyPhrase, _moePhrases);
		occurredPhrase[PhraseType.KYUN] = CountPhraseOccurrence(keyPhrase, _kyunPhrases);

		return occurredPhrase;
	}

	private int CountPhraseOccurrence(string phrase, List<string> list)
	{
		int occurringTimes = 0;

		phrase = phrase.ToLower();

		foreach (string word in list)
		{
			if (!phrase.Contains(word)) continue;

			occurringTimes++;

			// すでに検出済みの単語を削除する
			phrase = phrase.Remove(phrase.IndexOf(word), word.Length);
			// phrase = phrase[(phrase.IndexOf(word[^1]) + 1)..];
		}

		return occurringTimes;
	}
}