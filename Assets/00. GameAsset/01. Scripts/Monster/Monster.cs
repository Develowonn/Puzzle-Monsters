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
    private int       groupKey;
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

	private void Start()
	{
		Initialize(InGameManager.Instance.GetPlayerTransform().GetComponent<Player>(), MonsterRole.Leader, new AStartPathFinder(), movementTime);
	}

	public void Initialize(Player player, MonsterRole monsterRole, IPathFinder pathFinder, float movementTime)
    {
        this.movementTime = movementTime;
        this.player       = player;
        isAttack          = true;

		// Monster Ai 설정
		monsterAI.Initialize(this, pathFinder);
        repathInterval = GameManager.Instance.GetRepathInterval();
        repathTimer    = repathInterval;
        
        // 규칙에 맞게 움직이도록 실행
        HandleMovementLoopAsync().Forget();
    }

    public void Initialize(Player player, float movementTime, int groupKey)
    {
		this.movementTime = movementTime;
		this.player       = player;
        this.groupKey     = groupKey;
		isAttack          = true;

		// 규칙에 맞게 움직이도록 실행
		HandleMovementLoopAsync().Forget();
	}
        
	private void Update()
    {
        if(player.IsDie) return;

        UpdatePath();
		UpdateRunAnimation();
        UpdateSpriteX();
        TryAttackPlayer();
    }

    private async UniTaskVoid HandleMovementLoopAsync()
    {
        // 객체가 삭제될 경우 비동기 함수도 종료
        var token = this.GetCancellationTokenOnDestroy();

        while (!token.IsCancellationRequested)
        {
            if (monsterAI != null && monsterAI.HasPath())
            {
                Vector3 end = (Vector3Int)monsterAI.GetTargetNodePosition();
                await MoveSmoothGrid(end);
                monsterAI.MoveNextNode();
            }
            await UniTask.Yield();
        }
    }

    private void UpdatePath()
    {
		repathTimer += Time.deltaTime;
		if (repathTimer > repathInterval && !IsMove)
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

    public void PromoteToLeader()
    {
        monsterRole = MonsterRole.Leader;

        // 경로 재탐색 
    }

    public MonsterRole GetMonsterRole()
    {
        return monsterRole;
    }
}