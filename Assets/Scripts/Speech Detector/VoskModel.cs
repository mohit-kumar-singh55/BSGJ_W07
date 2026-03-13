using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Vosk;

// Singleton manager for Vosk model to ensure it's loaded only once at the start of the game and shared across the game
public class VoskModel : Singleton<VoskModel>
{
    private string _fullModelPath;

    public bool ModelReady { get; set; } = false;
    public Model Model { get; private set; }

    private const string MODEL_PATH = "vosk-model-ja-0.22";

    void OnDestroy()
    {
        // Dispose model to avoid memory leaks
        Model?.Dispose();
    }

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        // Dispose model to avoid memory leaks
        Model?.Dispose();
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => LoadModel();

    private void LoadModel()
    {
        _fullModelPath = Path.Combine(Application.streamingAssetsPath, MODEL_PATH).Replace("\\", "/");

        // Load model in background to avoid freezing Unity
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