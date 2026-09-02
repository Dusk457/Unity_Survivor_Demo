using UnityEngine;
using SurvivorDemo.Data;
using SurvivorDemo.Gameplay;

namespace SurvivorDemo.Managers
{
    // 游戏状态枚举
    public enum E_GameState 
    { 
        Boot, 
        MainMenu, 
        Playing, 
        Paused, 
        GameOver 
    }
    /// <summary>
    /// GameManager
    /// 单例 Singleton：全局唯一入口
    /// 状态机 E_GameState：控制游戏主流程，切换时通过事件广播
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance 
        { 
            get;
            private set;
        }

        public E_GameState State { get; private set; } = E_GameState.Boot;

        public int Score 
        { 
            get; 
            private set; 
        }
        public int Kills 
        { 
            get; 
            private set; 
        }
        public float CurrentTime { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 1.先加载 JSON 配置
            ConfigLoader.LoadAll();

            // 2.读取存档
            SaveManager.Instance.Load();
        }

        private void Start()
        {
            SetState(E_GameState.MainMenu);
        }

        public void StartGame()
        {
            ResetSession();
            // 重置玩家状态
            var pc = FindObjectOfType<PlayerController>();
            if (pc != null) pc.ResetPlayer();
            SetState(E_GameState.Playing);
        }

        public void PauseGame()
        {
            if (State == E_GameState.Playing)
            {
                Time.timeScale = 0f;
                SetState(E_GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (State == E_GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(E_GameState.Playing);
            }
        }

        public void GameOver()
        {
            Time.timeScale = 1f;
            SaveManager.Instance.UpdateBest(Score, Kills);
            SetState(E_GameState.GameOver);
        }

        private void ResetSession()
        {
            Score = 0;
            Kills = 0;
            CurrentTime = 0;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (State == E_GameState.Playing)
            {
                CurrentTime += Time.deltaTime;
            }
        }
        public void AddScore(int v)
        {
            Score += v;
            EventManager.Instance.Emit(new PlayerScoreChangedEvent(Score));
        }

        public void AddKill()
        {
            Kills++;
        }

        private void SetState(E_GameState newState)
        {
            State = newState;
            EventManager.Instance?.Emit(new GameStateChangedEvent(newState));
        }

        public void SetStateFromUI(E_GameState newState)
        {
            Time.timeScale = 1f;
            if (newState == E_GameState.MainMenu || newState == E_GameState.Playing)
                ResetSession();
            SetState(newState);
        }
    }
}
