// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Player : Character
{
    [SerializeField]
    private int      maxLifeCount;
    [SerializeField]
    private int      invincibilityDuration;
    private int      currentLifeCount;

    private bool     isInvincibility;
    private int      hashOnDie;

    private Animator animator;

    // �б����� ������Ƽ
    public int       CurrentLifeCount => currentLifeCount;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
		currentLifeCount = maxLifeCount;
		hashOnDie        = Animator.StringToHash("OnDie");
    }

    private async UniTaskVoid ActivateInvincibility()
    {
        isInvincibility = true;
        await UniTask.Delay(TimeSpan.FromSeconds(invincibilityDuration));
        isInvincibility = false;
    }

    public void TakeDamage()
    {
        if (isInvincibility) return;
		currentLifeCount--;

        // ��Ʈ ������ ���� 
        InGameUIManager.Instance.FadeOutLifeIcon();

        // �����ð����� ���� �ο� 
        ActivateInvincibility().Forget();

        if (!IsDie && currentLifeCount <= 0)
            Die();
    }

    private void Die()
    {
        IsDie = true;
        animator.SetTrigger(hashOnDie);
    }
}