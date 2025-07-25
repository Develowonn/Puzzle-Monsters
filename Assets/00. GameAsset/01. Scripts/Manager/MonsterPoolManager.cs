// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterPoolManager : MonoBehaviour
{
    public static MonsterPoolManager Instance { get; private set; }

    [SerializeField]
    private MonsterPool[] poolDatas;
    private Dictionary<MonsterType, MonsterPool> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

		Initialize();
	}

    private void Initialize()
    {
        poolDictionary = new Dictionary<MonsterType, MonsterPool>();

        for (int i = 0; i < poolDatas.Length; i++)
        {
            Register(poolDatas[i]);
        }
    }

    private void Register(MonsterPool poolData)
    {
        // 중복 키 등록 불가능 
        if (poolDictionary.ContainsKey(poolData.key))
            return;

        poolDictionary.Add(poolData.key, poolData);
        poolData.pool = new Queue<GameObject>();

        for (int i = 0; i < poolData.objectCount; i++)
        {
            Instantiate(poolData);
        }
    }

    public GameObject SpawnFromPool(MonsterType key, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(key))
            return null;

        var currentPool = poolDictionary[key];

        // 풀에 오브젝트가 없으면 새로 생성해서 추가
        if (currentPool.pool.Count <= 0)
        {
            Instantiate(currentPool);
        }

        GameObject result = currentPool.pool.Dequeue();
        result.transform.SetParent(null);
        result.transform.position = position;
        result.SetActive(true);

        InGameManager.Instance.AddAliveMonster(result.GetComponent<Monster>());

        return result;
    }

    public void ReturnToPool(MonsterType key, GameObject obj)
    {
        var currentPool = poolDictionary[key];

        obj.SetActive(false);
        obj.transform.SetParent(currentPool.parentTransform);
        currentPool.pool.Enqueue(obj);

        InGameManager.Instance.RemoveAliveMonster(obj.GetComponent<Monster>());
    }

    private void Instantiate(MonsterPool pool)
    {
        GameObject monsterObj = Instantiate(pool.monster.gameObject, pool.parentTransform);
        monsterObj.SetActive(false);
        poolDictionary[pool.key].pool.Enqueue(monsterObj);
    }
}
