using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class UniversalSerializer
{
    // ���л����������ж���
    public static Dictionary<string, Dictionary<string, string>> SerializeAllObjects()
    {
        var allData = new Dictionary<string, Dictionary<string, string>>();

        // �������������л����
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>(true))
        {
            //obj.activeInHierarchy,�������

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

    // ���л���������
    private static Dictionary<string, string> SerializeObject(GameObject obj)
    {
        var objData = new Dictionary<string, string>();

        // ���뱣�� active ״̬
        objData["GameObject.active"] = obj.activeSelf.ToString();

        // ���л�Transform��������
        SerializeTransform(obj.transform, objData);

        // ���л��������
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null) continue;

            Type componentType = component.GetType();

            // ����Unity�������(��Щͨ������Ҫ����)
            if (componentType.Assembly == typeof(MonoBehaviour).Assembly &&
                !componentType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                continue;
            }

            SerializeComponent(component, objData);
        }

        return objData;
    }

    // ���л�Transform
    private static void SerializeTransform(Transform transform, Dictionary<string, string> data)
    {
        data["Transform.position"] = JsonUtility.ToJson(transform.position);
        data["Transform.rotation"] = JsonUtility.ToJson(transform.rotation.eulerAngles);
        data["Transform.scale"] = JsonUtility.ToJson(transform.localScale);
        // BreakableObject �� active ״̬������ʵ����������
        data["GameObject.active"] = transform.gameObject.activeSelf.ToString();
    }

    // ���л����
    private static void SerializeComponent(Component component, Dictionary<string, string> data)
    {
        Type type = component.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // �������ΪNonSerialized���ֶ�
            if (Attribute.IsDefined(field, typeof(NonSerializedAttribute)))
                continue;

            // ����ί�к��¼�
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

    // ��ȡ����Ψһ��ʶ
    private static string GetUniqueObjectId(GameObject obj)
    {
        // ʹ��InstanceIDȷ��Ψһ�ԣ���ʹͬ������Ҳ�����ͻ
        return $"{obj.name}_{obj.GetInstanceID()}";
    }
}