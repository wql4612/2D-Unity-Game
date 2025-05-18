using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
        public int maxSize; // 新增最大数量
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, int> poolCount; // 当前池中对象总数

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // 初始化对象池
    private void InitializePool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolCount = new Dictionary<string, int>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolCount.Add(pool.tag, pool.initialSize);
        }
    }

    // 从池中获取对象
    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag {tag} doesn't exist!");
            return null;
        }

        Pool poolConfig = pools.Find(p => p.tag == tag);

        // 如果池空了且未超过最大数量，则新建对象
        if (poolDictionary[tag].Count == 0)
        {
            if (poolCount[tag] < poolConfig.maxSize)
            {
                GameObject newObj = Instantiate(poolConfig.prefab);
                newObj.SetActive(false);
                poolDictionary[tag].Enqueue(newObj);
                poolCount[tag]++;
            }
            else
            {
                Debug.LogWarning($"Pool with tag {tag} has reached max size!");
                return null;
            }
        }

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        // 调用对象的初始化方法
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        return obj;
    }

    // 将对象回收到池
    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag {tag} doesn't exist!");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
        // poolCount 不变，因为对象没有被销毁
    }
}
// 接口定义
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}
