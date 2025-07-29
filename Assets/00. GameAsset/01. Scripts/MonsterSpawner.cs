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
		// 객체가 삭제될 경우 비동기 함수도 종료
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

					// IsGroupSystem 활성화 된 상태일 때 Monster Group 추가
					if (GameManager.Instance.IsGroupSystem())
						MonsterGroupManager.Instance.RegisterToMonsterGroup(groupID, monster);

					if (InGameManager.Instance.IsDebugMode)
						monster.EnableLineRender();
				}
			}

			await UniTask.Delay(TimeSpan.FromSeconds(nextWaveInterval));

			if(GameManager.Instance.IsGamePlaying())
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
