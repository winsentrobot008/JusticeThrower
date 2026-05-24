using UnityEngine;

/// <summary>
/// Plays sound effects for bounce and hit events.
/// Uses AudioSource on the same GameObject or creates one.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BounceSoundFX : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip hitNaughtySound;
    [SerializeField] private AudioClip hitVictimSound;
    [SerializeField] private AudioClip splashSound;

    [Header("Settings")]
    [SerializeField] private float bounceVolume = 0.5f;
    [SerializeField] private float hitVolume = 0.8f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    private AudioSource audioSource;
    private BouncePhysics bouncePhysics;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        bouncePhysics = GetComponent<BouncePhysics>();
        if (bouncePhysics != null)
        {
            bouncePhysics.onBounce.AddListener(PlayBounceSound);
            bouncePhysics.onHitNaughty.AddListener(PlayHitNaughtySound);
            bouncePhysics.onHitVictim.AddListener(PlayHitVictimSound);
        }
    }

    private void PlayBounceSound()
    {
        if (bounceSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(bounceSound, bounceVolume);
        }
    }

    private void PlayHitNaughtySound(GameObject naughty)
    {
        if (hitNaughtySound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(hitNaughtySound, hitVolume);
        }
    }

    private void PlayHitVictimSound(GameObject victim)
    {
        if (hitVictimSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(hitVictimSound, hitVolume);
        }
    }

    /// <summary>
    /// Play splash sound (for Water Balloon).
    /// </summary>
    public void PlaySplashSound()
    {
        if (splashSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(splashSound, hitVolume);
        }
    }

    private void OnDestroy()
    {
        if (bouncePhysics != null)
        {
            bouncePhysics.onBounce.RemoveListener(PlayBounceSound);
            bouncePhysics.onHitNaughty.RemoveListener(PlayHitNaughtySound);
            bouncePhysics.onHitVictim.RemoveListener(PlayHitVictimSound);
        }
    }
}
