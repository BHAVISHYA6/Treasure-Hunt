using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    public static GameHUD Instance { get; private set; }

    [Header("UI Elements (Optional / Auto-Generated if null)")]
    public Canvas hudCanvas;
    public Text titleText;
    public Text livesText;
    public Text coinsText;
    public Text scoreText;
    public Text timerText;
    public Text mapText;
    public Text keysText;
    public Text objectiveText;
    public Text interactPromptText;
    public Text notificationText;

    [Header("Modal Panels")]
    public GameObject clueModalPanel;
    public Text clueTitleText;
    public Text clueBodyText;

    public GameObject pauseMenuPanel;
    public GameObject victoryModalPanel;
    public Text victoryStatsText;
    public Text victoryRankText;

    public GameObject levelClearModalPanel;
    public Text levelClearTitleText;
    public Text levelClearBodyText;

    public GameObject gameOverModalPanel;
    public Text gameOverTitleText;
    public Text gameOverReasonText;

    private Coroutine notificationRoutine;
    private PlayerController cachedPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (hudCanvas == null)
        {
            BuildProceduralUI();
        }
    }

    private void Start()
    {
        cachedPlayer = FindObjectOfType<PlayerController>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += UpdateHUD;
            GameManager.Instance.OnLivesChanged += UpdateHUD;
            GameManager.Instance.OnNotification += ShowNotification;
        }

        UpdateHUD();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= UpdateHUD;
            GameManager.Instance.OnLivesChanged -= UpdateHUD;
            GameManager.Instance.OnNotification -= ShowNotification;
        }
    }

    private void Update()
    {
        // Toggle pause menu with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (clueModalPanel != null && clueModalPanel.activeSelf)
            {
                CloseClueModal();
            }
            else
            {
                TogglePause();
            }
        }

        // Update interaction prompt from player
        if (cachedPlayer == null) cachedPlayer = FindObjectOfType<PlayerController>();
        if (cachedPlayer != null && interactPromptText != null)
        {
            var interactable = cachedPlayer.GetCurrentInteractable();
            if (interactable != null && interactable.CanInteract(cachedPlayer))
            {
                interactPromptText.text = interactable.GetInteractionPrompt();
                interactPromptText.gameObject.SetActive(true);
            }
            else
            {
                interactPromptText.gameObject.SetActive(false);
            }
        }

        // Update timer
        if (timerText != null && GameManager.Instance != null)
        {
            int mins = Mathf.FloorToInt(GameManager.Instance.timeElapsed / 60f);
            int secs = Mathf.FloorToInt(GameManager.Instance.timeElapsed % 60f);
            timerText.text = $"Time: {mins:00}:{secs:00}";
        }
    }

    public void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        string sceneName = SceneManager.GetActiveScene().name;
        if (titleText != null)
        {
            if (sceneName.Contains("1") || sceneName.Contains("Island") || sceneName.Contains("Main"))
                titleText.text = "LEVEL 1: THE TROPICAL COVE";
            else if (sceneName.Contains("2") || sceneName.Contains("Temple") || sceneName.Contains("Dungeon"))
                titleText.text = "LEVEL 2: FORBIDDEN TEMPLE";
            else
                titleText.text = "LEVEL 3: PIRATE KING'S VAULT";
        }

        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < GameManager.Instance.currentLives; i++) hearts += "❤️ ";
            for (int i = GameManager.Instance.currentLives; i < GameManager.Instance.maxLives; i++) hearts += "🖤 ";
            livesText.text = $"Lives: {hearts.Trim()}";
        }

        if (coinsText != null)
        {
            coinsText.text = $"Level Coins: {GameManager.Instance.levelCoinsCollected}/{GameManager.Instance.levelCoinsRequired}  (Total: {GameManager.Instance.totalCoins})";
        }
        if (scoreText != null) scoreText.text = $"Score: {GameManager.Instance.totalScore}";

        if (mapText != null)
        {
            bool showMap = sceneName.Contains("1") || sceneName.Contains("Island") || sceneName.Contains("Main");
            mapText.gameObject.SetActive(showMap);
            if (showMap)
            {
                mapText.text = $"Map Fragments: {GameManager.Instance.mapPieces}/{GameManager.TotalMapPieces}";
            }
        }

        if (keysText != null)
        {
            string keyStr = "Keys: ";
            bool any = false;
            if (GameManager.Instance.hasBronzeKey) { keyStr += "[Bronze] "; any = true; }
            if (GameManager.Instance.hasSilverKey) { keyStr += "[Silver] "; any = true; }
            if (GameManager.Instance.hasGoldKey) { keyStr += "[Gold] "; any = true; }
            if (!any) keyStr += "None";
            keysText.text = keyStr;
        }

        if (objectiveText != null)
        {
            objectiveText.text = $"Quest: {GameManager.Instance.currentObjective}";
        }

        // Show victory modal if game is finished
        if (GameManager.Instance.isGameFinished && victoryModalPanel != null)
        {
            ShowVictoryModal();
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationText == null) return;
        if (notificationRoutine != null) StopCoroutine(notificationRoutine);
        notificationRoutine = StartCoroutine(NotificationRoutine(message));
    }

    private IEnumerator NotificationRoutine(string msg)
    {
        notificationText.text = msg;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.8f);
        notificationText.gameObject.SetActive(false);
    }

    public void ShowClueModal(string title, string body)
    {
        if (clueModalPanel == null) return;
        if (clueTitleText != null) clueTitleText.text = title;
        if (clueBodyText != null) clueBodyText.text = body;
        clueModalPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseClueModal()
    {
        if (clueModalPanel != null) clueModalPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (pauseMenuPanel == null) return;
        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        if (GameManager.Instance != null) GameManager.Instance.isGamePaused = isPaused;
    }

    public void ShowVictoryModal()
    {
        if (victoryModalPanel == null) return;
        victoryModalPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        if (victoryStatsText != null && GameManager.Instance != null)
        {
            int mins = Mathf.FloorToInt(GameManager.Instance.timeElapsed / 60f);
            int secs = Mathf.FloorToInt(GameManager.Instance.timeElapsed % 60f);
            victoryStatsText.text = $"Total Coins Collected: {GameManager.Instance.totalCoins}\nFinal Adventure Score: {GameManager.Instance.totalScore}\nTotal Time: {mins:00}:{secs:00}";
        }
        if (victoryRankText != null && GameManager.Instance != null)
        {
            victoryRankText.text = $"Explorer Rank: {GameManager.Instance.GetExplorerRank()}!";
        }
    }

    // Button Callbacks
    public void OnResumeClicked() => TogglePause();
    public void OnRestartLevelClicked() => GameManager.Instance?.RestartCurrentLevel();
    public void OnRestartGameClicked() => GameManager.Instance?.RestartGame();
    public void OnLoadScene1Clicked() => GameManager.Instance?.LoadLevel(GameManager.Instance.scene1Name);
    public void OnLoadScene2Clicked() => GameManager.Instance?.LoadLevel(GameManager.Instance.scene2Name);
    public void OnLoadScene3Clicked() => GameManager.Instance?.LoadLevel(GameManager.Instance.scene3Name);
    public void OnToggleMuteClicked() => SoundManager.Instance?.ToggleMute();

    // Procedural UI Generator
    private void BuildProceduralUI()
    {
        GameObject canvasObj = new GameObject("ProceduralGameHUD");
        hudCanvas = canvasObj.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Top Bar Background
        GameObject topBar = CreateUIPanel(canvasObj.transform, "TopBar", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(0f, 80f), new Color(0.08f, 0.12f, 0.18f, 0.85f));

        // Title
        titleText = CreateUIText(topBar.transform, "Title", "LEVEL 1: THE TROPICAL COVE", 18, FontStyle.Bold, Color.yellow, TextAnchor.MiddleLeft, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(30f, 12f), new Vector2(360f, 30f), defaultFont);

        // Lives Tracker (Left lower)
        livesText = CreateUIText(topBar.transform, "Lives", "Lives: ❤️ ❤️ ❤️", 16, FontStyle.Bold, new Color(1f, 0.3f, 0.3f), TextAnchor.MiddleLeft, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(30f, -14f), new Vector2(250f, 25f), defaultFont);

        // Coins & Score (Right)
        coinsText = CreateUIText(topBar.transform, "Coins", "Level Coins: 0/10", 16, FontStyle.Bold, Color.yellow, TextAnchor.MiddleRight, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-200f, 12f), new Vector2(300f, 30f), defaultFont);
        scoreText = CreateUIText(topBar.transform, "Score", "Score: 0", 15, FontStyle.Normal, Color.white, TextAnchor.MiddleRight, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-200f, -12f), new Vector2(300f, 30f), defaultFont);
        timerText = CreateUIText(topBar.transform, "Timer", "Time: 00:00", 16, FontStyle.Normal, Color.cyan, TextAnchor.MiddleRight, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20f, 0f), new Vector2(140f, 40f), defaultFont);

        // Sub Bar: Map and Keys
        mapText = CreateUIText(canvasObj.transform, "MapTracker", "Map Fragments: 0/3", 16, FontStyle.Bold, new Color(0.4f, 0.9f, 1f), TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -95f), new Vector2(250f, 30f), defaultFont);
        keysText = CreateUIText(canvasObj.transform, "KeyTracker", "Keys: None", 16, FontStyle.Bold, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -125f), new Vector2(250f, 30f), defaultFont);

        // Quest Box (Bottom Left)
        GameObject questBox = CreateUIPanel(canvasObj.transform, "QuestBox", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(30f, 30f), new Vector2(500f, 60f), new Color(0.08f, 0.12f, 0.18f, 0.85f));
        objectiveText = CreateUIText(questBox.transform, "Objective", "Quest: Collect all 10 coins to sail to Temple!", 15, FontStyle.Normal, Color.white, TextAnchor.MiddleLeft, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(470f, 50f), defaultFont);

        // Notification Banner (Top Center)
        notificationText = CreateUIText(canvasObj.transform, "Notification", "", 20, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -120f), new Vector2(600f, 45f), defaultFont);
        notificationText.gameObject.SetActive(false);

        // Interaction Prompt (Center Bottom)
        interactPromptText = CreateUIText(canvasObj.transform, "InteractPrompt", "Press [E] to Interact", 18, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(500f, 40f), defaultFont);
        interactPromptText.gameObject.SetActive(false);

        // Build Modals
        BuildClueModal(canvasObj.transform, defaultFont);
        BuildPauseMenu(canvasObj.transform, defaultFont);
        BuildVictoryModal(canvasObj.transform, defaultFont);
        BuildLevelClearModal(canvasObj.transform, defaultFont);
        BuildGameOverModal(canvasObj.transform, defaultFont);
    }

    private void BuildClueModal(Transform parent, Font font)
    {
        clueModalPanel = CreateUIPanel(parent, "ClueModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 380f), new Color(0.92f, 0.85f, 0.7f, 0.98f));
        clueTitleText = CreateUIText(clueModalPanel.transform, "ClueTitle", "Captain's Parchment", 24, FontStyle.Bold, new Color(0.35f, 0.18f, 0.05f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(500f, 50f), font);
        clueBodyText = CreateUIText(clueModalPanel.transform, "ClueBody", "", 18, FontStyle.Normal, new Color(0.2f, 0.1f, 0.05f), TextAnchor.UpperLeft, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(480f, 210f), font);

        GameObject closeBtn = CreateUIButton(clueModalPanel.transform, "CloseBtn", "Close [ESC]", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 35f), new Vector2(160f, 40f), font);
        closeBtn.GetComponent<Button>().onClick.AddListener(CloseClueModal);

        clueModalPanel.SetActive(false);
    }

    private void BuildPauseMenu(Transform parent, Font font)
    {
        pauseMenuPanel = CreateUIPanel(parent, "PauseMenu", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(480f, 460f), new Color(0.08f, 0.12f, 0.18f, 0.95f));
        CreateUIText(pauseMenuPanel.transform, "PauseTitle", "ADVENTURE PAUSED", 26, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(400f, 50f), font);

        float startY = -100f;
        float spacing = 48f;

        CreateMenuButton(pauseMenuPanel.transform, "Resume", "Resume Game", new Vector2(0f, startY), font, OnResumeClicked);
        CreateMenuButton(pauseMenuPanel.transform, "Restart", "Restart Level", new Vector2(0f, startY - spacing), font, OnRestartLevelClicked);
        CreateMenuButton(pauseMenuPanel.transform, "Scene1", "Jump to Level 1 (Cove)", new Vector2(0f, startY - spacing * 2), font, OnLoadScene1Clicked);
        CreateMenuButton(pauseMenuPanel.transform, "Scene2", "Jump to Level 2 (Temple)", new Vector2(0f, startY - spacing * 3), font, OnLoadScene2Clicked);
        CreateMenuButton(pauseMenuPanel.transform, "Scene3", "Jump to Level 3 (Vault)", new Vector2(0f, startY - spacing * 4), font, OnLoadScene3Clicked);
        CreateMenuButton(pauseMenuPanel.transform, "AudioMute", "Toggle Sound & Music", new Vector2(0f, startY - spacing * 5), font, OnToggleMuteClicked);

        pauseMenuPanel.SetActive(false);
    }

    private void BuildVictoryModal(Transform parent, Font font)
    {
        victoryModalPanel = CreateUIPanel(parent, "VictoryModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 520f), new Color(0.06f, 0.1f, 0.16f, 0.98f));
        CreateUIText(victoryModalPanel.transform, "VictTitle", "🏆 THE END - TREASURE HUNT COMPLETE! 🏆", 25, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(580f, 50f), font);

        victoryRankText = CreateUIText(victoryModalPanel.transform, "VictRank", "Explorer Rank: Legendary Pirate King!", 20, FontStyle.Bold, Color.cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -85f), new Vector2(580f, 35f), font);
        victoryStatsText = CreateUIText(victoryModalPanel.transform, "VictStats", "Stats Summary", 18, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 25f), new Vector2(520f, 130f), font);

        CreateMenuButton(victoryModalPanel.transform, "RestartAgain", "🔄 Restart Again (Play From Beginning)", new Vector2(0f, -125f), font, OnRestartGameClicked);
        CreateMenuButton(victoryModalPanel.transform, "ReplayLevel3", "🔁 Replay Level 3 (Vault)", new Vector2(0f, -170f), font, OnRestartLevelClicked);
        CreateMenuButton(victoryModalPanel.transform, "CloseVic", "✨ Close & Explore Vault", new Vector2(0f, -215f), font, () => {
            victoryModalPanel.SetActive(false);
            Time.timeScale = 1f;
        });

        victoryModalPanel.SetActive(false);
    }

    private void BuildLevelClearModal(Transform parent, Font font)
    {
        levelClearModalPanel = CreateUIPanel(parent, "LevelClearModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 260f), new Color(0.08f, 0.18f, 0.28f, 0.96f));
        levelClearTitleText = CreateUIText(levelClearModalPanel.transform, "ClearTitle", "LEVEL COMPLETE!", 26, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(500f, 50f), font);
        levelClearBodyText = CreateUIText(levelClearModalPanel.transform, "ClearBody", "All Coins Collected!\nTraveling to next area...", 18, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(480f, 100f), font);
        levelClearModalPanel.SetActive(false);
    }

    private void BuildGameOverModal(Transform parent, Font font)
    {
        gameOverModalPanel = CreateUIPanel(parent, "GameOverModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(540f, 320f), new Color(0.2f, 0.05f, 0.05f, 0.96f));
        gameOverTitleText = CreateUIText(gameOverModalPanel.transform, "GOverTitle", "☠️ THE VOYAGE HAS ENDED ☠️", 24, FontStyle.Bold, new Color(1f, 0.3f, 0.3f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -45f), new Vector2(500f, 50f), font);
        gameOverReasonText = CreateUIText(gameOverModalPanel.transform, "GOverReason", "The Captain was claimed by the ocean depths.", 17, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(460f, 70f), font);

        CreateMenuButton(gameOverModalPanel.transform, "RetryLevel", "Try Again (Restart Level)", new Vector2(0f, -70f), font, () =>
        {
            gameOverModalPanel.SetActive(false);
            GameManager.Instance?.RestartCurrentLevel();
        });

        CreateMenuButton(gameOverModalPanel.transform, "RestartGame", "Restart From Beginning", new Vector2(0f, -120f), font, () =>
        {
            gameOverModalPanel.SetActive(false);
            GameManager.Instance?.RestartGame();
        });

        gameOverModalPanel.SetActive(false);
    }

    public void ShowLevelClearModal(string title, string body)
    {
        if (levelClearModalPanel == null) return;
        if (levelClearTitleText != null) levelClearTitleText.text = title;
        if (levelClearBodyText != null) levelClearBodyText.text = body;
        levelClearModalPanel.SetActive(true);
    }

    public void ShowGameOverModal(string reason)
    {
        if (gameOverModalPanel == null) return;
        if (gameOverReasonText != null) gameOverReasonText.text = reason;
        gameOverModalPanel.SetActive(true);
    }

    private GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private Text CreateUIText(Transform parent, string name, string content, int size, FontStyle style, Color color, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 delta, Font font)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = delta;
        var txt = textObj.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = size;
        txt.fontStyle = style;
        txt.color = color;
        txt.alignment = align;
        txt.text = content;
        return txt;
    }

    private GameObject CreateUIButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Font font)
    {
        GameObject btnObj = CreateUIPanel(parent, name, anchorMin, anchorMax, pivot, pos, size, new Color(0.25f, 0.35f, 0.5f, 1f));
        var btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.35f, 0.5f, 0.7f, 1f);
        cb.pressedColor = new Color(0.18f, 0.25f, 0.4f, 1f);
        btn.colors = cb;

        CreateUIText(btnObj.transform, "Label", label, 16, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, font);
        return btnObj;
    }

    private void CreateMenuButton(Transform parent, string name, string label, Vector2 pos, Font font, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btn = CreateUIButton(parent, name, label, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), pos, new Vector2(340f, 38f), font);
        btn.GetComponent<Button>().onClick.AddListener(onClick);
    }
}
