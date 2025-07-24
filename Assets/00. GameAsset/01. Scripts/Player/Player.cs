// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Player : Character
{
    [SerializeField]
    private int      lives;
    private int      hashOnDie;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        hashOnDie = Animator.StringToHash("OnDie");
    }

    public void TakeDamage()
    {
        if (!IsDie)
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