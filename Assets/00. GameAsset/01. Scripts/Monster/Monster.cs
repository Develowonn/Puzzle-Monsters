// # System
using System;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Monster : Character
{
    [SerializeField]
    private MonsterType monsterType;
    private MonsterRole monsterRole;

    [SerializeField] [Space(10)]
    private float     attackRange;
    [SerializeField]
    private float     attackCooltime;

    private bool      isAttack;
    private int       groupID;
    private int       hashIsRun;

	private float     repathInterval = 0.0f;
	private float     repathTimer    = 0.0f;

	private Player    player;
    private Animator  animator;
    private MonsterAI monsterAI;

    private void Awake()
    {
        monsterAI      = new MonsterAI();
        animator       = GetComponent<Animator>();
        hashIsRun      = Animator.StringToHash("IsRun");
    }

	public void Initialize(Player player, MonsterRole monsterRole, float delay, int groupKey)
    {
        this.player             = player;
		groupID                 = groupKey;
		isAttack                = true;
		IPathFinder pathFinder  = null;

        // 경로 검색기 생성
        if (monsterRole == MonsterRole.Leader)
            pathFinder = new AStarPathFinder();

		// Monster Ai 설정
		monsterAI.Initialize(this, pathFinder);
        repathInterval = GameManager.Instance.GetRepathInterval();
        repathTimer    = repathInterval;
        
        // 규칙에 맞게 움직이도록 실행
        HandleMovementLoopAsync(delay).Forget();
    }
        
	private void Update()
    {
        if(player.IsDie || !GameManager.Instance.IsGamePlaying())
            return;

        UpdatePath();
		UpdateRunAnimation();
        UpdateSpriteX();
        TryAttackPlayer();
    }

    private async UniTaskVoid HandleMovementLoopAsync(float delay)
    {
        // 객체가 삭제될 경우 비동기 함수도 종료
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        while (!token.IsCancellationRequested)
        {
			if (!GameManager.Instance.IsGamePlaying())
            {
				await UniTask.Yield();
				continue;
			}

			if (monsterRole == MonsterRole.Leader)
            {
                await MoveLeaderStepAsync();
			}
            else
            {
                await MoveFollowerStepAsync();
			}
			await UniTask.Yield();
		}
	}

    private async UniTask MoveLeaderStepAsync()
    {
		if (monsterAI == null || !monsterAI.HasPath())
                return;

		Vector2Int end = monsterAI.GetTargetNodePosition();

        // Follow Monster 들에게 경로 공유
        if (GameManager.Instance.IsGroupSystem())
        {
			MonsterGroup monsterGroup = MonsterGroupManager.Instance.GetMonsterGroup(groupID);
			monsterGroup.RecordPathToFollowers(end);
		}

		await MoveSmoothGrid((Vector3Int)end);
		monsterAI.MoveNextNode();
	}

    private async UniTask MoveFollowerStepAsync()
    {
		if (monsterAI == null && !monsterAI.HasLeaderNodeHistory())
            return;
        
		Vector2Int? end = monsterAI.GetNextLeaderNode();
		if (end == null) 
            return;

		await MoveSmoothGrid((Vector3Int)end);
		monsterAI.MoveNextNode();
	}

    private void UpdatePath()
    {
        // 리더만 경로를 업데이트 하도록 조건 설정
        if(monsterRole == MonsterRole.Follower) return;

		repathTimer += Time.deltaTime;
		if (repathTimer >= repathInterval && !IsMove)
		{
			monsterAI.UpdatePath();
			repathTimer = 0;
		}
	}

    private void UpdateRunAnimation()
    {
        if (!animator.GetBool(hashIsRun) && IsMove)
        {
            animator.SetBool(hashIsRun, true);
        }
        else if (animator.GetBool(hashIsRun) && !IsMove)
        {
            animator.SetBool(hashIsRun, false);
        }
    }

    private void UpdateSpriteX()
    {
        if(transform.position.x > InGameManager.Instance.GetPlayerTransform().position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else 
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void TryAttackPlayer()
    {
        float distanceSqr = (InGameManager.Instance.GetPlayerTransform().position - transform.position).sqrMagnitude;

        if (isAttack && distanceSqr <= attackRange)
        {
            isAttack = false;

            player.TakeDamage();
            WaitForAttackCooltime().Forget();
        }
    }

    private async UniTaskVoid WaitForAttackCooltime()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(attackCooltime));
        isAttack = true;
    }

    public void AssignRole(MonsterRole role)
    {
        monsterRole = role;
    }

    public int GetGroupID()
    {
        return groupID;
    }

    public float GetMovementTime()
    {
        return movementTime;
    }

    public MonsterAI GetMonsterAI()
    {
        return monsterAI;
    }

    public MonsterRole GetMonsterRole()
    {
        return monsterRole;
    }
}