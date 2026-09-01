using System.Collections.Generic;
using UnityEngine;

namespace SurvivorDemo.Data
{
    public static class ConfigLoader
    {
        private static readonly string ConfigPath = "Config/";

        private static Dictionary<string, WeaponConfig> _weaponMap;
        private static Dictionary<string, EnemyConfig> _enemyMap;
        private static LevelConfig _level;

        public static System.Action OnConfigLoaded;

        public static void LoadAll()
        {
            LoadWeapons();
            LoadEnemies();
            LoadLevel();
            OnConfigLoaded?.Invoke();
        }

        private static void LoadWeapons()
        {
            var list = LoadJson<WeaponConfigList>("weapons");
            _weaponMap = new Dictionary<string, WeaponConfig>();
            if (list != null)
            {
                foreach (var w in list.weapons)
                {
                    if (!_weaponMap.ContainsKey(w.id)) _weaponMap.Add(w.id, w);
                }                   
            }
                
        }

        private static void LoadEnemies()
        {
            var list = LoadJson<EnemyConfigList>("enemies");
            _enemyMap = new Dictionary<string, EnemyConfig>();
            if (list != null)
            {
                foreach (var e in list.enemies)
                {
                    if (!_enemyMap.ContainsKey(e.id)) _enemyMap.Add(e.id, e);
                }
            }                    
        }

        private static void LoadLevel()
        {
            _level = LoadJson<LevelConfig>("level");
        }

        private static T LoadJson<T>(string fileName) where T : class
        {
            TextAsset asset = Resources.Load<TextAsset>(ConfigPath + fileName);
            if (asset == null)
            {
                Debug.LogWarning($"[ConfigLoader] 找不到配置文件 {ConfigPath}{fileName}");
                return null;
            }
            try
            {
                T data = JsonUtility.FromJson<T>(asset.text);
                Debug.Log($"[ConfigLoader] 已加载 {fileName} -> {typeof(T).Name}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ConfigLoader] 解析 {fileName} 失败: {e.Message}");
                return null;
            }
        }

        //对外查询接口
        public static WeaponConfig GetWeapon(string id) =>
            _weaponMap != null && _weaponMap.TryGetValue(id, out var w) ? w : null;

        public static EnemyConfig GetEnemy(string id) =>
            _enemyMap != null && _enemyMap.TryGetValue(id, out var e) ? e : null;

        public static LevelConfig GetLevel() => _level;

        public static IReadOnlyCollection<WeaponConfig> AllWeapons =>
            _weaponMap?.Values;

        public static IReadOnlyCollection<EnemyConfig> AllEnemies =>
            _enemyMap?.Values;
    }
}
