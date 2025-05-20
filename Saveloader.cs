using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
    // ������Ϸ���ݵ�PlayerPrefs (��ʱ������ʵ����Ŀ��Ӧ�ô浽���ݿ�)
    public void SaveGame()
    {
        Debug.Log("��ʼ������Ϸ...");
        // 1. ���л����ж���
        Dictionary<string, Dictionary<string, string>> gameData = UniversalSerializer.SerializeAllObjects();
        // ��ϸ��� gameData
        if (gameData == null)
        {
            Debug.LogError("gameData �� null!");
        }
        else if (gameData.Count == 0)
        {
            Debug.LogWarning("gameData �ǿյ��ֵ䣡");

            // ��ӡ���ö�ջ�԰�������
            Debug.Log("���ö�ջ:\n" + Environment.StackTrace);
        }
        else
        {
            Debug.Log($"���л�����: {gameData.Count} ������");

            // ��ӡǰ������Ŀ��������־����
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
        Debug.Log($"���л�JSON: {jsonData}"); // ���ڻ���ȷ���


        //// 2. ת��ΪJSON�ַ���
        //var wrapper = new SerializationWrapper(gameData);
        //Debug.Log($"Wrapper ������: {wrapper.data?.Count ?? 0}");
        //// 3. ���л�Ϊ JSON
        //string jsonData = JsonUtility.ToJson(wrapper, prettyPrint: true);
        //Debug.Log($"���л�JSON: {jsonData}");  // ����Ӧ������ȷ���
        //// 3. ���浽PlayerPrefs (ʵ����Ŀ��Ӧ�ô浽���ݿ�)
        //PlayerPrefs.SetString("SavedGame", jsonData);
        //PlayerPrefs.Save();

        string path = Application.persistentDataPath + "/savefile.json";
        Debug.Log($"����·��: {path}");
        SaveToJsonFile(jsonData, path);
        Debug.Log("��Ϸ�ѱ��棡");
    }

    // ��PlayerPrefs������Ϸ����
    public void LoadGame()
    {
        try
        {
            // 1. ����·������ƽ̨����д����
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");
            Debug.Log($"���Դ�·������: {path}");

            // 2. ����ļ��Ƿ����
            if (!File.Exists(path))
            {
                Debug.LogWarning("�浵�ļ������ڣ�·��: " + path);
                return;
            }

            // 3. ��ȡ�ļ�����
            string jsonData = File.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogError("�浵�ļ�Ϊ�գ�");
                return;
            }

            // 4. �����л���ʹ�� Newtonsoft.Json��
            var gameData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

            // 5. ��֤����
            if (gameData == null || gameData.Count == 0)
            {
                Debug.LogWarning("�����л�����Ϊ�գ�");
                return;
            }

            // 6. �ָ���Ϸ״̬
            UniversalDeserializer.DeserializeAllObjects(gameData);
            Debug.Log("��Ϸ���سɹ������ض�������: " + gameData.Count);
        }
        catch (Exception ex)
        {
            Debug.LogError($"����ʧ��: {ex.GetType()} - {ex.Message}\n{ex.StackTrace}");
        }
    }

    // �������������浽JSON�ļ�
    private void SaveToJsonFile(string jsonData, string customPath = null)
    {
        // ���û��ָ��·����ʹ��Ĭ��·��
        string filePath = customPath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

        try
        {
            // д���ļ�
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"��Ϸ�ѱ��浽�ļ�: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�����ļ�ʧ��: {e.Message}");
        }
    }

    // �ļ�������Ϸ����
    public void LoadGameFromJson(bool fromFile = true, string filePath = null)
    {
        string jsonData;

        if (fromFile)
        {
            // ���ļ�����
            string path = filePath ?? Path.Combine(Application.persistentDataPath, "SavedGame.json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"�浵�ļ�������: {path}");
                return;
            }

            try
            {
                jsonData = File.ReadAllText(path);
                Debug.Log($"���ļ����ش浵: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"��ȡ�浵�ļ�ʧ��: {e.Message}");
                return;
            }
        }
        else
        {
            // ��PlayerPrefs����
            if (!PlayerPrefs.HasKey("SavedGame"))
            {
                Debug.LogWarning("û���ҵ�PlayerPrefs�浵��");
                return;
            }

            jsonData = PlayerPrefs.GetString("SavedGame");
            Debug.Log("��PlayerPrefs���ش浵");
        }

        // �����л�
        SerializationWrapper wrapper = JsonUtility.FromJson<SerializationWrapper>(jsonData);

        // �ָ���Ϸ״̬
        UniversalDeserializer.DeserializeAllObjects(wrapper.data);

        Debug.Log("��Ϸ�Ѽ��أ�");
    }

    // �����࣬���ڰ�װDictionary�Ա�JsonUtility����ȷ���л�
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