// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Player : Character
{
    [SerializeField]
    private int      maxLifeCount;
    private int      currentLifeCount;

    private int      hashOnDie;

    private Animator animator;

    // 읽기전용 프로퍼티
    public int CurrentLifeCount => currentLifeCount;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
		currentLifeCount = maxLifeCount;

		hashOnDie = Animator.StringToHash("OnDie");
    }

    public void TakeDamage()
    {
		currentLifeCount--;
        InGameUIManager.Instance.FadeOutLifeIcon();

        if (!IsDie && currentLifeCount <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDie = true;
        animator.SetTrigger(hashOnDie);
    }
}