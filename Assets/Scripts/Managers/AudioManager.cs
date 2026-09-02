using UnityEngine;
using SurvivorDemo.Data;

namespace SurvivorDemo.Managers
{
    /// <summary>
    /// AudioManager：音频管理类
    /// 集中管理音频，避免每个对象各自放 AudioSource
    /// 通过事件或直接调用播放 BGM
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance 
        { 
            get; 
            private set; 
        }

        [Header("Audio Clips")]
        public AudioClip bgm;
        public AudioClip shootSfx;
        public AudioClip hitSfx;
        public AudioClip gameOverSfx;

        private AudioSource _bgmSource;
        private AudioSource _sfxSource;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            ApplyVolume();
            PlayBgm();
        }

        public void SetVolumes(float bgmVol, float sfxVol)
        {
            if (_bgmSource != null) _bgmSource.volume = Mathf.Clamp01(bgmVol);
            if (_sfxSource != null) _sfxSource.volume = Mathf.Clamp01(sfxVol);
        }

        private void ApplyVolume()
        {
            if (SaveManager.Instance != null)
                SetVolumes(SaveManager.Instance.Data.bgmVolume, SaveManager.Instance.Data.sfxVolume);
        }

        public void PlayBgm()
        {
            if (bgm != null)
            {
                _bgmSource.clip = bgm;
                _bgmSource.Play();
            }
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayShoot() => PlaySfx(shootSfx);
        public void PlayHit() => PlaySfx(hitSfx);
        public void PlayGameOver() => PlaySfx(gameOverSfx);
    }
}
