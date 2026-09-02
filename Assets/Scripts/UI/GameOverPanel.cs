using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{
    public class GameOverPanel : PanelBase
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI killsText;
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI bestText;
        public Button restartButton;
        public Button menuButton;

        public override void OnShow(System.Action onClosed = null)
        {
            base.OnShow(onClosed);

            var gm = GameManager.Instance;
            if (scoreText != null) scoreText.text = "本局得分 " + gm.Score;
            if (killsText != null) killsText.text = "击杀 " + gm.Kills;
            if (timeText != null) timeText.text = "存活 " + Mathf.FloorToInt(gm.CurrentTime) + "s";
            if (bestText != null) bestText.text = "最高分 " + SaveManager.Instance.Data.bestScore;

            AudioManager.Instance?.PlayGameOver();
        }

        private void Start()
        {
            if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
            if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
        }

        private void OnRestart()
        {
            GameManager.Instance.StartGame();
        }

        private void OnMenu()
        {
            GameManager.Instance.SetStateFromUI(E_GameState.MainMenu);
        }
    }
}
