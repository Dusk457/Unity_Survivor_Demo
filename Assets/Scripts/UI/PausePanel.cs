using UnityEngine;
using UnityEngine.UI;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{

    public class PausePanel : PanelBase
    {
        public Button resumeButton;
        public Button quitButton;

        private void Start()
        {
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResume);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuit);
        }

        private void OnResume()
        {
            GameManager.Instance.ResumeGame();
        }

        private void OnQuit()
        {
            GameManager.Instance.SetStateFromUI(E_GameState.MainMenu);
        }
    }
}
