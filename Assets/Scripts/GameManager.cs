using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Progress")]
    public int totalCoins = 0;
    public int totalScore = 0;
    public int mapPieces = 0;
    public const int TotalMapPieces = 3;

    [Header("Lives & Health")]
    public int maxLives = 3;
    public int currentLives = 3;

    [Header("Per-Level Coin Progress")]
    public int levelCoinsCollected = 0;
    public int levelCoinsRequired = 10;
    public bool isLevelTransitioning = false;

    public bool hasBronzeKey = false;
    public bool hasSilverKey = false;
    public bool hasGoldKey = false;

    public float timeElapsed = 0f;
    public bool isGamePaused = false;
    public bool isGameFinished = false;

    [Header("Current Quest")]
    public string currentObjective = "Read Captain's Logbook to begin the hunt.";
    public bool isObjectiveComplete = false;

    [Header("Level Configuration")]
    public string scene1Name = "Scene1_IslandCove";
    public string scene2Name = "Scene2_AncientTemple";
    public string scene3Name = "Scene3_TreasureVault";

    public event Action OnStateChanged;
    public event Action OnLivesChanged;
    public event Action<string> OnNotification;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
        if (!isGameFinished && !isGamePaused)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGamePaused = false;
        isLevelTransitioning = false;
        levelCoinsCollected = 0;
        Time.timeScale = 1f;

        string name = scene.name;
        if (name.Contains("1") || name.Contains("Island") || name.Contains("Main"))
        {
            levelCoinsRequired = 10;
        }
        else if (name.Contains("2") || name.Contains("Temple"))
        {
            levelCoinsRequired = 10;
        }
        else
        {
            levelCoinsRequired = 20;
        }

        // Ensure SoundManager plays appropriate ambient soundtrack
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusicForScene(scene.name);
        }

        // Initialize scene objective if not already customized
        UpdateSceneObjective(scene.name);
        OnStateChanged?.Invoke();
        OnLivesChanged?.Invoke();
    }

    public void UpdateSceneObjective(string sceneName)
    {
        if (sceneName.Contains("1") || sceneName.Contains("Island") || sceneName.Contains("Main"))
        {
            SetObjective($"Explore Cove: Collect all {levelCoinsRequired} coins ({levelCoinsCollected}/{levelCoinsRequired}) to sail to Temple!");
        }
        else if (sceneName.Contains("2") || sceneName.Contains("Temple") || sceneName.Contains("Dungeon"))
        {
            SetObjective($"Temple Trial: Awaken 3 Altars & gather {levelCoinsRequired} treasures ({levelCoinsCollected}/{levelCoinsRequired})!");
        }
        else if (sceneName.Contains("3") || sceneName.Contains("Vault"))
        {
            SetObjective($"The Climax: Collect all {levelCoinsRequired} coins ({levelCoinsCollected}/{levelCoinsRequired}) or unlock the Pirate King's Grand Chest!");
        }
    }

    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        OnStateChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        levelCoinsCollected += amount;
        totalScore += amount * 100;
        SoundManager.Instance?.PlayCoinSound();
        Notify($"+{amount} Gold! ({levelCoinsCollected}/{levelCoinsRequired} in this area)");
        OnStateChanged?.Invoke();

        CheckLevelProgress();
    }

    public void CheckLevelProgress()
    {
        if (isLevelTransitioning) return;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("1") || sceneName.Contains("Island") || sceneName.Contains("Main"))
        {
            if (levelCoinsCollected >= levelCoinsRequired)
            {
                StartCoroutine(TransitionToLevel2Routine());
            }
            else
            {
                SetObjective($"Cove Quest: Collect all coins ({levelCoinsCollected}/{levelCoinsRequired}) to sail to Temple!");
            }
        }
        else if (sceneName.Contains("2") || sceneName.Contains("Temple"))
        {
            if (levelCoinsCollected >= levelCoinsRequired && hasSilverKey)
            {
                StartCoroutine(TransitionToLevel3Routine());
            }
            else
            {
                SetObjective($"Temple Quest: Altars & Treasures ({levelCoinsCollected}/{levelCoinsRequired}) to enter Vault!");
            }
        }
        else if (sceneName.Contains("3") || sceneName.Contains("Vault"))
        {
            if (levelCoinsCollected >= levelCoinsRequired)
            {
                FinishGame();
            }
            else
            {
                SetObjective($"Vault Quest: Gather all {levelCoinsRequired} coins ({levelCoinsCollected}/{levelCoinsRequired}) to claim victory!");
            }
        }
    }

    public void LoseLife(string cause)
    {
        currentLives--;
        SoundManager.Instance?.PlayHurt();
        OnLivesChanged?.Invoke();

        if (currentLives <= 0)
        {
            currentLives = 0;
            TriggerGameOver(cause);
        }
        else
        {
            Notify($"Danger! Lost 1 Life ({currentLives}/{maxLives} remaining)!");
        }
        OnStateChanged?.Invoke();
    }

    private void TriggerGameOver(string reason)
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        SoundManager.Instance?.PlayGameOver();
        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.ShowGameOverModal(reason);
        }
    }

    private IEnumerator TransitionToLevel2Routine()
    {
        isLevelTransitioning = true;
        SoundManager.Instance?.PlayLevelComplete();
        Notify("🌟 LEVEL 1 COMPLETE! 10 Coins Collected! Sailing to the Ancient Temple...");

        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.ShowLevelClearModal(
                "LEVEL 1 CLEARED!",
                "10 Gold Coins Collected!\nSetting sail to the Forbidden Temple..."
            );
        }

        yield return new WaitForSeconds(2.8f);
        LoadLevel(scene2Name);
    }

    public void OnAltarsComplete()
    {
        Notify("Temple Trial Complete! The Silver Key has manifested!");
        CheckLevelProgress();
    }

    private IEnumerator TransitionToLevel3Routine()
    {
        isLevelTransitioning = true;
        SoundManager.Instance?.PlayLevelComplete();
        Notify("🌟 LEVEL 2 COMPLETE! Temple Trial Conquered! Entering Pirate King's Vault...");

        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.ShowLevelClearModal(
                "LEVEL 2 CLEARED!",
                "Elemental Altars Awakened & Silver Key Claimed!\nEntering the Pirate King's Vault..."
            );
        }

        yield return new WaitForSeconds(2.8f);
        LoadLevel(scene3Name);
    }

    public void AddMapPiece()
    {
        mapPieces++;
        totalScore += 500;
        SoundManager.Instance?.PlayKeySound();
        Notify($"Found Map Fragment ({mapPieces}/{TotalMapPieces})!");

        if (mapPieces >= TotalMapPieces)
        {
            SetObjective("All fragments united! Claim the Bronze Key at the Sea Shrine!");
            Notify("Map Complete! Sea Shrine Activated!");
        }
        else
        {
            SetObjective($"Search the island for 3 Map Fragments ({mapPieces}/{TotalMapPieces})");
        }

        OnStateChanged?.Invoke();
    }

    public void CollectKey(string keyType)
    {
        SoundManager.Instance?.PlayKeySound();
        if (keyType.ToLower().Contains("bronze"))
        {
            hasBronzeKey = true;
            totalScore += 1000;
            Notify("Obtained Bronze Skeleton Key!");
            SetObjective("Unlock the Ancient Sea Gate to enter the Temple!");
        }
        else if (keyType.ToLower().Contains("silver"))
        {
            hasSilverKey = true;
            totalScore += 2000;
            Notify("Obtained Silver Key of Passage!");
            SetObjective("Unlock the Grand Temple Portal to enter the Vault!");
            CheckLevelProgress();
        }
        else if (keyType.ToLower().Contains("gold"))
        {
            hasGoldKey = true;
            totalScore += 5000;
            Notify("Obtained Golden Dragon Master Key!");
        }
        OnStateChanged?.Invoke();
    }

    public void Notify(string message)
    {
        OnNotification?.Invoke(message);
    }

    public void FinishGame()
    {
        if (isGameFinished) return;
        isGameFinished = true;
        totalScore += 10000;
        SoundManager.Instance?.PlayVictorySound();
        Notify("🏆 TREASURE HUNT COMPLETE! You have conquered the Pirate King's Vault!");

        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.TriggerVictoryAnimation();
        }

        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.ShowVictoryModal();
        }

        OnStateChanged?.Invoke();
    }

    public string GetExplorerRank()
    {
        if (totalCoins >= 45 && timeElapsed < 180f) return "Legendary Pirate King";
        if (totalCoins >= 30) return "Master Archaeologist";
        if (totalCoins >= 15) return "Daring Buccaneer";
        return "Apprentice Explorer";
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    public void LoadNextLevel()
    {
        string current = SceneManager.GetActiveScene().name;
        if (current.Contains("1") || current.Contains("Island") || current.Contains("Main"))
        {
            LoadLevel(scene2Name);
        }
        else if (current.Contains("2") || current.Contains("Temple") || current.Contains("Dungeon"))
        {
            LoadLevel(scene3Name);
        }
        else
        {
            RestartGame();
        }
    }

    public void RestartCurrentLevel()
    {
        currentLives = maxLives;
        levelCoinsCollected = 0;
        isLevelTransitioning = false;
        isGamePaused = false;
        Time.timeScale = 1f;
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        totalCoins = 0;
        totalScore = 0;
        mapPieces = 0;
        levelCoinsCollected = 0;
        currentLives = maxLives;
        isLevelTransitioning = false;
        hasBronzeKey = false;
        hasSilverKey = false;
        hasGoldKey = false;
        timeElapsed = 0f;
        isGameFinished = false;
        isGamePaused = false;
        Time.timeScale = 1f;
        LoadLevel(scene1Name);
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
    }
}
