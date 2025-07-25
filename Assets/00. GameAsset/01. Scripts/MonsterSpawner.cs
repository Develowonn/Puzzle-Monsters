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

    private void Start()
    {
		SpawnMonsterInWaveAsync().Forget();
    }

    public async UniTaskVoid SpawnMonsterInWaveAsync()
	{
        while (true)
        {
			WaveData currentWave = WaveManager.Instance.GetCurrentWaveData();

			for (int i = 0; i < currentWave.pointSpawnDatas.Length; i++)
			{
				Vector3 spawnPoint = spawnPoints[i].position;
				PointSpawnData pointSpawnData = currentWave.pointSpawnDatas[i];

				for (int j = 0; j < pointSpawnData.monsterCount; j++)
				{
					if (pointSpawnData.type == MonsterType.None)
						continue;
					MonsterPoolManager.Instance.SpawnFromPool(pointSpawnData.type, spawnPoint);
				}
			}

			await UniTask.Delay(TimeSpan.FromSeconds(nextWaveInterval));
			WaveManager.Instance.SetNextWaveData();
		}
	}
}
