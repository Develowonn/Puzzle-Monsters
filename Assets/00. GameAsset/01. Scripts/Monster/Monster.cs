// # System
using System.Collections;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Monster : Character
{
	[SerializeField]
	private float       movementTime;

	public  bool		IsMove { get; private set; }

	private MonsterAI	monsterAI;

	private void Awake()
	{
		monsterAI = GetComponent<MonsterAI>();
	}

	private void Start()
	{
		StartMovingAsync().Forget();
	}

	private async UniTaskVoid StartMovingAsync()
	{
		while (true)
		{
			if (monsterAI.GetTargetNodePosition() != Vector2Int.zero)
			{
				Vector3 end = (Vector3Int)monsterAI.GetTargetNodePosition();

				await MoveSmoothGrid(end);
				
				monsterAI.MoveNextNode();
			}

			await UniTask.Yield();
		}
	}

	private async UniTask MoveSmoothGrid(Vector3 end)
	{
		IsMove = true;

		Vector3 start = transform.position;
		float current = 0;
		float percent = 0;

		while (percent < 1.0f)
		{
			current += Time.deltaTime;
			percent  = current / movementTime;

			transform.position = Vector3.Lerp(start, end, percent);

			await UniTask.Yield();
		}

		IsMove = false;
	}
}
