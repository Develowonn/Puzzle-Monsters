// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(MonsterAI))]
public class Monster : Character
{
    [SerializeField]
    private MonsterType monsterType;

    [SerializeField] [Space(10)]
    private float     attackRange;
    [SerializeField]
    private float     attackCooltime;

    private bool      isAttack;
    private int       hashIsRun;

    private Player    player;
    private Animator  animator;

    private MonsterAI monsterAI;


    private void Awake()
    {
        monsterAI = GetComponent<MonsterAI>();
        animator  = GetComponent<Animator>();
        hashIsRun = Animator.StringToHash("IsRun");
    }

    private void Start()
    {
        Initialize(InGameManager.Instance.GetPlayerTransform().GetComponent<Player>(),
                   new AStartPathFinder(), movementTime);
    }

    private void Initialize(Player player, IPathFinder pathFinder, float movementTime)
    {
        this.movementTime = movementTime;
        this.player       = player;
        isAttack          = true;

		// Monster Ai 설정
		monsterAI.Initialize(this, pathFinder);
        
        // 규칙에 맞게 움직이도록 실행
        HandleMovementLoopAsync().Forget();
    }

    private void Update()
    {
        if(player.IsDie) return;

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
}