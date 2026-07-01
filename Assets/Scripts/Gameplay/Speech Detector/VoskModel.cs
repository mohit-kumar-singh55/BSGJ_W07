using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Vosk;

/// <summary>
/// Voskモデルを管理するSingletonクラス
/// ゲーム開始時に一度だけ読み込み、ゲーム全体で共有する
/// </summary>
public class VoskModel : Singleton<VoskModel>
{
    private string _fullModelPath;

    public bool ModelReady { get; set; } = false;
    public Model Model { get; private set; }

    private const string MODEL_PATH = "vosk-model-ja-0.22";

    void OnDestroy()
    {
        // メモリリークを防ぐためにモデルを破棄する
        Model?.Dispose();
    }

    void OnApplicationQuit()
    {
        // メモリリークを防ぐためにモデルを破棄する
        Model?.Dispose();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => LoadModel();

    private void LoadModel()
    {
        _fullModelPath = Path.Combine(Application.streamingAssetsPath, MODEL_PATH).Replace("\\", "/");

        // Unityがフリーズしないようにバックグラウンドでモデルを読み込む
        Task.Run(() =>
        {
            if (Model == null)
            {
                Debug.Log("Loading Vosk model...");
                Model = new Model(_fullModelPath);
                Debug.Log("Model loaded.");
            }

            ModelReady = true;
        });
    }
}