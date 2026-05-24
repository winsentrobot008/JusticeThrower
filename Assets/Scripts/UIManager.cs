using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages basic UI for MVP:
/// - Crosshair (simple十字)
/// - Cooldown bar (Image fill)
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

    [Header("Level Complete")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Text levelCompleteText;

    private PlayerThrow playerThrow;

    private void Awake()
    {
        playerThrow = FindObjectOfType<PlayerThrow>();

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    private void Start()
    {
        CreateCrosshair();
    }

    private void Update()
    {
        UpdateCooldownBar();
    }

    private void CreateCrosshair()
    {
        if (crosshair != null) return;

        // Create a canvas if none exists
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("UI_Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

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

        // Create cooldown bar if not assigned
        if (cooldownBar == null)
        {
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

        // Create level complete panel if not assigned
        if (levelCompletePanel == null)
        {
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

    /// <summary>
    /// Show level complete UI.
    /// </summary>
    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }
}
