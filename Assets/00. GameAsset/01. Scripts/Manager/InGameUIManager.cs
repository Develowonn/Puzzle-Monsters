// # System 
using System.Text;

// # Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// # Etc
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class InGameUIManager : MonoBehaviour
{
	public static InGameUIManager Instance { get; private set; }

	[SerializeField]
	private float			    countingEffectTime;
	[SerializeField]
	private float				scaleEffectTime;

	[SerializeField] [Space(10)]
	private TMP_Text			gameStartTimertext;

	[Header("Life UI")]
	[SerializeField]
	private Image[]				lifesIcon;
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

	[Header("Game Over UI")]
	[SerializeField]
	private GameObject			gameOverBackground;
	[SerializeField]
	private GameObject			gameOverPanel;
	[SerializeField]
	private TMP_Text			finialTimerText;
	[SerializeField]
	private TMP_Text			finialMonsterCountText;
	[SerializeField]
	private Button				homeButton;
	[SerializeField]
	private Button				restartButton;

	private Player				player;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		hashIsDebug		    = Animator.StringToHash("IsDebug");
	}

	private void Start()
	{
		player = InGameManager.Instance.GetPlayerTransform().GetComponent<Player>();

		homeButton.onClick.AddListener(() => OnHomeButton());
		restartButton.onClick.AddListener(() => OnRestartButton());
	}

	private void Update()
	{
		if (!GameManager.Instance.IsGamePlaying()) 
			return;

		UpdateTimerUI();
	}

	public void FadeOutLifeIcon()
	{
		if(player.CurrentLifeCount < 0) 
			return;

		lifesIcon[player.CurrentLifeCount].DOFade(0.0f, lifeFadeDuration);
	}

	public void UpdateTimerUI()
	{
		time		  += Time.deltaTime;

		timerText.text = time.ToString("F2");
	}

	public void UpdateAliveMonsterCountUI()
    {
		aliveMonsterCountText.text = $"{InGameManager.Instance.GetAliveMonsterCount()}";
    }

	public void UpdateWaveUI()
    {
		wave++;
		waveText.text = $"Wave {wave}";
    }

	public async UniTask PlayGameStartTimer(int delay)
	{
		await PlayTextCountingAsync(gameStartTimertext, delay + 1, 0 + 1, delay);
		if(gameStartTimertext != null)
			gameStartTimertext.gameObject.SetActive(false);
	}

	public void ActivateGameOverPanel()
	{
		gameOverBackground.SetActive(true);
		gameOverPanel.transform.DOScale(Vector3.one, scaleEffectTime);

		PlayTextCountingAsync(finialTimerText, 0, time, "F2").Forget();
		PlayTextCountingAsync(finialMonsterCountText, 0, InGameManager.Instance.GetAliveMonsterCount()).Forget();

	}

	public void OnDebugMode()
	{
		if (!debugAniamtor.GetBool(hashIsDebug))
			debugAniamtor.SetBool(hashIsDebug, true);
		else
			debugAniamtor.SetBool(hashIsDebug, false);
	}

	private void OnHomeButton()
	{
		WaveManager.Instance.Restart();
		SceneManager.LoadScene(Constants.SceneName.Lobby);
	}

	private void OnRestartButton()
	{
		WaveManager.Instance.Restart();
		SceneManager.LoadScene(Constants.SceneName.Game);
	}

	private async UniTaskVoid PlayTextCountingAsync(TMP_Text text, float start, float end, string format)
	{
		float current = 0;
		float percent = 0;

		// 객체가 삭제될 경우 비동기 함수도 종료
		var token = this.GetCancellationTokenOnDestroy();

		while (!token.IsCancellationRequested && percent < 1)
		{
			current    += Time.deltaTime;	
			percent     = current / countingEffectTime;
			text.text   = Mathf.Lerp(start, end, percent).ToString(format);

			await UniTask.Yield();
		}
	}

	private async UniTaskVoid PlayTextCountingAsync(TMP_Text text, int start, int end)
	{
		float current = 0;
		float percent = 0;

		// 객체가 삭제될 경우 비동기 함수도 종료
		var token = this.GetCancellationTokenOnDestroy();

		while (!token.IsCancellationRequested && percent < 1)
		{
			current  += Time.deltaTime;
			percent   = current / countingEffectTime;
			text.text = Mathf.FloorToInt(Mathf.Lerp(start, end, percent)).ToString();

			await UniTask.Yield();
		}
	}

	private async UniTask PlayTextCountingAsync(TMP_Text text, int start, int end, float duration)
	{
		float current = 0;
		float percent = 0;

		// 객체가 삭제될 경우 비동기 함수도 종료
		var token = this.GetCancellationTokenOnDestroy();

		while (!token.IsCancellationRequested && percent < 1)
		{
			current += Time.deltaTime;
			percent = current / duration;

			if(text == null || !text.gameObject.activeSelf)
				break;

			text.text = Mathf.FloorToInt(Mathf.Lerp(start, end, percent)).ToString();

			await UniTask.Yield();
		}
	}

	private void OnDestroy()
	{
		DOTween.Kill(this);
	}
}
