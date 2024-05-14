using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public PoolItem[] pool;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddObjectsInPool();
    }

    void AddObjectsInPool()
    {
        foreach (PoolItem poolItem in pool)
        {
            for (int i = 0; i < poolItem.poolAmount; i++)
            {
                var pooledObject = Instantiate(poolItem.item, transform);
                if (poolItem.poolName!="Zombie")
                {
                    pooledObject.SetActive(false);
                }
                poolItem.pooledItems.Add(pooledObject);
            }
        }
    }

    public GameObject GetPooledObject(string poolName)
    {
        PoolItem poolItem = Array.Find(pool, item => item.poolName == poolName);
        var pooledObjects = poolItem.pooledItems;
        for (int i = 0; i < poolItem.pooledItems.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    void AddObjectInSpecificPool(string poolName)
    {
        PoolItem poolItem = Array.Find(pool, item => item.poolName == poolName);
        var item = Instantiate(poolItem.item);
        //if (poolItem.poolName!="Zombie")
        {
            item.SetActive(false);
        }
        
        poolItem.pooledItems.Add(item);
    }

    public GameObject FetchPooledObject(string name)
    {
        var pooledObject = GetPooledObject(name);
        if (!pooledObject)
        {
            AddObjectInSpecificPool(name);
            pooledObject = GetPooledObject(name);
        }
        return pooledObject;
    }

    public void GetObjectBackInPool(GameObject backObject)
    {
        backObject.SetActive(false);
        backObject.transform.parent = transform;
    }
    
}

[System.Serializable]
public class PoolItem
{
    public string poolName;
    public GameObject item;
    public List<GameObject> pooledItems;
    public int poolAmount;
    public bool canExpand;
}
