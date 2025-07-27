// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class MonsterSpawner : MonoBehaviour
{
	[SerializeField]
	private float		nextWaveInterval;
	[SerializeField]
	private float	    movementDelayTimeStep;
	[SerializeField]
	private Transform[] spawnPoints;
	[SerializeField]
	private Player		player;

	private MonsterGroupKeyGenerator monsterGroupKeyGenerator;

	private void Awake()
	{
		monsterGroupKeyGenerator = new MonsterGroupKeyGenerator();
	}

	private void Start()
    {
		SpawnMonsterInWaveAsync().Forget();
    }

    public async UniTaskVoid SpawnMonsterInWaveAsync()
	{
		// ��ü�� ������ ��� �񵿱� �Լ��� ����
		var token = this.GetCancellationTokenOnDestroy();

		while (!token.IsCancellationRequested)
        {
			if (!GameManager.Instance.IsGamePlaying())
			{
				await UniTask.Yield();
				continue;
			}

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

					float		delay        = movementDelayTimeStep * j;
					MonsterRole	monsterRole  = GetMonsterRole(j);
					Monster     monster      = MonsterPoolManager.Instance.SpawnFromPool(pointSpawnData.type, spawnPoint).GetComponent<Monster>();

					// Monster Init
					monster.Initialize(player, monsterRole, delay, groupID);

					// IsGroupSystem Ȱ��ȭ �� ������ �� Monster Group �߰�
					if (GameManager.Instance.IsGroupSystem())
						MonsterGroupManager.Instance.RegisterToMonsterGroup(groupID, monster);
				}
			}

			await UniTask.Delay(TimeSpan.FromSeconds(nextWaveInterval));
			WaveManager.Instance.SetNextWaveData();
		}
	}

	private MonsterRole GetMonsterRole(int index)
	{
		if (!GameManager.Instance.IsGroupSystem())
			return MonsterRole.Leader;

		return index > 0 ? MonsterRole.Follower : MonsterRole.Leader;
	}
}
