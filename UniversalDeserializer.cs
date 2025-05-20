using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public static class UniversalDeserializer
{
    public static void DeserializeAllObjects(Dictionary<string, Dictionary<string, string>> savedData)
    {
        // �����ռ��������������ж���
        Dictionary<string, GameObject> sceneObjects = new Dictionary<string, GameObject>();
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (obj.activeInHierarchy)
            {
                sceneObjects.Add(GetUniqueObjectId(obj), obj);
            }
        }

        // �����������
        foreach (var kvp in savedData)
        {
            string objectId = kvp.Key;
            Dictionary<string, string> objData = kvp.Value;

            // ���ҳ����еĶ�Ӧ����
            if (sceneObjects.TryGetValue(objectId, out GameObject obj))
            {
                DeserializeObject(obj, objData);
            }
            else
            {
                Debug.LogWarning($"Saved object not found in scene: {objectId}");
            }
        }
    }

    private static void DeserializeObject(GameObject obj, Dictionary<string, string> objData)
    {
        // Debug�������Ȼָ� active ״̬���ڻָ�Transform֮ǰ��
        if (objData.TryGetValue("GameObject.active", out string activeStr))
        {
            bool isActive = bool.Parse(activeStr);
            obj.SetActive(isActive);
        }
        // �����л�Transform
        if (objData.TryGetValue("Transform.position", out string posJson))
        {
            obj.transform.position = JsonUtility.FromJson<Vector3>(posJson);
        }
        if (objData.TryGetValue("Transform.rotation", out string rotJson))
        {
            obj.transform.rotation = Quaternion.Euler(JsonUtility.FromJson<Vector3>(rotJson));
        }
        if (objData.TryGetValue("Transform.scale", out string scaleJson))
        {
            obj.transform.localScale = JsonUtility.FromJson<Vector3>(scaleJson);
        }

        // �����л����
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null) continue;

            Type componentType = component.GetType(); // �����ȡ�������
            FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                string key = $"{componentType.Name}.{field.Name}";
                if (objData.TryGetValue(key, out string valueJson))
                {
                    try
                    {
                        object value = JsonUtility.FromJson(valueJson, field.FieldType);
                        field.SetValue(component, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to deserialize {key}: {e.Message}");
                    }
                }
            }
        }
    }

    private static string GetUniqueObjectId(GameObject obj)
    {
        return $"{obj.name}_{obj.GetInstanceID()}";
    }
}