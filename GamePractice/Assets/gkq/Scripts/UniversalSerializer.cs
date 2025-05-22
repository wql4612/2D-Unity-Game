using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class UniversalSerializer
{
    // 序列化场景中所有对象
    public static Dictionary<string, Dictionary<string, string>> SerializeAllObjects()
    {
        var allData = new Dictionary<string, Dictionary<string, string>>();

        // 遍历场景中所有活动对象
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>(true))
        {
            //obj.activeInHierarchy,无需过滤

            Debug.Log($"Serializing object: {obj.name} (ID: {obj.GetInstanceID()})");
            var objData = SerializeObject(obj);
            if (objData.Count > 0)
            {
                string uniqueId = GetUniqueObjectId(obj);
                allData.Add(uniqueId, objData);
            }

        }

        return allData;
    }

    // 序列化单个对象
    private static Dictionary<string, string> SerializeObject(GameObject obj)
    {
        var objData = new Dictionary<string, string>();

        // 必须保存 active 状态
        objData["GameObject.active"] = obj.activeSelf.ToString();

        // 序列化Transform基础数据
        SerializeTransform(obj.transform, objData);

        // 序列化所有组件
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null) continue;

            Type componentType = component.GetType();

            // 跳过Unity内置组件(这些通常不需要保存)
            if (componentType.Assembly == typeof(MonoBehaviour).Assembly &&
                !componentType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                continue;
            }

            SerializeComponent(component, objData);
        }

        return objData;
    }

    // 序列化Transform
    private static void SerializeTransform(Transform transform, Dictionary<string, string> data)
    {
        data["Transform.position"] = JsonUtility.ToJson(transform.position);
        data["Transform.rotation"] = JsonUtility.ToJson(transform.rotation.eulerAngles);
        data["Transform.scale"] = JsonUtility.ToJson(transform.localScale);
        // BreakableObject 的 active 状态，用于实现物体销毁
        data["GameObject.active"] = transform.gameObject.activeSelf.ToString();
    }

    // 序列化组件
    private static void SerializeComponent(Component component, Dictionary<string, string> data)
    {
        Type type = component.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // 跳过标记为NonSerialized的字段
            if (Attribute.IsDefined(field, typeof(NonSerializedAttribute)))
                continue;

            // 跳过委托和事件
            if (field.FieldType.IsSubclassOf(typeof(Delegate)) ||
                field.FieldType.IsSubclassOf(typeof(MulticastDelegate)))
                continue;

            try
            {
                object value = field.GetValue(component);
                string jsonValue = JsonUtility.ToJson(value);
                string key = $"{type.Name}.{field.Name}";
                data[key] = jsonValue;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to serialize {type.Name}.{field.Name}: {e.Message}");
            }
        }
    }

    // 获取对象唯一标识
    private static string GetUniqueObjectId(GameObject obj)
    {
        // 使用InstanceID确保唯一性，即使同名对象也不会冲突
        return $"{obj.name}_{obj.GetInstanceID()}";
    }
}