using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level state.
/// - Detects when NaughtyNPC is hit
/// - Tracks victim hits (3 = game over)
/// - Shows "Level Complete" or "Game Over" message
/// - Resets scene after delay
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string levelCompleteMessage = "Level Complete!";
    [SerializeField] private string gameOverMessage = "Game Over! Too many innocents hit!";
    [SerializeField] private float resetDelay = 3f;
    [SerializeField] private int maxVictimHits = 3;

    [Header("UI References")]
    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private GameObject gameOverUI;

    private bool isLevelComplete = false;
    private bool isGameOver = false;
    private int victimHitCount = 0;
    private NaughtyNPC naughtyNPC;
    private VictimNPC[] victimNPCs;

    public int VictimHitCount => victimHitCount;
    public int MaxVictimHits => maxVictimHits;
    public bool IsLevelComplete => isLevelComplete;
    public bool IsGameOver => isGameOver;

    private static LevelManager _instance;
    public static LevelManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        // Find the NaughtyNPC
        naughtyNPC = FindObjectOfType<NaughtyNPC>();
        if (naughtyNPC == null)
        {
            Debug.LogWarning("LevelManager: No NaughtyNPC found in scene!");
        }

        // Find all VictimNPCs
        victimNPCs = FindObjectsOfType<VictimNPC>();
    }

    /// <summary>
    /// Called when a NaughtyNPC is hit.
    /// </summary>
    public void OnNaughtyHit(GameObject naughty)
    {
        if (isLevelComplete || isGameOver) return;
        isLevelComplete = true;

        Debug.Log(levelCompleteMessage);

        // Show level complete UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
            ui.ShowLevelComplete();
        else if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        // Reset scene after delay
        Invoke(nameof(ResetScene), resetDelay);
    }

    /// <summary>
    /// Called when a VictimNPC is hit.
    /// </summary>
    public void OnVictimHit(GameObject victim)
    {
        if (isLevelComplete || isGameOver) return;

        victimHitCount++;
        Debug.LogWarning($"Victim hit! ({victimHitCount}/{maxVictimHits})");

        // Update UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
            ui.UpdateVictimHitCount(victimHitCount, maxVictimHits);

        if (victimHitCount >= maxVictimHits)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log(gameOverMessage);

        // Show game over UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
            ui.ShowGameOver();
        else if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Reset scene after delay
        Invoke(nameof(ResetScene), resetDelay);
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
