using UnityEngine;
using System.Collections;

/// <summary>
/// Manages visual hit feedback:
/// - Screen shake on impact
/// - Screen flash on hit
/// - Hit marker indicator
/// </summary>
public class HitFeedbackManager : MonoBehaviour
{
    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Screen Flash")]
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color victimHitColor = Color.red;
    [SerializeField] private Color naughtyHitColor = Color.green;

    [Header("Hit Marker")]
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private float hitMarkerDuration = 0.5f;

    private Camera playerCamera;
    private Vector3 originalCameraPosition;
    private GameObject screenFlashOverlay;
    private Canvas uiCanvas;

    private static HitFeedbackManager _instance;
    public static HitFeedbackManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera != null)
            originalCameraPosition = playerCamera.transform.localPosition;

        CreateScreenFlashOverlay();
    }

    private void CreateScreenFlashOverlay()
    {
        uiCanvas = FindObjectOfType<Canvas>();
        if (uiCanvas == null) return;

        screenFlashOverlay = new GameObject("ScreenFlashOverlay");
        screenFlashOverlay.transform.SetParent(uiCanvas.transform, false);

        UnityEngine.UI.Image flashImage = screenFlashOverlay.AddComponent<UnityEngine.UI.Image>();
        flashImage.color = new Color(1, 1, 1, 0);

        RectTransform rect = screenFlashOverlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        screenFlashOverlay.SetActive(false);
    }

    /// <summary>
    /// Play hit feedback for hitting a NaughtyNPC (good hit).
    /// </summary>
    public void PlayNaughtyHitFeedback(Vector3 hitPosition)
    {
        StartCoroutine(ScreenShakeCoroutine());
        StartCoroutine(ScreenFlashCoroutine(naughtyHitColor));
        SpawnHitMarker(hitPosition, naughtyHitColor);
    }

    /// <summary>
    /// Play hit feedback for hitting a VictimNPC (bad hit).
    /// </summary>
    public void PlayVictimHitFeedback(Vector3 hitPosition)
    {
        StartCoroutine(ScreenShakeCoroutine());
        StartCoroutine(ScreenFlashCoroutine(victimHitColor));
        SpawnHitMarker(hitPosition, victimHitColor);
    }

    /// <summary>
    /// Play generic hit feedback.
    /// </summary>
    public void PlayHitFeedback(Vector3 hitPosition, bool isGood)
    {
        if (isGood)
            PlayNaughtyHitFeedback(hitPosition);
        else
            PlayVictimHitFeedback(hitPosition);
    }

    private IEnumerator ScreenShakeCoroutine()
    {
        if (playerCamera == null) yield break;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            playerCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalCameraPosition;
    }

    private IEnumerator ScreenFlashCoroutine(Color flashColor)
    {
        if (screenFlashOverlay == null) yield break;

        screenFlashOverlay.SetActive(true);
        UnityEngine.UI.Image flashImage = screenFlashOverlay.GetComponent<UnityEngine.UI.Image>();

        float elapsed = 0f;
        while (elapsed < hitFlashDuration)
        {
            float alpha = Mathf.Lerp(0.5f, 0f, elapsed / hitFlashDuration);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
        screenFlashOverlay.SetActive(false);
    }

    private void SpawnHitMarker(Vector3 position, Color color)
    {
        if (hitMarkerPrefab != null)
        {
            GameObject marker = Instantiate(hitMarkerPrefab, position, Quaternion.identity);
            Destroy(marker, hitMarkerDuration);
        }
        else
        {
            // Create simple cross marker
            GameObject marker = new GameObject("HitMarker");
            marker.transform.position = position;

            // Create a simple cross using lines
            LineRenderer line = marker.AddComponent<LineRenderer>();
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            line.positionCount = 4;
            line.SetPositions(new Vector3[] {
                position + Vector3.left * 0.2f,
                position + Vector3.right * 0.2f,
                position + Vector3.up * 0.2f,
                position + Vector3.down * 0.2f
            });
            line.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            line.startColor = color;
            line.endColor = color;

            Destroy(marker, hitMarkerDuration);
        }
    }
}
