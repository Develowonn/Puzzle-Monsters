// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class MonsterSpawner : MonoBehaviour
{
	[SerializeField]
	private Transform[] spawnPoints;
	[SerializeField]
	private float       nextWaveInterval;
	[SerializeField]
	private Player		player;

	private MonsterGroupKeyGenerator monsterGroupKeyGenerator;

    private void Start()
    {
		monsterGroupKeyGenerator = new MonsterGroupKeyGenerator();
		SpawnMonsterInWaveAsync().Forget();
    }

    public async UniTaskVoid SpawnMonsterInWaveAsync()
	{
        while (true)
        {
			WaveData currentWave = WaveManager.Instance.GetCurrentWaveData();

			for (int i = 0; i < currentWave.pointSpawnDatas.Length; i++)
			{
				Vector3        spawnPoint     = spawnPoints[i].position;
				PointSpawnData pointSpawnData = currentWave.pointSpawnDatas[i];

				// Point 의 Spawn Data 의 Monster Count만큼 소환 
				for (int j = 0; j < pointSpawnData.monsterCount; j++)
				{
					// 몬스터 타입이 정해져 있지 않으면 생성 X
					if (pointSpawnData.type == MonsterType.None)
						continue;

					Monster monster = MonsterPoolManager.Instance.SpawnFromPool(pointSpawnData.type, spawnPoint).GetComponent<Monster>();
				}
			}

			await UniTask.Delay(TimeSpan.FromSeconds(nextWaveInterval));
			WaveManager.Instance.SetNextWaveData();
		}
	}
}
