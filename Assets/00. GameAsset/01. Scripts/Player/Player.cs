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

    // 읽기전용 프로퍼티
    public int       CurrentLifeCount => currentLifeCount;

    private void Awake()
    {
        animator  = GetComponent<Animator>();
		hashOnDie = Animator.StringToHash("OnDie");
		currentLifeCount = maxLifeCount;
	}

    private async UniTaskVoid ActivateInvincibility()
    {
        isInvincibility = true;
        await UniTask.Delay(TimeSpan.FromSeconds(invincibilityDuration));
        isInvincibility = false;
    }

    public void TakeDamage()
    {
        if (isInvincibility || !GameManager.Instance.IsGamePlaying()) 
            return;

        // 목숨 1개 마이너스
		currentLifeCount--;

        // 하트 아이콘 연출 
        InGameUIManager.Instance.FadeOutLifeIcon();

        // 일정시간동안 무적 부여 
        ActivateInvincibility().Forget();

        if (!IsDie && currentLifeCount <= 0)
            Die();
    }

    private void Die()
    {
        GameManager.Instance.EndGame();
        InGameUIManager.Instance.ActivateGameOverPanel();

        IsDie = true;
        animator.SetTrigger(hashOnDie);
    }
}