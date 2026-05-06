using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemBreach.Core;

namespace SystemBreach.UI
{
    // Options panel: music volume, SFX volume, auto-skip wave toggle.
    public class OptionsUI : MonoBehaviour
    {
        [SerializeField] private Slider   musicSlider;
        [SerializeField] private Slider   sfxSlider;
        [SerializeField] private Toggle   autoSkipToggle;
        [SerializeField] private Button   mainMenuButton;

        void Start()
        {
            if (AudioManager.Instance != null)
            {
                if (musicSlider != null)
                {
                    musicSlider.value = AudioManager.Instance.MusicVolume;
                    musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetMusicVolume(v));
                }
                if (sfxSlider != null)
                {
                    sfxSlider.value = AudioManager.Instance.SFXVolume;
                    sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetSFXVolume(v));
                }
            }

            if (autoSkipToggle != null && WaveManager.Instance != null)
            {
                autoSkipToggle.isOn = WaveManager.Instance.AutoSkipWave;
                autoSkipToggle.onValueChanged.AddListener(v => WaveManager.Instance.AutoSkipWave = v);
            }

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
        }
    }
}
