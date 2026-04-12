using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    // Singleton Instance
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    [SerializeField] private float musicFadeDuration = 1.0f;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tek seferlik ses efekti çalar (Patlama, ateþ etme vb.)
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Arka plan müziðini deðiþtirir (Yumuþak geçiþli)
    /// </summary>
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource.clip == musicClip) return;

        StartCoroutine(FadeMusic(musicClip, loop));
    }

    private IEnumerator FadeMusic(AudioClip newClip, bool loop)
    {
        float startVolume = musicSource.volume;

        // Fade Out
        if (musicSource.isPlaying)
        {
            while (musicSource.volume > 0)
            {
                musicSource.volume -= startVolume * Time.deltaTime / musicFadeDuration;
                yield return null;
            }
            musicSource.Stop();
        }

        // Yeni müziði hazýrla
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade In
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / musicFadeDuration;
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}