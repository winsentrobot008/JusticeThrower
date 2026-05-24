using UnityEngine;
using TMPro;

/// <summary>
/// Manages score tracking for the level.
/// Tracks: accuracy, victim hits, time, score.
/// Displays score using TextMeshPro.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int baseScore = 1000;
    [SerializeField] private int naughtyHitBonus = 500;
    [SerializeField] private int victimHitPenalty = -300;
    [SerializeField] private int accuracyBonus = 200;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private int totalScore = 0;
    private int totalThrows = 0;
    private int successfulHits = 0;
    private int naughtyHits = 0;
    private int victimHits = 0;
    private float levelStartTime;
    private bool isLevelComplete = false;

    private Canvas canvas;
    private LevelManager levelManager;

    private static ScoreManager _instance;
    public static ScoreManager Instance => _instance;

    public int TotalScore => totalScore;
    public float Accuracy => totalThrows > 0 ? (float)successfulHits / totalThrows * 100f : 0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start()
    {
        levelStartTime = Time.time;
        CreateScoreUI();
    }

    private void Update()
    {
        if (!isLevelComplete)
        {
            UpdateTimeDisplay();
        }
    }

    private void CreateScoreUI()
    {
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("UI_Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Score text (top-left)
        if (scoreText == null)
        {
            GameObject scoreGO = new GameObject("ScoreText");
            scoreGO.transform.SetParent(canvas.transform, false);
            scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
            scoreText.fontSize = 18;
            scoreText.alignment = TextAlignmentOptions.TopLeft;
            scoreText.color = Color.white;
            scoreText.text = "Score: 0";
            RectTransform scoreRect = scoreGO.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0, 1);
            scoreRect.anchorMax = new Vector2(0, 1);
            scoreRect.sizeDelta = new Vector2(200, 30);
            scoreRect.anchoredPosition = new Vector2(10, -10);
        }

        // Accuracy text (top-left, below score)
        if (accuracyText == null)
        {
            GameObject accGO = new GameObject("AccuracyText");
            accGO.transform.SetParent(canvas.transform, false);
            accuracyText = accGO.AddComponent<TextMeshProUGUI>();
            accuracyText.fontSize = 14;
            accuracyText.alignment = TextAlignmentOptions.TopLeft;
            accuracyText.color = Color.white;
            accuracyText.text = "Accuracy: 0%";
            RectTransform accRect = accGO.GetComponent<RectTransform>();
            accRect.anchorMin = new Vector2(0, 1);
            accRect.anchorMax = new Vector2(0, 1);
            accRect.sizeDelta = new Vector2(200, 20);
            accRect.anchoredPosition = new Vector2(10, -35);
        }

        // Time text (top-center)
        if (timeText == null)
        {
            GameObject timeGO = new GameObject("TimeText");
            timeGO.transform.SetParent(canvas.transform, false);
            timeText = timeGO.AddComponent<TextMeshProUGUI>();
            timeText.fontSize = 16;
            timeText.alignment = TextAlignmentOptions.TopCenter;
            timeText.color = Color.white;
            timeText.text = "Time: 0s";
            RectTransform timeRect = timeGO.GetComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0.5f, 1);
            timeRect.anchorMax = new Vector2(0.5f, 1);
            timeRect.sizeDelta = new Vector2(150, 30);
            timeRect.anchoredPosition = new Vector2(0, -10);
        }

        // Final score text (hidden initially, shown on level complete)
        if (finalScoreText == null)
        {
            GameObject finalGO = new GameObject("FinalScoreText");
            finalGO.transform.SetParent(canvas.transform, false);
            finalScoreText = finalGO.AddComponent<TextMeshProUGUI>();
            finalScoreText.fontSize = 20;
            finalScoreText.alignment = TextAlignmentOptions.MidCenter;
            finalScoreText.color = Color.green;
            finalScoreText.text = "";
            RectTransform finalRect = finalGO.GetComponent<RectTransform>();
            finalRect.anchorMin = new Vector2(0.5f, 0.5f);
            finalRect.anchorMax = new Vector2(0.5f, 0.5f);
            finalRect.sizeDelta = new Vector2(300, 100);
            finalRect.anchoredPosition = new Vector2(0, -60);
        }
    }

    /// <summary>
    /// Record a throw attempt.
    /// </summary>
    public void RecordThrow()
    {
        totalThrows++;
    }

    /// <summary>
    /// Record a successful hit on NaughtyNPC.
    /// </summary>
    public void RecordNaughtyHit()
    {
        naughtyHits++;
        successfulHits++;
        totalScore += naughtyHitBonus;
        UpdateScoreDisplay();
    }

    /// <summary>
    /// Record a hit on VictimNPC.
    /// </summary>
    public void RecordVictimHit()
    {
        victimHits++;
        totalScore += victimHitPenalty;
        if (totalScore < 0) totalScore = 0;
        UpdateScoreDisplay();
    }

    /// <summary>
    /// Calculate final score on level complete.
    /// </summary>
    public void CalculateFinalScore()
    {
        isLevelComplete = true;

        // Base score
        totalScore += baseScore;

        // Accuracy bonus
        float accuracy = Accuracy;
        if (accuracy >= 80f)
            totalScore += accuracyBonus;

        // Time bonus (faster = more points)
        float timeTaken = Time.time - levelStartTime;
        int timeBonus = Mathf.Max(0, Mathf.RoundToInt(500 - timeTaken * 10));
        totalScore += timeBonus;

        UpdateScoreDisplay();

        // Show final score
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {totalScore}\n" +
                                  $"Accuracy: {accuracy:F1}%\n" +
                                  $"Time: {timeTaken:F1}s\n" +
                                  $"Naughty Hits: {naughtyHits} | Victim Hits: {victimHits}";
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {totalScore}";

        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {Accuracy:F1}%";
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            float timeTaken = Time.time - levelStartTime;
            timeText.text = $"Time: {timeTaken:F1}s";
        }
    }
}
