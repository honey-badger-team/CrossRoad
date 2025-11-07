using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance; // 싱글톤

    [System.Serializable]
    public class PoolItem
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    public List<PoolItem> itemsToPool;
    private Dictionary<string, List<GameObject>> pooledObjects;

    void Awake()
    {
        Instance = this;
        pooledObjects = new Dictionary<string, List<GameObject>>();

        foreach (PoolItem item in itemsToPool)
        {
            List<GameObject> objectList = new List<GameObject>();
            for (int i = 0; i < item.initialSize; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform);
                obj.SetActive(false);
                objectList.Add(obj);
            }
            pooledObjects.Add(item.tag, objectList);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!pooledObjects.ContainsKey(tag)) return null;

        List<GameObject> objectList = pooledObjects[tag];
        foreach (GameObject obj in objectList)
        {
            if (!obj.activeInHierarchy) // 놀고 있는 오브젝트 찾기
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        // 풀이 부족하면 자동으로 확장 (PoolItem 정보 찾아서 새로 생성)
        PoolItem item = itemsToPool.Find(i => i.tag == tag);
        if (item != null)
        {
            GameObject newObj = Instantiate(item.prefab, transform);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);
            objectList.Add(newObj); // 새 오브젝트도 풀에 추가
            return newObj;
        }
        return null;
    }
}