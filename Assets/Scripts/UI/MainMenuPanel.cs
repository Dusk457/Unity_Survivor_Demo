using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{
    public class MainMenuPanel : PanelBase
    {
        public Button startButton;
        public Button quitButton;
        public Button settingsButton;
        public TextMeshProUGUI bestScoreText;

        public override void OnShow(System.Action onClosed = null)
        {
            base.OnShow(onClosed);
            if (bestScoreText != null)
                bestScoreText.text = "最高分 " + SaveManager.Instance.Data.bestScore;
        }

        private void Start()
        {
            if (startButton != null) startButton.onClick.AddListener(OnStart);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuit);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettings);
        }

        private void OnSettings()
        {
            UIManager.Instance.Show("SettingsPanel");
        }

        private void OnStart()
        {
            GameManager.Instance.StartGame();
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
