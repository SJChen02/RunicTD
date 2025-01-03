using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    TowerShoot,
    TowerUpgrade,
    TowerSell,
    TowerPlace,
    EnemyDie,
    EnemyHit,
    EnemyBossWarning,
    GameOver,
    GameWin,
    WaveStart,
    WaveEnd,
    ButtonClick,
    BackgroundMusic
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private int audioSourcePoolSize = 20;
    private static SoundManager instance;
    private List<AudioSource> audioSourcePool;

    // Background music source
    private AudioSource backgroundMusicSource;

    // Volume categories
    public static float MasterVolume = 1f;
    public static float MusicVolume = 0.15f;
    public static float SoundEffectVolume = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitialiseAudioSourcePool();

            // Initialize the background music source
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true; // Ensure looping
            backgroundMusicSource.playOnAwake = false;
            backgroundMusicSource.volume = MusicVolume * MasterVolume;
            PlayBackgroundMusic(SoundType.BackgroundMusic);
        }
    }

    public static void PlayBackgroundMusic(SoundType soundType)
    {
        if (instance.backgroundMusicSource.isPlaying)
        {
            instance.backgroundMusicSource.Stop();
        }
        instance.backgroundMusicSource.clip = instance.soundList[(int)soundType];
        instance.backgroundMusicSource.volume = MusicVolume * MasterVolume;
        instance.backgroundMusicSource.Play();
    }
    
    public static void UpdateMusicVolume()
    {
        if (instance.backgroundMusicSource != null)
        {
            instance.backgroundMusicSource.volume = MusicVolume * MasterVolume;
        }
    }

    private void InitialiseAudioSourcePool()
    {
        audioSourcePool = new List<AudioSource>();
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSourcePool.Add(source);
        }
    }

    private AudioSource GetAvailableAudioSource(int priority)
    {
        // Find an available AudioSource that is not playing
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If all AudioSources are playing, find the lowest-priority source
        AudioSource lowestPrioritySource = null;
        foreach (var source in audioSourcePool)
        {
            if (lowestPrioritySource == null || source.priority > lowestPrioritySource.priority)
            {
                lowestPrioritySource = source;
            }
        }

        // If a low-priority sound can be overridden, stop it
        if (lowestPrioritySource != null && lowestPrioritySource.priority > priority)
        {
            lowestPrioritySource.Stop();
            return lowestPrioritySource;
        }

        return null;
    }

    public static void PlaySound(SoundType soundType, float volume = 1f, int priority = 128)
    {
        AudioSource source = instance.GetAvailableAudioSource(priority);
        if (source == null) return; // No available AudioSource

        source.clip = instance.soundList[(int)soundType];
        source.volume = volume * SoundEffectVolume * MasterVolume;
        source.priority = priority; // Set sound priority
        source.Play();
    }
}
