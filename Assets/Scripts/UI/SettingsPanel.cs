using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{
    public class SettingsPanel : PanelBase
    {
        [Header("滑条")]
        public Slider bgmSlider;   // 音乐音量
        public Slider sfxSlider;   // 音效音量

        [Header("文本")]
        public TextMeshProUGUI bgmValueText;
        public TextMeshProUGUI sfxValueText;

        [Header("关闭")]
        public Button closeButton;

        private bool _loading; 

        public override void OnShow(System.Action onClosed = null)
        {
            base.OnShow(onClosed);
            Refresh();
        }

        public override void Refresh()
        {
            if (SaveManager.Instance == null) return;
            var d = SaveManager.Instance.Data;
            _loading = true;
            if (bgmSlider != null) 
            {
                bgmSlider.value = d.bgmVolume;
            }
            if (sfxSlider != null) 
            {
                sfxSlider.value = d.sfxVolume;
            } 
            _loading = false;
            RefreshLabels();
        }

        private void Start()
        {
            if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(OnBgmChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            if (closeButton != null) closeButton.onClick.AddListener(OnClose);
        }

        private void OnBgmChanged(float v)
        {
            if (_loading || SaveManager.Instance == null) return;
            SaveManager.Instance.Data.bgmVolume = v;
            AudioManager.Instance?.SetVolumes(v, SaveManager.Instance.Data.sfxVolume);
            SaveManager.Instance.Save();
            RefreshLabels();
        }

        private void OnSfxChanged(float v)
        {
            if (_loading || SaveManager.Instance == null) return;
            SaveManager.Instance.Data.sfxVolume = v;
            AudioManager.Instance?.SetVolumes(SaveManager.Instance.Data.bgmVolume, v);
            SaveManager.Instance.Save();
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            var d = SaveManager.Instance.Data;
            if (bgmValueText != null)
            {
                bgmValueText.text = Mathf.RoundToInt(d.bgmVolume * 100f) + "%";
            }
            if (sfxValueText != null)
            {
                sfxValueText.text = Mathf.RoundToInt(d.sfxVolume * 100f) + "%";
            }
        }

        private void OnClose()
        {
            UIManager.Instance.Hide("SettingsPanel");
        }
    }
}
