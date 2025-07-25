// # System 
using System.Text;

// # Unity
using UnityEngine;

// # Etc
using TMPro;
using DG.Tweening;

public class InGameUIManager : MonoBehaviour
{
	public static InGameUIManager Instance { get; private set; }

	[Header("Life UI")]
	[SerializeField]
	private SpriteRenderer[]	lifesIcon;
	[SerializeField]
	private float			    lifeFadeDuration;

	[Header("Alive Monster UI")]
	[SerializeField]
	private TMP_Text		    aliveMonsterCountText;

	[Header("Timer UI")]
	[SerializeField]
	private TMP_Text            timerText;
	private float			    time;

	[Header("Wave UI")]
	[SerializeField]
	private TMP_Text		    waveText;
	private int				    wave = 1;

	[Header("Debug Mode UI")]
	[SerializeField]
	private Animator			debugAniamtor;
	private int                 hashIsDebug;

	private StringBuilder		waveStringBuilder;
	private StringBuilder       timerStringBuilder;
	private StringBuilder		aliveMonsterCountStringBuilder;

	private Player				player;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		hashIsDebug                    = Animator.StringToHash("IsDebug");

		waveStringBuilder			   = new StringBuilder();
		timerStringBuilder             = new StringBuilder();
		aliveMonsterCountStringBuilder = new StringBuilder();
	}

	private void Start()
	{
		player = InGameManager.Instance.GetPlayerTransform().GetComponent<Player>();
	}

	private void Update()
	{
		UpdateTimerUI();
	}

	public void FadeOutLifeIcon()
	{
		if(player.CurrentLifeCount < 0) return;

		lifesIcon[player.CurrentLifeCount].DOFade(0.0f, lifeFadeDuration);
	}

	public void UpdateTimerUI()
	{
		time += Time.deltaTime;

		timerStringBuilder.Clear();
		timerStringBuilder.Append(time.ToString("F2"));

		timerText.text = timerStringBuilder.ToString();
	}

	public void UpdateAliveMonsterCountUI()
    {
		aliveMonsterCountStringBuilder.Clear();
		aliveMonsterCountStringBuilder.Append($"{InGameManager.Instance.GetAliveMonsterCount()}");

		aliveMonsterCountText.text = aliveMonsterCountStringBuilder.ToString();
    }

	public void UpdateWaveUI()
    {
		wave++;

		waveStringBuilder.Clear();
		waveStringBuilder.Append($"Wave {wave}");

		waveText.text = waveStringBuilder.ToString();
    }

	public void OnDebugMode()
	{
		if(!debugAniamtor.GetBool(hashIsDebug))
			debugAniamtor.SetBool(hashIsDebug, true);
		else 
			debugAniamtor.SetBool(hashIsDebug, false);
	}
}
