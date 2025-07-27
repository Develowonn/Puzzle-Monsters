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
        // ��ü�� ������ ��� �񵿱� �Լ��� ����
        var token = this.GetCancellationTokenOnDestroy();

        while (!token.IsCancellationRequested && GameManager.Instance.IsGamePlaying())
        {
            // �÷��̾ ������ ���⿡ ���� �ִ��� �˻�
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
        // �밢������ �����̴°� �����ϰ�, �Է� �켱������ ���� 
        // �켱���� : �� -> �Ʒ� -> ���� -> ������
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
