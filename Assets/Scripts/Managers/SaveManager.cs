using System.IO;
using UnityEngine;
using SurvivorDemo.Data;

namespace SurvivorDemo.Managers
{
    /// <summary>
    /// SaveManager:JSON持久化到本地文件
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance 
        { 
            get; 
            private set;
        }


        private string SavePath => Path.Combine(Application.persistentDataPath, "survivor_save.json");

        public SaveData Data 
        { 
            get; 
            private set; 
        } = new SaveData();

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

        // 读取存档
        public void Load()
        {
            if (!File.Exists(SavePath))
            {
                Data = new SaveData();
                Debug.Log($"[SaveManager] 首次运行，无存档，使用默认。");
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                Debug.Log($"[SaveManager] 已读取存档: {SavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] 读取/解析存档失败: {e.Message}");
                Data = new SaveData();
            }
        }

        // 保存存档
        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[SaveManager] 已保存到: {SavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] 保存失败: {e.Message}");
            }
        }

        // 更新最佳成绩
        public void UpdateBest(int score, int kills)
        {
            if (score > Data.bestScore) Data.bestScore = score;
            Data.totalKills += kills;
            Data.playTime += GameManager.Instance.CurrentTime;
            Save();
        }
    }
}
