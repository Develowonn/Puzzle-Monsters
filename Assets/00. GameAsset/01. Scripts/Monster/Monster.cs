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
    private float     attackRange;
    [SerializeField]
    private float     attackCooltime;
    private bool      isAttack;

    private int       hashIsRun;

    private Player    player;
    private MonsterAI monsterAI;
    private Animator  animator;

    private void Awake()
    {
        monsterAI = GetComponent<MonsterAI>();
        animator  = GetComponent<Animator>();

        hashIsRun = Animator.StringToHash("IsRun");
    }

    private void Start()
    {
        isAttack = true;
		player   = InGameManager.Instance.GetPlayerTransform().GetComponent<Player>();

		// Monster Ai ����
		monsterAI.Initialize(this, new AStartPathFinder());
        
        // ��Ģ�� �°� �����̵��� ����
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
        // ��ü�� ������ ��� �񵿱� �Լ��� ����
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