using UnityEngine;

/// <summary>
/// Enhanced Player throw controller for Stage 2.
/// Supports multiple throwable items, Spin Throw, and Arc Throw skills.
/// 
/// Controls:
/// - Left Click: Throw current item
/// - Q / E: Cycle through unlocked items
/// - Mouse X (when Spin Throw active): Controls spin rate
/// - Hold Left Click (when Arc Throw active): Charge arc
/// </summary>
public class PlayerThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float defaultThrowForce = 15f;
    [SerializeField] private float cooldownTime = 1f;

    [Header("References")]
    [SerializeField] private Transform throwOrigin;

    [Header("Skill Components")]
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SpinThrow spinThrow;
    [SerializeField] private ArcThrow arcThrow;

    private float lastThrowTime = -1f;
    private Camera playerCamera;
    private GameObject currentItemPrefab;

    // Cooldown properties for UI
    public float CooldownRemaining => Mathf.Max(0, cooldownTime - (Time.time - lastThrowTime));
    public float CooldownNormalized => CooldownRemaining / cooldownTime;
    public bool IsOnCooldown => Time.time - lastThrowTime < cooldownTime;

    // Current item info for UI
    public string CurrentItemName => skillManager != null ? skillManager.CurrentItemName : "Slipper";
    public bool IsSpinActive => spinThrow != null && spinThrow.IsActive;
    public bool IsArcActive => arcThrow != null && arcThrow.IsActive;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (throwOrigin == null)
            throwOrigin = transform;

        // Auto-find skill components
        if (skillManager == null)
            skillManager = FindObjectOfType<SkillManager>();

        if (spinThrow == null)
            spinThrow = GetComponent<SpinThrow>();

        if (arcThrow == null)
            arcThrow = GetComponent<ArcThrow>();
    }

    private void Start()
    {
        UpdateCurrentItemPrefab();
    }

    private void Update()
    {
        // Item switching
        if (Input.GetKeyDown(KeyCode.Q))
            CycleItem(-1);
        if (Input.GetKeyDown(KeyCode.E))
            CycleItem(1);

        // Skill activation (toggle with number keys)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleSkill("Basic Bounce");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleSkill("Spin Throw");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ToggleSkill("Arc Throw");

        // Handle throw input
        HandleThrowInput();
    }

    private void HandleThrowInput()
    {
        // Arc Throw: hold to charge, release to throw
        if (arcThrow != null && arcThrow.IsActive)
        {
            if (Input.GetButtonDown("Fire1") && !IsOnCooldown)
            {
                arcThrow.StartCharge();
            }

            if (Input.GetButtonUp("Fire1") && arcThrow.IsCharging)
            {
                Throw();
            }
            return;
        }

        // Normal / Spin Throw: press to throw
        if (Input.GetButtonDown("Fire1") && !IsOnCooldown)
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (currentItemPrefab == null)
        {
            Debug.LogError("PlayerThrow: No current item prefab available!");
            return;
        }

        lastThrowTime = Time.time;

        // Instantiate item at throw origin
        GameObject itemGO = Instantiate(currentItemPrefab, throwOrigin.position, throwOrigin.rotation);

        // Get ThrowableItem component
        ThrowableItem throwable = itemGO.GetComponent<ThrowableItem>();
        if (throwable == null)
        {
            Debug.LogError("PlayerThrow: Instantiated item has no ThrowableItem component!");
            Destroy(itemGO);
            return;
        }

        // Calculate base velocity
        Vector3 velocity = throwOrigin.forward * throwable.BaseThrowForce;
        Vector3 angularVelocity = Vector3.zero;

        // Apply item-specific modifiers
        throwable.OnBeforeThrow(ref velocity, ref angularVelocity);

        // Apply Spin Throw modifier
        if (spinThrow != null && spinThrow.IsActive)
        {
            spinThrow.ApplySpin(ref velocity, ref angularVelocity);
        }

        // Apply Arc Throw modifier
        if (arcThrow != null && arcThrow.IsActive)
        {
            arcThrow.ApplyArc(ref velocity);
        }

        // Record throw for score tracking
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RecordThrow();

        // Launch
        throwable.Launch(velocity, angularVelocity);
    }

    private void CycleItem(int direction)
    {
        if (skillManager == null) return;

        var unlockedItems = skillManager.GetUnlockedItemNames();
        if (unlockedItems.Count == 0) return;

        int currentIndex = unlockedItems.IndexOf(skillManager.CurrentItemName);
        int newIndex = (currentIndex + direction + unlockedItems.Count) % unlockedItems.Count;

        skillManager.SwitchToItem(unlockedItems[newIndex]);
        UpdateCurrentItemPrefab();
    }

    private void UpdateCurrentItemPrefab()
    {
        if (skillManager != null)
        {
            currentItemPrefab = skillManager.GetCurrentItemPrefab();
        }
    }

    private void ToggleSkill(string skillName)
    {
        if (skillManager == null || !skillManager.IsSkillUnlocked(skillName))
            return;

        switch (skillName)
        {
            case "Spin Throw":
                if (spinThrow != null)
                {
                    if (spinThrow.IsActive)
                        spinThrow.Deactivate();
                    else
                    {
                        // Deactivate other skills
                        if (arcThrow != null) arcThrow.Deactivate();
                        spinThrow.Activate();
                    }
                }
                break;

            case "Arc Throw":
                if (arcThrow != null)
                {
                    if (arcThrow.IsActive)
                        arcThrow.Deactivate();
                    else
                    {
                        // Deactivate other skills
                        if (spinThrow != null) spinThrow.Deactivate();
                        arcThrow.Activate();
                    }
                }
                break;

            case "Basic Bounce":
                // Deactivate all skills
                if (spinThrow != null) spinThrow.Deactivate();
                if (arcThrow != null) arcThrow.Deactivate();
                break;
        }
    }

    /// <summary>
    /// Set the skill manager reference (called by scene setup).
    /// </summary>
    public void SetSkillManager(SkillManager manager)
    {
        skillManager = manager;
        UpdateCurrentItemPrefab();
    }
}
