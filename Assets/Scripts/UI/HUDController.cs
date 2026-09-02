using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurvivorDemo.Managers;
using SurvivorDemo.Gameplay;

namespace SurvivorDemo.UI
{
    public class HUDController : PanelBase
    {
        [Header("引用")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI timeText;
        public Image hpBarFill;          
        public TextMeshProUGUI weaponText;

        private void OnEnable()
        {
            EventManager.Instance.On<PlayerScoreChangedEvent>(OnScore);
            EventManager.Instance.On<PlayerHpChangedEvent>(OnHp);
            EventManager.Instance.On<WeaponChangedEvent>(OnWeapon);

            var pc = FindObjectOfType<PlayerController>();
            if (pc != null)
                OnHp(new PlayerHpChangedEvent(pc.CurrentHp, pc.maxHp));
        }

        private void OnDisable()
        {
            if (EventManager.Instance == null) return;
            EventManager.Instance.Off<PlayerScoreChangedEvent>(OnScore);
            EventManager.Instance.Off<PlayerHpChangedEvent>(OnHp);
            EventManager.Instance.Off<WeaponChangedEvent>(OnWeapon);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var gm = GameManager.Instance;
                if (gm != null)
                {
                    if (gm.State == E_GameState.Playing) gm.PauseGame();
                    else if (gm.State == E_GameState.Paused) gm.ResumeGame();
                }
            }

            if (timeText != null && GameManager.Instance.State == E_GameState.Playing)
                timeText.text = "时间 " + Mathf.FloorToInt(GameManager.Instance.CurrentTime).ToString("00") + "s";
        }

        private void OnScore(PlayerScoreChangedEvent e)
        {
            if (scoreText != null) scoreText.text = "得分 " + e.Score;
        }

        private void OnHp(PlayerHpChangedEvent e)
        {
            if (hpText != null) hpText.text = "HP " + Mathf.CeilToInt(e.Current) + " / " + Mathf.CeilToInt(e.Max);
            if (hpBarFill != null)
            {
                if (hpBarFill.type != Image.Type.Filled)
                {
                    hpBarFill.type = Image.Type.Filled;
                    hpBarFill.fillMethod = Image.FillMethod.Horizontal;
                    hpBarFill.fillOrigin = 0; 
                }
                hpBarFill.fillAmount = e.Max > 0 ? e.Current / e.Max : 0f;
            }
        }

        private void OnWeapon(WeaponChangedEvent e)
        {
            if (weaponText != null) weaponText.text = "武器 " + e.WeaponId;
        }
    }
}
