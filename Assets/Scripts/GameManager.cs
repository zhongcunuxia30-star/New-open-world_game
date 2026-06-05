using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲーム全体管理 & Player + Camera + UI の接続ハブ
/// シーンにこのコンポーネント付き GameObject を1つ配置する
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("参照")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerInput playerInput;

    [Header("設定")]
    [SerializeField] private bool pauseOnMenu = true;

    public PlayerController Player => playerController;
    public CameraController Camera => cameraController;
    public UIManager UI => uiManager;
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // カーソルロック
        if (cameraController != null)
            cameraController.LockCursor();

        // フレームレート設定
        Application.targetFrameRate = 60;
    }

    public void PauseGame()
    {
        if (pauseOnMenu)
        {
            Time.timeScale = 0f;
            IsPaused = true;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}
