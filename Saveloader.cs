using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
    // 保存游戏数据到PlayerPrefs (临时方案，实际项目中应该存到数据库)
    public void SaveGame()
    {
        Debug.Log("开始保存游戏...");
        // 1. 序列化所有对象
        Dictionary<string, Dictionary<string, string>> gameData = UniversalSerializer.SerializeAllObjects();
        // 详细检查 gameData
        if (gameData == null)
        {
            Debug.LogError("gameData 是 null!");
        }
        else if (gameData.Count == 0)
        {
            Debug.LogWarning("gameData 是空的字典！");

            // 打印调用堆栈以帮助调试
            Debug.Log("调用堆栈:\n" + Environment.StackTrace);
        }
        else
        {
            Debug.Log($"序列化数据: {gameData.Count} 个对象");

            // 打印前几个条目（避免日志过大）
            int maxToShow = 5;
            int shown = 0;
            foreach (var entry in gameData)
            {
                Debug.Log($"Key: {entry.Key}, ValueCount: {entry.Value?.Count ?? 0}");
                shown++;
                if (shown >= maxToShow) break;
            }
        }



        string jsonData = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        Debug.Log($"序列化JSON: {jsonData}"); // 现在会正确输出


        //// 2. 转换为JSON字符串
        //var wrapper = new SerializationWrapper(gameData);
        //Debug.Log($"Wrapper 数据量: {wrapper.data?.Count ?? 0}");
        //// 3. 序列化为 JSON
        //string jsonData = JsonUtility.ToJson(wrapper, prettyPrint: true);
        //Debug.Log($"序列化JSON: {jsonData}");  // 现在应该能正确输出
        //// 3. 保存到PlayerPrefs (实际项目中应该存到数据库)
        //PlayerPrefs.SetString("SavedGame", jsonData);
        //PlayerPrefs.Save();

        string path = Application.persistentDataPath + "/savefile.json";
        Debug.Log($"保存路径: {path}");
        SaveToJsonFile(jsonData, path);
        Debug.Log("游戏已保存！");
    }

    // 从PlayerPrefs加载游戏数据
    public void LoadGame()
    {
        try
        {
            // 1. 构建路径（跨平台兼容写法）
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");
            Debug.Log($"尝试从路径加载: {path}");

            // 2. 检查文件是否存在
            if (!File.Exists(path))
            {
                Debug.LogWarning("存档文件不存在，路径: " + path);
                return;
            }

            // 3. 读取文件内容
            string jsonData = File.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogError("存档文件为空！");
                return;
            }

            // 4. 反序列化（使用 Newtonsoft.Json）
            var gameData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

            // 5. 验证数据
            if (gameData == null || gameData.Count == 0)
            {
                Debug.LogWarning("反序列化数据为空！");
                return;
            }

            // 6. 恢复游戏状态
            UniversalDeserializer.DeserializeAllObjects(gameData);
            Debug.Log("游戏加载成功！加载对象数量: " + gameData.Count);
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载失败: {ex.GetType()} - {ex.Message}\n{ex.StackTrace}");
        }
    }

    // 新增方法：保存到JSON文件
    private void SaveToJsonFile(string jsonData, string customPath = null)
    {
        // 如果没有指定路径，使用默认路径
        string filePath = customPath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

        try
        {
            // 写入文件
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"游戏已保存到文件: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存文件失败: {e.Message}");
        }
    }

    // 文件加载游戏数据
    public void LoadGameFromJson(bool fromFile = true, string filePath = null)
    {
        string jsonData;

        if (fromFile)
        {
            // 从文件加载
            string path = filePath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"存档文件不存在: {path}");
                return;
            }

            try
            {
                jsonData = File.ReadAllText(path);
                Debug.Log($"从文件加载存档: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取存档文件失败: {e.Message}");
                return;
            }
        }
        else
        {
            // 从PlayerPrefs加载
            if (!PlayerPrefs.HasKey("SavedGame"))
            {
                Debug.LogWarning("没有找到PlayerPrefs存档！");
                return;
            }

            jsonData = PlayerPrefs.GetString("SavedGame");
            Debug.Log("从PlayerPrefs加载存档");
        }

        // 反序列化
        SerializationWrapper wrapper = JsonUtility.FromJson<SerializationWrapper>(jsonData);

        // 恢复游戏状态
        UniversalDeserializer.DeserializeAllObjects(wrapper.data);

        Debug.Log("游戏已加载！");
    }

    // 辅助类，用于包装Dictionary以便JsonUtility能正确序列化
    [System.Serializable]
    private class SerializationWrapper
    {
        public Dictionary<string, Dictionary<string, string>> data;

        public SerializationWrapper(Dictionary<string, Dictionary<string, string>> data)
        {
            this.data = data;
        }
    }

}