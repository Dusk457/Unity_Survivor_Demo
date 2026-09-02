using System.Collections.Generic;
using UnityEngine;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance 
        { 
            get; 
            private set; 
        }

        [SerializeField] 
        private PanelBase[] panels;

        private readonly Dictionary<string, PanelBase> _panelMap = new Dictionary<string, PanelBase>();
        private readonly Stack<PanelBase> _stack = new Stack<PanelBase>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (panels != null)
                foreach (var p in panels)
                {
                    if (p != null && !_panelMap.ContainsKey(p.name))
                        _panelMap.Add(p.name, p);
                }
        }

        private void Start()
        {
            EventManager.Instance?.On<GameStateChangedEvent>(OnGameStateChanged);
            if (GameManager.Instance != null)
                OnGameStateChanged(new GameStateChangedEvent(GameManager.Instance.State));
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Off<GameStateChangedEvent>(OnGameStateChanged);
        }

        public void Show(string panelName, System.Action onClosed = null)
        {
            if (_panelMap.TryGetValue(panelName, out var panel))
            {
                panel.OnShow(onClosed);
                _stack.Push(panel);
            }
            else Debug.LogWarning($"[UIManager] 未注册面板 {panelName}");
        }

        public void Hide(string panelName)
        {
            if (_panelMap.TryGetValue(panelName, out var panel))
                panel.OnHide();
        }

        public void HideTop()
        {
            if (_stack.Count > 0) _stack.Pop()?.OnHide();
        }

        public void HideAll()
        {
            foreach (var p in _panelMap.Values) p?.OnHide();
            _stack.Clear();
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            switch (e.State)
            {
                case E_GameState.MainMenu:
                    HideAll();
                    Show("MainMenuPanel");
                    break;
                case E_GameState.Playing:
                    HideAll();
                    Show("HUDController");
                    break;
                case E_GameState.Paused:
                    Show("PausePanel");
                    break;
                case E_GameState.GameOver:
                    Hide("HUDController");
                    Show("GameOverPanel");
                    break;
            }
        }
    }
}
