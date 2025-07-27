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

	[Header("Monster Movement Time")]
	[SerializeField]
	private float movementTime;
	[SerializeField]
	private float movementTimeStep;

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

				int groupID = monsterGroupKeyGenerator.GetNextGroupKey();

				for (int j = 0; j < pointSpawnData.monsterCount; j++)
				{
					if (pointSpawnData.type == MonsterType.None)
						continue;

					float		movementTime = Mathf.Max(0.1f, this.movementTime + (movementTimeStep * j));
					MonsterRole	monsterRole  = j == 0 ? MonsterRole.Leader : MonsterRole.Follower;
					Monster     monster      = MonsterPoolManager.Instance.SpawnFromPool(pointSpawnData.type, spawnPoint).GetComponent<Monster>();

					// 소환된 몬스터를 Monster Group 에 넣기 
					MonsterGroupManager.Instance.RegisterToMonsterGroup(groupID, monster);

					// Monster Init
					monster.Initialize(player, monsterRole, movementTime, groupID);
				}
			}

			await UniTask.Delay(TimeSpan.FromSeconds(nextWaveInterval));
			WaveManager.Instance.SetNextWaveData();
		}
	}
}
