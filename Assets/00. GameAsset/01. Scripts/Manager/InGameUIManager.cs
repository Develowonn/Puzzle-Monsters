// # System 
using System.Text;

// # Unity
using UnityEngine;
using UnityEngine.UI;

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

	[Header("Timer UI")]
	[SerializeField]
	private TMP_Text            timerText;
	private float			    time;

	private StringBuilder       timerStringBuilder;
	private Player				player;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		player = InGameManager.Instance.GetPlayerTransform().GetComponent<Player>();

		timerStringBuilder = new StringBuilder();
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
}
