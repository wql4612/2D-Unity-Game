using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using MySql.Data.MySqlClient;

public class SaveLoadManager : MonoBehaviour
{
    public string connectionString = string.Format("server={0};port={1};database={2};user={3};password={4};",
        "localhost", 3306, "demo", "root", "123456");

    private void Start()
    {
        EnsureTableExists();
    }

    private void EnsureTableExists()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS game_saves (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    save_name VARCHAR(255) NOT NULL,
                    json_data LONGTEXT NOT NULL,
                    created_at DATETIME NOT NULL,
                    last_updated DATETIME NOT NULL,
                    UNIQUE KEY (save_name)
                )";

                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                    Debug.Log("确保game_saves表存在成功");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"创建表失败: {e.Message}");
        }
    }

    public void SaveGame()
    {
        // 清除当前选中对象
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("开始保存游戏...");
        // 1. 序列化所有对象
        Dictionary<string, Dictionary<string, string>> gameData = UniversalSerializer.SerializeAllObjects();

        if (gameData == null)
        {
            Debug.LogError("gameData 是 null!");
            return;
        }
        else if (gameData.Count == 0)
        {
            Debug.LogWarning("gameData 是空的字典！");
            Debug.Log("调用堆栈:\n" + Environment.StackTrace);
            return;
        }

        Debug.Log($"序列化数据: {gameData.Count} 个对象");
        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        Debug.Log($"序列化JSON: {jsonData}");

        // 保存到数据库
        SaveToDatabase(jsonData, "default_save");

        // 同时保留本地文件保存（可选）
        string path = Application.persistentDataPath + "/savefile.json";
        SaveToJsonFile(jsonData, path);
    }

    public void LoadGame()
    {
        // 优先从数据库加载
        string jsonData = LoadFromDatabase("default_save");

        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("尝试从本地文件加载...");
            // 如果数据库没有，尝试从文件加载
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");
            if (File.Exists(path))
            {
                jsonData = File.ReadAllText(path);
            }
            else
            {
                Debug.LogWarning("没有找到任何存档！");
                return;
            }
        }

        try
        {
            var gameData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            if (gameData == null || gameData.Count == 0)
            {
                Debug.LogWarning("反序列化数据为空！");
                return;
            }

            UniversalDeserializer.DeserializeAllObjects(gameData);
            Debug.Log("游戏加载成功！加载对象数量: " + gameData.Count);
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载失败: {ex.GetType()} - {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void SaveToDatabase(string jsonData, string saveName = "default")
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 使用REPLACE INTO语句简化操作（会自动处理插入或更新）
                string query = @"
                REPLACE INTO game_saves (save_name, json_data, created_at, last_updated) 
                VALUES (@saveName, @jsonData, NOW(), NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@saveName", saveName);
                    cmd.Parameters.AddWithValue("@jsonData", jsonData);
                    cmd.ExecuteNonQuery();
                }

                Debug.Log($"游戏已保存到数据库，存档名称: {saveName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存到数据库失败: {e.Message}");
        }
    }

    private string LoadFromDatabase(string saveName = "default")
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT json_data FROM game_saves WHERE save_name = @saveName";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@saveName", saveName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string jsonData = reader["json_data"].ToString();
                            Debug.Log($"从数据库加载存档成功: {saveName}");
                            return jsonData;
                        }
                        else
                        {
                            Debug.LogWarning($"未找到存档: {saveName}");
                            return null;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"从数据库加载失败: {e.Message}");
            return null;
        }
    }

    // 保留本地文件保存方法（可选）
    private void SaveToJsonFile(string jsonData, string customPath = null)
    {
        string filePath = customPath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

        try
        {
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"游戏已保存到文件: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存文件失败: {e.Message}");
        }
    }
}