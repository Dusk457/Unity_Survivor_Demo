using System.Collections.Generic;
using System;

namespace SurvivorDemo.Data
{
    [Serializable]
    public class WeaponConfig
    {
        public string id;
        public string name;
        public float damage;          // 单发伤害
        public float fireRate;        // 每秒发射次数
        public float projectileSpeed; // 弹道速度
        public float range;           // 射程
        public int count = 1;         // 同时发射数量
        public float spreadAngle;     // 散射角度
        public string spritePath;     // 弹体贴图资源路径
    }

    [Serializable]
    public class WeaponConfigList
    {
        public List<WeaponConfig> weapons = new List<WeaponConfig>();
    }

    [Serializable]
    public class EnemyConfig
    {
        public string id;
        public string name;
        public float maxHp;
        public float moveSpeed;
        public float damage;
        public int score;
        public string spritePath;
    }

    [Serializable]
    public class EnemyConfigList
    {
        public List<EnemyConfig> enemies = new List<EnemyConfig>();
    }

    [Serializable]
    public class WaveConfig
    {
        public string enemyId;
        public int count;             // 本波数量
        public float spawnInterval;   // 单个生成间隔
        public float startDelay;      // 波次开始延迟
    }

    [Serializable]
    public class LevelConfig
    {
        public int levelId;
        public string levelName;
        public float duration;        // 关卡时长
        public float waveInterval;    // 波次间隔
        public List<WaveConfig> waves = new List<WaveConfig>();
    }

    // 存档数据
    [Serializable]
    public class SaveData
    {
        public int bestScore;
        public int totalKills;
        public float playTime;
        public string playerName = "无名勇者";
        public float bgmVolume = 1f;   // 音乐音量（0~1）
        public float sfxVolume = 1f;   // 音效音量（0~1）
    }
}
