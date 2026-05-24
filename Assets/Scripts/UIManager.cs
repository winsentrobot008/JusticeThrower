using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages UI for Stage 3:
/// - Crosshair (simple十字)
/// - Cooldown bar (Image fill)
/// - Item name display
/// - Active skill display
/// - Victim hit count
/// - Level complete panel
/// - Game over panel
/// - Hit feedback (screen flash handled by HitFeedbackManager)
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Crosshair")]
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private float crosshairSize = 20f;
    [SerializeField] private float crosshairThickness = 2f;

    [Header("Cooldown Bar")]
    [SerializeField] private Image cooldownBar;
    [SerializeField] private Color cooldownReadyColor = Color.green;
    [SerializeField] private Color cooldownActiveColor = Color.red;

    [Header("Item Info")]
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text skillNameText;

    [Header("Victim Hit Count")]
    [SerializeField] private Text victimHitText;
    [SerializeField] private Color victimHitNormalColor = Color.white;
    [SerializeField] private Color victimHitWarningColor = Color.yellow;
    [SerializeField] private Color victimHitDangerColor = Color.red;

    [Header("Level Complete")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Text levelCompleteText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;

    private PlayerThrow playerThrow;
    private LevelManager levelManager;
    private Canvas canvas;

    private void Awake()
    {
        playerThrow = FindObjectOfType<PlayerThrow>();
        levelManager = FindObjectOfType<LevelManager>();

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        CreateUI();
    }

    private void Update()
    {
        UpdateCooldownBar();
        UpdateItemInfo();
    }

    private void CreateUI()
    {
        if (crosshair != null) return;

        // Create a canvas if none exists
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("UI_Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        CreateCrosshairUI();
        CreateCooldownBar();
        CreateItemInfoText();
        CreateVictimHitText();
        CreateLevelCompletePanel();
        CreateGameOverPanel();
    }

    private void CreateCrosshairUI()
    {
        // Create crosshair container
        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(canvas.transform, false);
        crosshair = crosshairGO.AddComponent<RectTransform>();
        crosshair.anchorMin = new Vector2(0.5f, 0.5f);
        crosshair.anchorMax = new Vector2(0.5f, 0.5f);
        crosshair.sizeDelta = new Vector2(crosshairSize, crosshairSize);

        // Create four lines for crosshair
        CreateCrosshairLine(crosshair, "Line_Top", new Vector2(0, crosshairSize / 2f), new Vector2(crosshairThickness, crosshairSize / 2f));
        CreateCrosshairLine(crosshair, "Line_Bottom", new Vector2(0, -crosshairSize / 2f), new Vector2(crosshairThickness, crosshairSize / 2f));
        CreateCrosshairLine(crosshair, "Line_Left", new Vector2(-crosshairSize / 2f, 0), new Vector2(crosshairSize / 2f, crosshairThickness));
        CreateCrosshairLine(crosshair, "Line_Right", new Vector2(crosshairSize / 2f, 0), new Vector2(crosshairSize / 2f, crosshairThickness));

        // Create center dot
        GameObject dot = new GameObject("Center_Dot");
        dot.transform.SetParent(crosshair, false);
        Image dotImage = dot.AddComponent<Image>();
        dotImage.color = Color.white;
        RectTransform dotRect = dot.GetComponent<RectTransform>();
        dotRect.anchorMin = new Vector2(0.5f, 0.5f);
        dotRect.anchorMax = new Vector2(0.5f, 0.5f);
        dotRect.sizeDelta = new Vector2(4, 4);
        dotRect.anchoredPosition = Vector2.zero;
    }

    private void CreateCrosshairLine(RectTransform parent, string name, Vector2 position, Vector2 size)
    {
        GameObject line = new GameObject(name);
        line.transform.SetParent(parent, false);
        Image lineImage = line.AddComponent<Image>();
        lineImage.color = Color.white;
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.sizeDelta = size;
        lineRect.anchoredPosition = position;
    }

    private void CreateCooldownBar()
    {
        if (cooldownBar != null) return;

        GameObject barGO = new GameObject("CooldownBar");
        barGO.transform.SetParent(canvas.transform, false);
        cooldownBar = barGO.AddComponent<Image>();
        cooldownBar.color = cooldownReadyColor;
        RectTransform barRect = barGO.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0);
        barRect.anchorMax = new Vector2(0.5f, 0);
        barRect.sizeDelta = new Vector2(100, 10);
        barRect.anchoredPosition = new Vector2(0, 30);
        cooldownBar.type = Image.Type.Filled;
        cooldownBar.fillMethod = Image.FillMethod.Horizontal;
        cooldownBar.fillAmount = 1f;
    }

    private void CreateItemInfoText()
    {
        // Item name text (bottom-left)
        if (itemNameText == null)
        {
            GameObject itemGO = new GameObject("ItemNameText");
            itemGO.transform.SetParent(canvas.transform, false);
            itemNameText = itemGO.AddComponent<Text>();
            itemNameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            itemNameText.fontSize = 16;
            itemNameText.alignment = TextAnchor.LowerLeft;
            itemNameText.color = Color.white;
            itemNameText.text = "Item: Slipper";
            RectTransform itemRect = itemGO.GetComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0);
            itemRect.anchorMax = new Vector2(0, 0);
            itemRect.sizeDelta = new Vector2(200, 30);
            itemRect.anchoredPosition = new Vector2(10, 10);
        }

        // Skill name text (bottom-center, above cooldown bar)
        if (skillNameText == null)
        {
            GameObject skillGO = new GameObject("SkillNameText");
            skillGO.transform.SetParent(canvas.transform, false);
            skillNameText = skillGO.AddComponent<Text>();
            skillNameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            skillNameText.fontSize = 14;
            skillNameText.alignment = TextAnchor.LowerCenter;
            skillNameText.color = new Color(0.5f, 0.8f, 1f);
            skillNameText.text = "Skill: Basic Bounce";
            RectTransform skillRect = skillGO.GetComponent<RectTransform>();
            skillRect.anchorMin = new Vector2(0.5f, 0);
            skillRect.anchorMax = new Vector2(0.5f, 0);
            skillRect.sizeDelta = new Vector2(200, 20);
            skillRect.anchoredPosition = new Vector2(0, 50);
        }
    }

    private void CreateVictimHitText()
    {
        if (victimHitText == null)
        {
            GameObject victimGO = new GameObject("VictimHitText");
            victimGO.transform.SetParent(canvas.transform, false);
            victimHitText = victimGO.AddComponent<Text>();
            victimHitText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            victimHitText.fontSize = 14;
            victimHitText.alignment = TextAnchor.UpperRight;
            victimHitText.color = victimHitNormalColor;
            victimHitText.text = "Innocents Hit: 0 / 3";
            RectTransform victimRect = victimGO.GetComponent<RectTransform>();
            victimRect.anchorMin = new Vector2(1, 1);
            victimRect.anchorMax = new Vector2(1, 1);
            victimRect.sizeDelta = new Vector2(160, 30);
            victimRect.anchoredPosition = new Vector2(-10, -10);
        }
    }

    private void CreateLevelCompletePanel()
    {
        if (levelCompletePanel != null) return;

        levelCompletePanel = new GameObject("LevelCompletePanel");
        levelCompletePanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = levelCompletePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(300, 100);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = levelCompletePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(levelCompletePanel.transform, false);
        levelCompleteText = textGO.AddComponent<Text>();
        levelCompleteText.text = "Level Complete!";
        levelCompleteText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        levelCompleteText.fontSize = 24;
        levelCompleteText.alignment = TextAnchor.MiddleCenter;
        levelCompleteText.color = Color.green;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        levelCompletePanel.SetActive(false);
    }

    private void CreateGameOverPanel()
    {
        if (gameOverPanel != null) return;

        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(300, 100);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(gameOverPanel.transform, false);
        gameOverText = textGO.AddComponent<Text>();
        gameOverText.text = "Game Over!";
        gameOverText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        gameOverText.fontSize = 24;
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.color = Color.red;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        gameOverPanel.SetActive(false);
    }

    private void UpdateCooldownBar()
    {
        if (cooldownBar == null || playerThrow == null) return;

        if (playerThrow.IsOnCooldown)
        {
            cooldownBar.fillAmount = playerThrow.CooldownNormalized;
            cooldownBar.color = cooldownActiveColor;
        }
        else
        {
            cooldownBar.fillAmount = 1f;
            cooldownBar.color = cooldownReadyColor;
        }
    }

    private void UpdateItemInfo()
    {
        if (playerThrow == null) return;

        // Update item name
        if (itemNameText != null)
        {
            itemNameText.text = $"Item: {playerThrow.CurrentItemName} (Q/E)";
        }

        // Update skill name
        if (skillNameText != null)
        {
            string skill = "Basic Bounce";
            if (playerThrow.IsSpinActive)
                skill = "Spin Throw";
            else if (playerThrow.IsArcActive)
                skill = "Arc Throw";

            skillNameText.text = $"Skill: {skill} [1/2/3]";
        }
    }

    /// <summary>
    /// Update victim hit count display.
    /// </summary>
    public void UpdateVictimHitCount(int count, int max)
    {
        if (victimHitText == null) return;

        victimHitText.text = $"Innocents Hit: {count} / {max}";

        // Color based on severity
        if (count >= max)
            victimHitText.color = victimHitDangerColor;
        else if (count >= max - 1)
            victimHitText.color = victimHitWarningColor;
        else
            victimHitText.color = victimHitNormalColor;
    }

    /// <summary>
    /// Show level complete UI.
    /// </summary>
    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    /// <summary>
    /// Show game over UI.
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
