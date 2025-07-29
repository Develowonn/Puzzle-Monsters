// # Unity
using UnityEngine;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

// # Etc
using TMPro;
using DG.Tweening;

public class LobbyUIManager : MonoBehaviour
{
	[SerializeField]
	private float		scaleEffectDuration;

	[Header("Game Start Panel")]
	[SerializeField]
	private GameObject	gameStartPanel;
	[SerializeField]
	private Button      groupSystemStateButton;
	[SerializeField]
	private TMP_Text	groupSystemStateText;
	[SerializeField]
	private Button		gameStartButton;

	[Header("Color")]
	[SerializeField]
	private Color       defaultButtonColor;
	[SerializeField]
	private Color       selectedButtonColor;

	private void Start()
	{
		InitializeGameStartPanel();

		FadeManager.Instance.Fade();
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0) && !gameStartPanel.activeSelf)
		{
			gameStartPanel.SetActive(true);
			gameStartPanel.transform.DOScale(Vector3.one, scaleEffectDuration);
		}
	}

	private void InitializeGameStartPanel()
	{
		groupSystemStateButton.onClick.AddListener(() => OnGroupSystemButton());
		gameStartButton.onClick.AddListener(() => SceneManager.LoadScene(Constants.SceneName.Game));

		gameStartPanel.SetActive(false);
	}

	private void OnGroupSystemButton()
	{
		bool isGroupSystem		  = GameManager.Instance.IsGroupSystem() ? false : true;
		groupSystemStateText.text = isGroupSystem ? "On" : "OFF"; 

		// Button Color
		var color = groupSystemStateButton.colors;

		if (isGroupSystem)
			color = GetColorBlock(color, true);
		else
			color = GetColorBlock(color, false);

		groupSystemStateButton.colors = color;

		GameManager.Instance.SetGroupSystemEnabled(isGroupSystem);
	}

	private ColorBlock GetColorBlock(ColorBlock colorBlock, bool isClick)
	{
		var color = colorBlock;

		if (isClick)
		{
			color.normalColor   = selectedButtonColor;
			color.selectedColor = selectedButtonColor;
		}
		else
		{
			color.normalColor   = defaultButtonColor;
			color.selectedColor = defaultButtonColor;
		}

		return color;
	}

	private void OnDestroy()
	{
		DOTween.Kill(this);
	}
}
