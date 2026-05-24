using UnityEngine;

/// <summary>
/// Arc Throw skill controller.
/// Hold to charge, release to throw with upward arc.
/// Charge time affects arc height and distance.
/// Level 3 unlock.
/// </summary>
public class ArcThrow : MonoBehaviour
{
    [Header("Arc Settings")]
    [SerializeField] private float minArcAngle = 15f;
    [SerializeField] private float maxArcAngle = 60f;
    [SerializeField] private float chargeSpeed = 1.5f;
    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private float arcForceMultiplier = 1.2f;

    [Header("Visual")]
    [SerializeField] private Color arcIndicatorColor = new Color(0.2f, 0.6f, 1.0f);
    [SerializeField] private float indicatorWidth = 100f;
    [SerializeField] private float indicatorHeight = 10f;

    private float currentCharge = 0f;
    private bool isCharging = false;
    private bool isActive = false;
    private Camera playerCamera;
    private GameObject chargeIndicator;
    private LineRenderer trajectoryLine;

    public float CurrentCharge => currentCharge;
    public float NormalizedCharge => Mathf.Clamp01(currentCharge / maxChargeTime);
    public bool IsCharging => isCharging;
    public bool IsActive => isActive;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Start()
    {
        CreateChargeIndicator();
        CreateTrajectoryLine();
    }

    private void Update()
    {
        if (!isActive) return;

        if (isCharging)
        {
            // Increase charge while holding
            currentCharge += Time.deltaTime * chargeSpeed;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxChargeTime);

            UpdateChargeIndicator();
            UpdateTrajectoryPreview();

            // Release on button up
            if (Input.GetButtonUp("Fire1"))
            {
                isCharging = false;
                // The throw will be handled by PlayerThrow
            }
        }
    }

    /// <summary>
    /// Activate arc throw mode.
    /// </summary>
    public void Activate()
    {
        isActive = true;
        if (chargeIndicator != null)
            chargeIndicator.SetActive(true);
        if (trajectoryLine != null)
            trajectoryLine.enabled = true;
    }

    /// <summary>
    /// Deactivate arc throw mode.
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        isCharging = false;
        currentCharge = 0f;
        if (chargeIndicator != null)
            chargeIndicator.SetActive(false);
        if (trajectoryLine != null)
            trajectoryLine.enabled = false;
    }

    /// <summary>
    /// Start charging (called when Fire1 is pressed).
    /// </summary>
    public void StartCharge()
    {
        if (!isActive) return;
        isCharging = true;
        currentCharge = 0f;
    }

    /// <summary>
    /// Apply arc to the throw velocity.
    /// </summary>
    public void ApplyArc(ref Vector3 velocity)
    {
        float chargeFactor = NormalizedCharge;

        // Add upward component for arc
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, chargeFactor);
        float arcRad = arcAngle * Mathf.Deg2Rad;

        // Rotate velocity upward
        Vector3 horizontalDir = velocity;
        horizontalDir.y = 0;
        if (horizontalDir.magnitude > 0.01f)
            horizontalDir = horizontalDir.normalized;

        float horizontalSpeed = velocity.magnitude * Mathf.Cos(arcRad);
        float verticalSpeed = velocity.magnitude * Mathf.Sin(arcRad);

        velocity = horizontalDir * horizontalSpeed * arcForceMultiplier + Vector3.up * verticalSpeed;

        // Reset charge
        currentCharge = 0f;
        isCharging = false;
    }

    private void CreateChargeIndicator()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        chargeIndicator = new GameObject("ArcChargeIndicator");
        chargeIndicator.transform.SetParent(canvas.transform, false);

        UnityEngine.UI.Image indicatorImage = chargeIndicator.AddComponent<UnityEngine.UI.Image>();
        indicatorImage.color = arcIndicatorColor;

        RectTransform rect = chargeIndicator.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.sizeDelta = new Vector2(indicatorWidth, indicatorHeight);
        rect.anchoredPosition = new Vector2(0, 65);

        indicatorImage.type = UnityEngine.UI.Image.Type.Filled;
        indicatorImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        indicatorImage.fillAmount = 0f;

        chargeIndicator.SetActive(false);
    }

    private void UpdateChargeIndicator()
    {
        if (chargeIndicator == null) return;

        UnityEngine.UI.Image img = chargeIndicator.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.fillAmount = NormalizedCharge;
            // Color shifts from blue to red as charge increases
            img.color = Color.Lerp(arcIndicatorColor, Color.red, NormalizedCharge);
        }
    }

    private void CreateTrajectoryLine()
    {
        GameObject lineGO = new GameObject("TrajectoryPreview");
        lineGO.transform.SetParent(transform, false);

        trajectoryLine = lineGO.AddComponent<LineRenderer>();
        trajectoryLine.startWidth = 0.02f;
        trajectoryLine.endWidth = 0.01f;
        trajectoryLine.positionCount = 20;
        trajectoryLine.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        trajectoryLine.startColor = arcIndicatorColor;
        trajectoryLine.endColor = new Color(arcIndicatorColor.r, arcIndicatorColor.g, arcIndicatorColor.b, 0.2f);

        trajectoryLine.enabled = false;
    }

    private void UpdateTrajectoryPreview()
    {
        if (trajectoryLine == null || playerCamera == null) return;

        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward;

        // Apply arc to preview
        float chargeFactor = NormalizedCharge;
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, chargeFactor);
        float arcRad = arcAngle * Mathf.Deg2Rad;

        Vector3 horizontalDir = direction;
        horizontalDir.y = 0;
        if (horizontalDir.magnitude > 0.01f)
            horizontalDir = horizontalDir.normalized;

        float speed = 15f * arcForceMultiplier;
        float horizontalSpeed = speed * Mathf.Cos(arcRad);
        float verticalSpeed = speed * Mathf.Sin(arcRad);

        Vector3 velocity = horizontalDir * horizontalSpeed + Vector3.up * verticalSpeed;

        // Simulate trajectory
        Vector3[] points = new Vector3[20];
        Vector3 pos = startPos;
        Vector3 vel = velocity;

        for (int i = 0; i < 20; i++)
        {
            points[i] = pos;
            vel += Physics.gravity * 0.1f;
            pos += vel * 0.1f;
        }

        trajectoryLine.positionCount = 20;
        trajectoryLine.SetPositions(points);
    }
}
