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
    }

    private void Start()
    {
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
        // �ߺ� Ű ��� �Ұ��� 
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

        // Ǯ�� ������Ʈ�� ������ ���� �����ؼ� �߰�
        if (currentPool.pool.Count <= 0)
        {
            Instantiate(currentPool);
        }

        GameObject result = currentPool.pool.Dequeue();
        result.transform.SetParent(null);
        result.transform.position = position;
        result.SetActive(true);

        InGameManager.Instance.IncreaseAliveMonsterCount();

        return result;
    }

    public void ReturnToPool(MonsterType key, GameObject obj)
    {
        var currentPool = poolDictionary[key];

        obj.SetActive(false);
        obj.transform.SetParent(currentPool.parentTransform);
        currentPool.pool.Enqueue(obj);

        InGameManager.Instance.DecreaseAliveMonsterCount();
    }

    private void Instantiate(MonsterPool pool)
    {
        GameObject monsterObj = Instantiate(pool.monster.gameObject, pool.parentTransform);
        monsterObj.SetActive(false);
        poolDictionary[pool.key].pool.Enqueue(monsterObj);
    }
}
