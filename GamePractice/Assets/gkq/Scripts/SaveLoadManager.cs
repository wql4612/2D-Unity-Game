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
                    Debug.Log("ȷ��game_saves����ڳɹ�");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ʧ��: {e.Message}");
        }
    }

    public void SaveGame()
    {
        // �����ǰѡ�ж���
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("��ʼ������Ϸ...");
        // 1. ���л����ж���
        Dictionary<string, Dictionary<string, string>> gameData = UniversalSerializer.SerializeAllObjects();

        if (gameData == null)
        {
            Debug.LogError("gameData �� null!");
            return;
        }
        else if (gameData.Count == 0)
        {
            Debug.LogWarning("gameData �ǿյ��ֵ䣡");
            Debug.Log("���ö�ջ:\n" + Environment.StackTrace);
            return;
        }

        Debug.Log($"���л�����: {gameData.Count} ������");
        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        Debug.Log($"���л�JSON: {jsonData}");

        // ���浽���ݿ�
        SaveToDatabase(jsonData, "default_save");

        // ͬʱ���������ļ����棨��ѡ��
        string path = Application.persistentDataPath + "/savefile.json";
        SaveToJsonFile(jsonData, path);
    }

    public void LoadGame()
    {
        // ���ȴ����ݿ����
        string jsonData = LoadFromDatabase("default_save");

        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("���Դӱ����ļ�����...");
            // ������ݿ�û�У����Դ��ļ�����
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");
            if (File.Exists(path))
            {
                jsonData = File.ReadAllText(path);
            }
            else
            {
                Debug.LogWarning("û���ҵ��κδ浵��");
                return;
            }
        }

        try
        {
            var gameData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            if (gameData == null || gameData.Count == 0)
            {
                Debug.LogWarning("�����л�����Ϊ�գ�");
                return;
            }

            UniversalDeserializer.DeserializeAllObjects(gameData);
            Debug.Log("��Ϸ���سɹ������ض�������: " + gameData.Count);
        }
        catch (Exception ex)
        {
            Debug.LogError($"����ʧ��: {ex.GetType()} - {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void SaveToDatabase(string jsonData, string saveName = "default")
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // ʹ��REPLACE INTO���򻯲��������Զ�����������£�
                string query = @"
                REPLACE INTO game_saves (save_name, json_data, created_at, last_updated) 
                VALUES (@saveName, @jsonData, NOW(), NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@saveName", saveName);
                    cmd.Parameters.AddWithValue("@jsonData", jsonData);
                    cmd.ExecuteNonQuery();
                }

                Debug.Log($"��Ϸ�ѱ��浽���ݿ⣬�浵����: {saveName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���浽���ݿ�ʧ��: {e.Message}");
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
                            Debug.Log($"�����ݿ���ش浵�ɹ�: {saveName}");
                            return jsonData;
                        }
                        else
                        {
                            Debug.LogWarning($"δ�ҵ��浵: {saveName}");
                            return null;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�����ݿ����ʧ��: {e.Message}");
            return null;
        }
    }

    // ���������ļ����淽������ѡ��
    private void SaveToJsonFile(string jsonData, string customPath = null)
    {
        string filePath = customPath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

        try
        {
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"��Ϸ�ѱ��浽�ļ�: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�����ļ�ʧ��: {e.Message}");
        }
    }
}