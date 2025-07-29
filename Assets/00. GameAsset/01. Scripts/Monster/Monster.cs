// # System
using System;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;
using Spine.Unity;

public class Monster : Character
{
    [SerializeField]
    private MonsterType  monsterType;
    private MonsterRole  monsterRole;

	[Space(10)] [SerializeField]
    private float        attackRange;
    [SerializeField]
    private float        attackCooltime;

	[Header("Animation")]
	[SerializeField] [SpineAnimation]
	private string       runAnimationName;

	private bool         isAttack;
    private int          groupID;

	private float        repathInterval = 0.0f;
	private float        repathTimer    = 0.0f;

	private Player               player;
    private LineRenderer         lineRenderer;
    private MonsterAI            monsterAI;
	private SkeletonAnimation    skeletonAnimation;
	private Spine.AnimationState spineAnimationState;

	private void Awake()
    {
        monsterAI      = new MonsterAI();
	}

	private void Update()
	{
		if (player.IsDie || !GameManager.Instance.IsGamePlaying())
			return;

		UpdatePath();
		UpdateSpriteX();
		TryAttackPlayer();
	}

	#region 초기화 관련 함수 
	public void Initialize(Player player, MonsterRole monsterRole, float delay, int groupKey)
    {
        this.player             = player;
		groupID                 = groupKey;
		isAttack                = true;
		IPathFinder pathFinder  = null;

        if (monsterRole == MonsterRole.Leader)
        {
            pathFinder   = new AStarPathFinder();
            lineRenderer = GetComponent<LineRenderer>();
        }

		// Monster Ai 설정
		monsterAI.Initialize(this, pathFinder);
        repathInterval = GameManager.Instance.GetRepathInterval();
        repathTimer    = UnityEngine.Random.Range(0, repathInterval);

		// 애니메이션 설정 
		InitializeAnimation();

		// 규칙에 맞게 움직이도록 실행
		HandleMovementLoopAsync(delay).Forget();
	}

	private void InitializeAnimation()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.AnimationState;
		PlayRunAnimation();
	}
	#endregion

	#region 이동 관련 함수 
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
                await MoveLeaderStepAsync();
            else 
                await MoveFollowerStepAsync();

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
	#endregion

	#region 경로 업데이트 
	private void UpdatePath()
    {
        // 리더만 경로를 업데이트 하도록 조건 설정
        if(monsterRole == MonsterRole.Follower) return;

		repathTimer += Time.deltaTime;
		if (repathTimer >= repathInterval && !IsMove)
		{
			monsterAI.UpdatePath();
            monsterAI.DrawPath();
			repathTimer = 0;
		}
	}
	#endregion

	#region 방향 제어 및 공격 관련 함수 
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
	#endregion

	#region 제어 함수
	public void AssignRole(MonsterRole role)
    {
        monsterRole = role;
    }

    public void EnableLineRender()
    {
        if(lineRenderer != null)
            lineRenderer.enabled = !lineRenderer.enabled;
    }
	#endregion

	#region Getter 함수 
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

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }
	#endregion

	public void PlayRunAnimation()
	{
		var current = spineAnimationState.GetCurrent(0);
		if (current == null || current.Animation.Name != runAnimationName)
			spineAnimationState.SetAnimation(0, runAnimationName, true);
	}
}