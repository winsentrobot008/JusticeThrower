using UnityEngine;

/// <summary>
/// Spin Throw skill controller.
/// Mouse horizontal movement controls spin rate and direction.
/// Spin affects bounce angle (curved reflection).
/// Level 2 unlock.
/// </summary>
public class SpinThrow : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private float maxSpinRate = 20f;
    [SerializeField] private float spinDecay = 5f;
    [SerializeField] private float spinBounceAngleInfluence = 0.3f;

    [Header("Visual")]
    [SerializeField] private float spinIndicatorSize = 0.5f;
    [SerializeField] private Color spinIndicatorColor = Color.cyan;

    private float currentSpinRate = 0f;
    private float lastMouseX = 0f;
    private bool isActive = false;
    private Camera playerCamera;
    private GameObject spinIndicator;

    public float CurrentSpinRate => currentSpinRate;
    public bool IsActive => isActive;
    public float NormalizedSpin => Mathf.Clamp01(Mathf.Abs(currentSpinRate) / maxSpinRate);

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Start()
    {
        CreateSpinIndicator();
    }

    private void Update()
    {
        if (!isActive) return;

        // Track mouse horizontal movement
        float mouseDelta = Input.GetAxis("Mouse X");
        float targetSpin = mouseDelta * mouseSensitivity;

        // Smoothly adjust spin rate
        currentSpinRate = Mathf.Lerp(currentSpinRate, targetSpin, Time.deltaTime * spinDecay);
        currentSpinRate = Mathf.Clamp(currentSpinRate, -maxSpinRate, maxSpinRate);

        // Update indicator
        UpdateSpinIndicator();
    }

    /// <summary>
    /// Activate spin throw mode.
    /// </summary>
    public void Activate()
    {
        isActive = true;
        currentSpinRate = 0f;
        if (spinIndicator != null)
            spinIndicator.SetActive(true);
    }

    /// <summary>
    /// Deactivate spin throw mode.
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        currentSpinRate = 0f;
        if (spinIndicator != null)
            spinIndicator.SetActive(false);
    }

    /// <summary>
    /// Apply spin to the throw velocity and angular velocity.
    /// </summary>
    public void ApplySpin(ref Vector3 velocity, ref Vector3 angularVelocity)
    {
        // Spin creates angular velocity around the up axis
        angularVelocity = Vector3.up * currentSpinRate;

        // Spin slightly curves the trajectory (Magnus effect simulation)
        Vector3 spinDirection = Vector3.Cross(velocity.normalized, Vector3.up) * (currentSpinRate / maxSpinRate);
        velocity += spinDirection * spinBounceAngleInfluence;

        // Reset spin after throw
        currentSpinRate = 0f;
    }

    /// <summary>
    /// Get modified bounce direction based on spin.
    /// </summary>
    public Vector3 GetModifiedBounceDirection(Vector3 originalDirection, Vector3 normal)
    {
        // Standard reflection
        Vector3 reflectDir = Vector3.Reflect(originalDirection, normal);

        // Add spin influence perpendicular to the reflection
        Vector3 spinInfluence = Vector3.Cross(reflectDir, normal) * (currentSpinRate / maxSpinRate) * spinBounceAngleInfluence;
        reflectDir += spinInfluence;

        return reflectDir.normalized;
    }

    private void CreateSpinIndicator()
    {
        // Create a simple UI indicator for spin
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        spinIndicator = new GameObject("SpinIndicator");
        spinIndicator.transform.SetParent(canvas.transform, false);

        // Create a bar that shows spin rate
        UnityEngine.UI.Image indicatorImage = spinIndicator.AddComponent<UnityEngine.UI.Image>();
        indicatorImage.color = spinIndicatorColor;

        RectTransform rect = spinIndicator.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.sizeDelta = new Vector2(80, 6);
        rect.anchoredPosition = new Vector2(0, 50);

        spinIndicator.SetActive(false);
    }

    private void UpdateSpinIndicator()
    {
        if (spinIndicator == null) return;

        UnityEngine.UI.Image img = spinIndicator.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            // Fill from center based on spin direction
            float normalizedSpin = NormalizedSpin;
            img.fillAmount = normalizedSpin;

            // Color: cyan for left, magenta for right
            img.color = currentSpinRate < 0 ? Color.cyan : Color.magenta;
        }
    }
}
