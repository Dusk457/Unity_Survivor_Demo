using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurvivorDemo.Managers;
using SurvivorDemo.Data;
using SurvivorDemo.Utils;

namespace SurvivorDemo.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("生成范围")]
        public float spawnRadius = 12f;

        [Header("随机刷怪")]
        public float minInterval = 1f;                  
        public float maxInterval = 3f;                  
        [Range(0f, 1f)] public float batChance = 0.3f;  
        public float bossStartDelay = 30f;              

        private Transform _player;
        private Dictionary<string, EnemyConfig> _enemyConfigs;

        private ObjectPool<EnemyBase> _slimePool;
        private ObjectPool<EnemyBase> _batPool;
        private ObjectPool<EnemyBase> _bossPool;
        private bool _spawning;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Start()
        {
            EventManager.Instance?.On<GameStateChangedEvent>(OnGameStateChanged);
            BuildPools();
            if (GameManager.Instance != null && GameManager.Instance.State == E_GameState.Playing)
                StartLevel();
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            switch (e.State)
            {
                case E_GameState.Playing:
                    StartLevel();
                    break;
                case E_GameState.Paused:
                    break;
                default: 
                    StopAndClear();
                    break;
            }
        }

        public void StartLevel()
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player")?.transform;
                Debug.LogWarning("[Spawner] 找不到 Player");
                return;
            }

            if (_spawning) return;

            ClearPools();
            _spawning = true;

            StartCoroutine(SpawnLoop());
            StartCoroutine(SpawnBoss());
        }

        private void BuildPools()
        {
            _enemyConfigs = new Dictionary<string, EnemyConfig>();
            foreach (var cfg in ConfigLoader.AllEnemies)
                _enemyConfigs[cfg.id] = cfg;

            _slimePool = MakePool("slime");
            _batPool = MakePool("bat");
            _bossPool = MakePool("boss");
        }

        private ObjectPool<EnemyBase> MakePool(string id)
        {
            if (!_enemyConfigs.ContainsKey(id)) return null;
            GameObject go = Resources.Load<GameObject>("Prefabs/" + id);
            EnemyBase pfb = go != null ? go.GetComponent<EnemyBase>() : null;
            if (pfb == null) 
            { 
                Debug.LogWarning($"[Spawner] 敌人 {id} 无预制体"); 
                return null; 
            }
            return new ObjectPool<EnemyBase>(pfb, 5, transform);
        }

        private IEnumerator SpawnLoop()
        {
            while (_spawning)
            {
                bool isBat = Random.value < batChance;
                ObjectPool<EnemyBase> pool = isBat ? _batPool : _slimePool;
                string id = isBat ? "bat" : "slime";

                if (pool != null && _enemyConfigs.TryGetValue(id, out var cfg))
                    SpawnOne(pool, cfg);

                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            }
        }

        private IEnumerator SpawnBoss()
        {
            yield return new WaitForSeconds(bossStartDelay);
            if (_bossPool != null && _enemyConfigs.TryGetValue("boss", out var cfg))
                SpawnOne(_bossPool, cfg);
        }

        private void SpawnOne(ObjectPool<EnemyBase> pool, EnemyConfig cfg)
        {
            Vector2 pos = (Vector2)_player.position + Random.insideUnitCircle.normalized * spawnRadius;
            EnemyBase enemy = pool.Spawn(pos, Quaternion.identity);
            enemy.Init(cfg, _player);
            enemy.SetPool(pool);
        }

        private void ClearPools()
        {
            _slimePool?.DespawnAll();
            _batPool?.DespawnAll();
            _bossPool?.DespawnAll();
        }

        private void StopAndClear()
        {
            _spawning = false;
            StopAllCoroutines();
            ClearPools();
        }

        private void OnDestroy()
        {
            _spawning = false;
            StopAllCoroutines();
            if (EventManager.Instance != null)
                EventManager.Instance.Off<GameStateChangedEvent>(OnGameStateChanged);
        }
    }
}
