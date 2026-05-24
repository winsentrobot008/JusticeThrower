using UnityEngine;

/// <summary>
/// NPC animation controller.
/// Provides idle sway, hit reaction, and dodge animations
/// using simple procedural transforms (no Animator needed).
/// </summary>
public class NPCAnimationController : MonoBehaviour
{
    [Header("Idle Sway")]
    [SerializeField] private bool enableIdleSway = true;
    [SerializeField] private float swayAmount = 2f;
    [SerializeField] private float swaySpeed = 1.5f;

    [Header("Hit Reaction")]
    [SerializeField] private float hitReactionDuration = 0.4f;
    [SerializeField] private float hitReactionAmount = 15f;
    [SerializeField] private float hitReactionRecoil = 0.3f;

    [Header("Dodge Animation")]
    [SerializeField] private float dodgeLeanAmount = 20f;
    [SerializeField] private float dodgeDuration = 0.3f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float idleTimer = 0f;

    // Animation state
    private bool isHitReaction = false;
    private float hitReactionTimer = 0f;
    private Vector3 hitDirection;

    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector3 dodgeDirection;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (enableIdleSway && !isHitReaction && !isDodging)
        {
            UpdateIdleSway();
        }

        if (isHitReaction)
        {
            UpdateHitReaction();
        }

        if (isDodging)
        {
            UpdateDodgeAnimation();
        }
    }

    private void UpdateIdleSway()
    {
        idleTimer += Time.deltaTime * swaySpeed;

        // Gentle sway
        float swayX = Mathf.Sin(idleTimer) * swayAmount * 0.01f;
        float swayZ = Mathf.Cos(idleTimer * 0.7f) * swayAmount * 0.005f;

        transform.localPosition = originalPosition + new Vector3(swayX, 0, swayZ);
        transform.localRotation = originalRotation * Quaternion.Euler(0, swayX * 5f, 0);
    }

    /// <summary>
    /// Play hit reaction animation.
    /// </summary>
    public void PlayHitReaction(Vector3 hitFromDirection)
    {
        isHitReaction = true;
        hitReactionTimer = 0f;
        hitDirection = hitFromDirection.normalized;
    }

    private void UpdateHitReaction()
    {
        hitReactionTimer += Time.deltaTime;
        float t = hitReactionTimer / hitReactionDuration;

        if (t >= 1f)
        {
            isHitReaction = false;
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            return;
        }

        // Ease out
        float ease = 1f - Mathf.Pow(1f - t, 3f);

        // Lean back in hit direction
        Vector3 leanDir = -hitDirection * hitReactionAmount * (1f - ease);
        transform.localRotation = originalRotation * Quaternion.Euler(leanDir.x, 0, leanDir.z);

        // Recoil backward
        transform.localPosition = originalPosition - hitDirection * hitReactionRecoil * (1f - ease);
    }

    /// <summary>
    /// Play dodge animation.
    /// </summary>
    public void PlayDodge(Vector3 dodgeDir)
    {
        isDodging = true;
        dodgeTimer = 0f;
        dodgeDirection = dodgeDir.normalized;
    }

    private void UpdateDodgeAnimation()
    {
        dodgeTimer += Time.deltaTime;
        float t = dodgeTimer / dodgeDuration;

        if (t >= 1f)
        {
            isDodging = false;
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            return;
        }

        // Lean into dodge direction
        float lean = Mathf.Sin(t * Mathf.PI) * dodgeLeanAmount;
        Vector3 leanAxis = Vector3.Cross(dodgeDirection, Vector3.up);
        transform.localRotation = originalRotation * Quaternion.Euler(0, 0, lean * Mathf.Sign(dodgeDirection.x));

        // Slight movement in dodge direction
        float move = Mathf.Sin(t * Mathf.PI) * 0.2f;
        transform.localPosition = originalPosition + dodgeDirection * move;
    }

    /// <summary>
    /// Reset all animations.
    /// </summary>
    public void ResetAnimations()
    {
        isHitReaction = false;
        isDodging = false;
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
