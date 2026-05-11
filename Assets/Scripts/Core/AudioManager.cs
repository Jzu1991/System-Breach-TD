using UnityEngine;

namespace SystemBreach.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music")]
        public AudioClip mainMenuMusic;
        public AudioClip gameplayMusic;
        public AudioClip bossMusic;
        public AudioClip victoryMusic;
        public AudioClip defeatMusic;

        [Header("SFX")]
        public AudioClip towerPlace;
        public AudioClip towerUpgrade;
        public AudioClip towerSell;
        public AudioClip enemyDeath;
        public AudioClip bossArrival;
        public AudioClip pcDamage;      // BSOD flash feedback
        public AudioClip waveStart;
        public AudioClip victory;
        public AudioClip defeat;

        private float musicVolume = 1f;
        private float sfxVolume   = 1f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            GameManager.OnVictory     += OnVictory;
            GameManager.OnGameOver    += OnGameOver;
            WaveManager.OnWaveStarted += OnWaveStarted;
        }

        void OnDisable()
        {
            GameManager.OnVictory     -= OnVictory;
            GameManager.OnGameOver    -= OnGameOver;
            WaveManager.OnWaveStarted -= OnWaveStarted;
        }

        private void OnVictory()                   => PlayMusic(victoryMusic);
        private void OnGameOver()                  => PlayMusic(defeatMusic);
        private void OnWaveStarted(int _, int __)  => PlaySFX(waveStart);

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || musicSource == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip   = clip;
            musicSource.loop   = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetMusicVolume(float v)
        {
            musicVolume = Mathf.Clamp01(v);
            if (musicSource != null) musicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float v)
        {
            sfxVolume = Mathf.Clamp01(v);
        }

        public float MusicVolume => musicVolume;
        public float SFXVolume   => sfxVolume;
    }
}
