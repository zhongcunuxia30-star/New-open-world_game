using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 原神風UIマネージャー
/// Esc=メニュー, M=マップ, B=バッグ, C=キャラ画面
/// パネルの表示/非表示とカーソルロック連動を管理
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UIパネル参照")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject characterPanel;

    [Header("参照")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PlayerInput playerInput;

    private GameObject currentOpenPanel;
    private string previousActionMap;

    public bool IsAnyPanelOpen => currentOpenPanel != null;

    private void Start()
    {
        // 全パネルを初期非表示
        CloseAllPanels();
    }

    #region Input System コールバック

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentOpenPanel != null)
        {
            CloseCurrentPanel();
        }
        else
        {
            OpenPanel(menuPanel);
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TogglePanel(mapPanel);
    }

    public void OnBag(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TogglePanel(bagPanel);
    }

    public void OnCharacter(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TogglePanel(characterPanel);
    }

    #endregion

    #region パネル管理

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        if (currentOpenPanel == panel)
        {
            CloseCurrentPanel();
        }
        else
        {
            OpenPanel(panel);
        }
    }

    private void OpenPanel(GameObject panel)
    {
        if (panel == null) return;

        // 既に開いているパネルがあれば閉じる
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
        }

        panel.SetActive(true);
        currentOpenPanel = panel;

        // カーソルアンロック & 入力切替
        if (cameraController != null)
            cameraController.UnlockCursor();

        if (playerInput != null)
        {
            previousActionMap = playerInput.currentActionMap?.name ?? "Player";
            playerInput.SwitchCurrentActionMap("UI");
        }

        Debug.Log($"[UIManager] パネル開: {panel.name}");
    }

    private void CloseCurrentPanel()
    {
        if (currentOpenPanel == null) return;

        currentOpenPanel.SetActive(false);
        Debug.Log($"[UIManager] パネル閉: {currentOpenPanel.name}");
        currentOpenPanel = null;

        // カーソルロック & 入力復帰
        if (cameraController != null)
            cameraController.LockCursor();

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(previousActionMap ?? "Player");
        }
    }

    private void CloseAllPanels()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (mapPanel != null) mapPanel.SetActive(false);
        if (bagPanel != null) bagPanel.SetActive(false);
        if (characterPanel != null) characterPanel.SetActive(false);
        currentOpenPanel = null;
    }

    #endregion

    #region 公開メソッド

    public void ForceCloseAll()
    {
        CloseAllPanels();

        if (cameraController != null)
            cameraController.LockCursor();

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");
    }

    #endregion
}
