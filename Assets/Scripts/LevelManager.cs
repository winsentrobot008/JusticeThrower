using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level state for MVP.
/// - Detects when NaughtyNPC is hit
/// - Shows "Level Complete" message
/// - Resets scene after 3 seconds
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string levelCompleteMessage = "Level Complete!";
    [SerializeField] private float resetDelay = 3f;

    [Header("UI References")]
    [SerializeField] private GameObject levelCompleteUI;

    private bool isLevelComplete = false;
    private NaughtyNPC naughtyNPC;
    private VictimNPC[] victimNPCs;

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

        // Subscribe to BouncePhysics events on all projectiles
        // (Projectiles will notify us via a simple static event approach)
    }

    /// <summary>
    /// Called when a NaughtyNPC is hit.
    /// </summary>
    public void OnNaughtyHit(GameObject naughty)
    {
        if (isLevelComplete) return;
        isLevelComplete = true;

        Debug.Log(levelCompleteMessage);

        // Show level complete UI
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        // Reset scene after delay
        Invoke(nameof(ResetScene), resetDelay);
    }

    /// <summary>
    /// Called when a VictimNPC is hit.
    /// </summary>
    public void OnVictimHit(GameObject victim)
    {
        Debug.LogWarning("Victim hit! That's bad!");
        // Future: track victim hits, game over at 3
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
