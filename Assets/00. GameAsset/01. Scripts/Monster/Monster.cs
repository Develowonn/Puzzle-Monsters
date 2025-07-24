// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Monster : Character
{
    private int       hashIsRun;

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
        // Monster Ai 설정
        monsterAI.Initialize(this, new AStartPathFinder());
        
        // 규칙에 맞게 움직이도록 실행
        HandleMovementLoopAsync().Forget();
    }

    private void Update()
    {
        UpdateRunAnimation();
        UpdateSpriteX();
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

    }
}
