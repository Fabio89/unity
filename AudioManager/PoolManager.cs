using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager
{
    const int defaultSize = 10;

    public GameObject pooledObject;
    public int pooledAmount;
    public bool canGrow;
    public string name;
    public List<GameObject> pool;

    public GameObject AddObject()
    {
        GameObject obj = Object.Instantiate(pooledObject);
        obj.name = name + "_" + pool.Count;
        obj.transform.SetParent(pooledObject.transform.parent);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public PoolManager(GameObject pooledObject, int pooledAmount = defaultSize, bool canGrow = true)
    {
        this.pooledObject = pooledObject;
        this.pooledAmount = pooledAmount;
        this.canGrow = canGrow;
        name = pooledObject.name;

        pool = new List<GameObject>(pooledAmount);
               
        pooledObject.name = pooledObject.name + "_0";
        pooledObject.SetActive(false);
        pool.Add(pooledObject);

        for (int i = 1; i < pooledAmount; i++)
        {
            AddObject();
        }
    }

    public GameObject PickAvailable()
    {
        int i;
        for (i = 0; i < pool.Count; i++)
            if (!pool[i].gameObject.activeInHierarchy)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
                
        if (canGrow)
        {            
            return AddObject();
        }
        return null;
    }
}
