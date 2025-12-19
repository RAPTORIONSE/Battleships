using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager _instance;
    public static SoundManager Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    [SerializeField] List<AudioClip> sinkingAudioClips;
    [SerializeField] List<AudioClip> explotionAudioClips;
    [SerializeField] List<AudioClip> fireAudioClips;
    [SerializeField] List<AudioClip> waterAudioClips;
    [SerializeField] List<AudioClip> buttonAudioClips;
    [SerializeField] List<AudioClip> victoryAudioClips;
    private AudioSource buttonAudioSource;
    private AudioSource explosionAudioSource;
    private AudioSource fireAudioSource;
    private AudioSource sinkingAudioSource;
    private AudioSource waterAudioSource;
    private AudioSource victoryAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonAudioSource = this.gameObject.AddComponent<AudioSource>();
        explosionAudioSource = this.gameObject.AddComponent<AudioSource>();
        fireAudioSource = this.gameObject.AddComponent<AudioSource>();
        sinkingAudioSource = this.gameObject.AddComponent<AudioSource>();
        waterAudioSource = this.gameObject.AddComponent<AudioSource>();
        victoryAudioSource = this.gameObject.AddComponent<AudioSource>();
        victoryAudioSource.clip = victoryAudioClips[Random.Range(0, victoryAudioClips.Count)];
    }

    /// <summary>
    /// Loads and plays a random button sound
    /// </summary>
    public void PlayButtonSound()
    {
        buttonAudioSource.clip = buttonAudioClips[Random.Range(0, buttonAudioClips.Count)];
        buttonAudioSource.Play();
    }

    /// <summary>
    /// Loads and plays a random explosion sound
    /// </summary>
    public void PlayExplosionSound()
    {
        explosionAudioSource.clip = explotionAudioClips[Random.Range(0, explotionAudioClips.Count)];
        explosionAudioSource.PlayDelayed(0.4f);
    }

    /// <summary>
    /// Loads and plays a random fire sound
    /// </summary>
    public void PlayFireSound()
    {
        fireAudioSource.clip = fireAudioClips[Random.Range(0, fireAudioClips.Count)];
        fireAudioSource.Play();
    }

    /// <summary>
    /// Loads and plays a random water sound
    /// </summary>
    public void PlayWaterSound()
    {
        waterAudioSource.clip = waterAudioClips[Random.Range(0, waterAudioClips.Count)];
        waterAudioSource.PlayDelayed(0.4f);
    }

    /// <summary>
    /// Loads and plays a random sinking sound
    /// </summary>
    public void PlaySinkingSound()
    {
        sinkingAudioSource.clip = sinkingAudioClips[Random.Range(0, sinkingAudioClips.Count)];
        sinkingAudioSource.PlayDelayed(1.0f);
    }

    /// <summary>
    /// Loads and plays a random victory sound
    /// </summary>
    public void PlayVictorySound()
    {
        victoryAudioSource.Play();
    }
}
