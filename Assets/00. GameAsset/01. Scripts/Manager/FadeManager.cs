// # System
using System;

// # Unity
using UnityEngine;

// # Etc
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class FadeManager : MonoBehaviour
{
	public static FadeManager Instance { get; private set; }

	[SerializeField]
	private GameObject	  canvas;
	[SerializeField]
	private RectTransform fadeImage;
	[SerializeField]
	private float		  fadeDuration;
	[SerializeField]
	private bool		  isDisableCanvas = true;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void Fade()
	{
		fadeImage.gameObject.SetActive(true);
		fadeImage.DOSizeDelta(new Vector2(5000, 5000), fadeDuration).SetEase(Ease.InQuad).OnComplete(() => {
			fadeImage.gameObject.SetActive(false);

			if(isDisableCanvas)
				canvas.SetActive(false);
		});
	}

	public float GetFadeDuration()
	{
		return fadeDuration;
	}
}
