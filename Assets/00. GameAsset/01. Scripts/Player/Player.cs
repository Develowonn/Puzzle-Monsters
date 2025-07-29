// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;
using Spine.Unity;

public class Player : Character
{
    [SerializeField]
    private int      maxLifeCount;
    [SerializeField]
    private int      invincibilityDuration;

    [Header("Animation")]
	[SerializeField] [SpineAnimation]
	private string    idleAnimationName;
	[SerializeField] [SpineAnimation]
	private string    runAnimationName;
	[SerializeField] [SpineAnimation]
	private string    deadAnimationName;

	private bool     isInvincibility;
	private int      currentLifeCount;

    private SkeletonAnimation    skeletonAnimation;
    private Spine.AnimationState spineAnimationState;

    // �б����� ������Ƽ
    public int       CurrentLifeCount => currentLifeCount;

    private void Awake()
    {
		currentLifeCount    = maxLifeCount;
        InitializeAnimation();
	}

    private void InitializeAnimation()
    {
		skeletonAnimation   = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.AnimationState;

		PlayIdleAniamtion();
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
        PlayDieAnimation();
    }

    public void PlayRunAnimation()
    {
        var current = spineAnimationState.GetCurrent(0);
        if (current == null || current.Animation.Name != runAnimationName)
			spineAnimationState.SetAnimation(0, runAnimationName, true);
    }

    public void PlayIdleAniamtion()
    {
		var current = spineAnimationState.GetCurrent(0);
		if (current == null || current.Animation.Name != idleAnimationName)
			spineAnimationState.SetAnimation(0, idleAnimationName, true);
	}

    public void PlayDieAnimation()
    {
		var current = spineAnimationState.GetCurrent(0);
		if (current == null || current.Animation.Name != deadAnimationName)
			spineAnimationState.SetAnimation(0, deadAnimationName, true);
	}
}