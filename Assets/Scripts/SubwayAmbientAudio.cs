using UnityEngine;

/// <summary>
/// Plays ambient subway sounds.
/// - Train rumble (continuous low hum)
/// - Track clatter (rhythmic)
/// - Door open/close sounds
/// - Station announcements (random intervals)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SubwayAmbientAudio : MonoBehaviour
{
    [Header("Ambient Sounds")]
    [SerializeField] private AudioClip trainRumble;
    [SerializeField] private AudioClip trackClatter;
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip doorClose;
    [SerializeField] private AudioClip stationAnnouncement;

    [Header("Settings")]
    [SerializeField] [Range(0f, 1f)] private float rumbleVolume = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float clatterVolume = 0.15f;
    [SerializeField] [Range(0f, 1f)] private float doorVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float announcementVolume = 0.4f;
    [SerializeField] private float minAnnouncementInterval = 15f;
    [SerializeField] private float maxAnnouncementInterval = 45f;

    private AudioSource rumbleSource;
    private AudioSource clatterSource;
    private AudioSource oneShotSource;

    private void Awake()
    {
        // Create three audio sources for different layers
        rumbleSource = gameObject.AddComponent<AudioSource>();
        rumbleSource.loop = true;
        rumbleSource.volume = rumbleVolume;
        rumbleSource.spatialBlend = 0f; // 2D

        clatterSource = gameObject.AddComponent<AudioSource>();
        clatterSource.loop = true;
        clatterSource.volume = clatterVolume;
        clatterSource.spatialBlend = 0f;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.spatialBlend = 0f;
    }

    private void Start()
    {
        // Start continuous sounds
        if (trainRumble != null)
        {
            rumbleSource.clip = trainRumble;
            rumbleSource.Play();
        }
        else
        {
            // Generate procedural rumble
            StartCoroutine(GenerateProceduralRumble());
        }

        if (trackClatter != null)
        {
            clatterSource.clip = trackClatter;
            clatterSource.Play();
        }
        else
        {
            // Generate procedural clatter
            StartCoroutine(GenerateProceduralClatter());
        }

        // Schedule random announcements
        ScheduleNextAnnouncement();
    }

    private System.Collections.IEnumerator GenerateProceduralRumble()
    {
        // Create a simple low-frequency oscillation as rumble
        float elapsed = 0f;
        while (true)
        {
            elapsed += Time.deltaTime;
            float rumble = Mathf.Sin(elapsed * 2f) * 0.1f +
                          Mathf.Sin(elapsed * 3.7f) * 0.05f +
                          Mathf.Sin(elapsed * 5.1f) * 0.03f;
            rumbleSource.volume = rumbleVolume + rumble * 0.1f;
            yield return null;
        }
    }

    private System.Collections.IEnumerator GenerateProceduralClatter()
    {
        // Create rhythmic clatter
        while (true)
        {
            float clatter = Random.Range(0f, 1f);
            clatterSource.volume = clatterVolume + clatter * 0.05f;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    private void ScheduleNextAnnouncement()
    {
        float interval = Random.Range(minAnnouncementInterval, maxAnnouncementInterval);
        Invoke(nameof(PlayAnnouncement), interval);
    }

    private void PlayAnnouncement()
    {
        if (stationAnnouncement != null)
        {
            oneShotSource.PlayOneShot(stationAnnouncement, announcementVolume);
        }
        else
        {
            // Procedural "announcement" - just a beep
            oneShotSource.PlayOneShot(CreateBeepClip(), announcementVolume);
        }

        ScheduleNextAnnouncement();
    }

    private AudioClip CreateBeepClip()
    {
        int sampleRate = 44100;
        float duration = 0.3f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] samplesArray = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            samplesArray[i] = Mathf.Sin(t * 800f * Mathf.PI * 2f) * Mathf.Exp(-t * 10f);
        }

        AudioClip clip = AudioClip.Create("ProceduralBeep", samples, 1, sampleRate, false);
        clip.SetData(samplesArray, 0);
        return clip;
    }

    /// <summary>
    /// Play door open sound.
    /// </summary>
    public void PlayDoorOpen()
    {
        if (doorOpen != null)
            oneShotSource.PlayOneShot(doorOpen, doorVolume);
    }

    /// <summary>
    /// Play door close sound.
    /// </summary>
    public void PlayDoorClose()
    {
        if (doorClose != null)
            oneShotSource.PlayOneShot(doorClose, doorVolume);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
