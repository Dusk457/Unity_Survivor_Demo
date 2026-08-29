using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorDemo.Managers
{
    /// <summary>
    /// EventManager：事件总线
    /// </summary>
    public sealed class EventManager : MonoBehaviour
    {
        public static EventManager Instance 
        { 
            get; 
            private set; 
        }

        private readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            { 
                Destroy(gameObject); 
                return; 
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // 订阅
        public void On<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
                _events[type] = Delegate.Combine(existing, handler);
            else
                _events[type] = handler;
        }

        // 取消订阅
        public void Off<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var existing))
            {
                var newDelegate = Delegate.Remove(existing, handler);
                if (newDelegate == null) _events.Remove(type);
                else _events[type] = newDelegate;
            }
        }

        // 发布
        public void Emit<T>(T evt)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var d))
                (d as Action<T>)?.Invoke(evt);
        }

        // 清空（切场景/重开）
        public void Clear()
        {
            _events.Clear();
        }
    }

    //定义具体事件
    public readonly struct GameStateChangedEvent { 
        public readonly E_GameState State;
        public GameStateChangedEvent(E_GameState s) => State = s;
    }
    public readonly struct PlayerHpChangedEvent { 
        public readonly float Current; 
        public readonly float Max;
        public PlayerHpChangedEvent(float c, float m) 
        { 
            Current = c; 
            Max = m; 
        } 
    }
    public readonly struct PlayerScoreChangedEvent { 
        public readonly int Score; 
        public PlayerScoreChangedEvent(int s) => Score = s;
    }
    public readonly struct PlayerLevelUpEvent { 
        public readonly int Level; 
        public PlayerLevelUpEvent(int l) => Level = l; 
    }
    public readonly struct EnemyKilledEvent { 
        public readonly int Score;
        public EnemyKilledEvent(int s) => Score = s;
    }
    public readonly struct WaveStartedEvent { 
        public readonly int WaveIndex; 
        public WaveStartedEvent(int i) => WaveIndex = i; 
    }
    public readonly struct WeaponChangedEvent { 
        public readonly string WeaponId; 
        public WeaponChangedEvent(string id) => WeaponId = id; 
    }
}
