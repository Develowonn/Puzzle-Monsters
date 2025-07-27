// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class PlayerMovement : MonoBehaviour
{
    private Vector3  movementDirection;

    private int      hashIsRun;

    private Animator animator;
    private Player   player;

    private void Awake()
    {
        player   = GetComponent<Player>();
        animator = GetComponent<Animator>();

        hashIsRun = Animator.StringToHash("IsRun");
    }

    private void Start()
    {
        HandleMovementLoopAsync().Forget();
    }

    private void Update()
    {
        if (player.IsDie || !GameManager.Instance.IsGamePlaying())
        {
            movementDirection = Vector3.zero;
            return;
        }

        UpdateMovementDirection();
        UpdateRunAnimation();
        UpdateSpriteX();
    }

    public async UniTaskVoid HandleMovementLoopAsync()
    {
        // 객체가 삭제될 경우 비동기 함수도 종료
        var token = this.GetCancellationTokenOnDestroy();

        while (!token.IsCancellationRequested && GameManager.Instance.IsGamePlaying())
        {
            // 플레이어가 움직일 방향에 벽이 있는지 검사
            bool isWall = GridMapManager.Instance.IsWallAtPosition(transform.position + movementDirection.normalized);

            if (movementDirection != Vector3.zero && !player.IsMove && !isWall)
            {
                Vector3 end = transform.position + movementDirection.normalized;
                await player.MoveSmoothGrid(end);   
            }
            await UniTask.Yield();
        }
    }

    private void UpdateMovementDirection()
    {
        // 대각선으로 움직이는걸 방지하고, 입력 우선순위를 정함 
        // 우선순위 : 위 -> 아래 -> 왼쪽 -> 오른쪽
        if      (Input.GetKey(KeyCode.W)) movementDirection = Vector3.up;
        else if (Input.GetKey(KeyCode.S)) movementDirection = Vector3.down;
        else if (Input.GetKey(KeyCode.A)) movementDirection = Vector3.left;
        else if (Input.GetKey(KeyCode.D)) movementDirection = Vector3.right;
        else movementDirection = Vector3.zero;
    }

    private void UpdateRunAnimation()
    {
        if (movementDirection == Vector3.zero && animator.GetBool(hashIsRun))
        {
            animator.SetBool(hashIsRun, false);
        }
        else if (movementDirection != Vector3.zero && !animator.GetBool(hashIsRun))
        {
            animator.SetBool(hashIsRun, true);
        }
    }

    private void UpdateSpriteX()
    {
        if (movementDirection != Vector3.zero && movementDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movementDirection != Vector3.zero && movementDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
