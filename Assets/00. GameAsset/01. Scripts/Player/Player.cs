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

        // ��� 1�� ���̳ʽ�
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
        GameManager.Instance.EndGame();
        InGameUIManager.Instance.ActivateGameOverPanel();

        IsDie = true;
        animator.SetTrigger(hashOnDie);
    }
}