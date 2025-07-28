// # System
using System;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; }

	public  bool		  IsDebugMode { get; private set; }

	[SerializeField]
	private Transform     playerTransform;

	private List<Monster> aliveMonsterList;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;

		aliveMonsterList = new List<Monster>();
	}

	private void Start()
	{
		PlayIntro().Forget();
	}

	private void Update()
	{
		InputDebugModeKey();
	}

	private async UniTask PlayIntro() 
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
		FadeManager.Instance.Fade();

		await UniTask.Delay(TimeSpan.FromSeconds(FadeManager.Instance.GetFadeDuration()));
		await InGameUIManager.Instance.PlayGameStartTimer(GameManager.Instance.GetGameStartDelay());
		GameManager.Instance.StartGame();
	}

	private void InputDebugModeKey()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			InGameUIManager.Instance.OnDebugMode();
			IsDebugMode = !IsDebugMode;

			foreach(var monster in aliveMonsterList)
            {
				monster.EnableLineRender();
            }
		}
	}

    public Transform GetPlayerTransform()
	{
		return playerTransform;
	}

	public int GetAliveMonsterCount()
    {
		return aliveMonsterList.Count;
    }

	public void AddAliveMonster(Monster monster)
    {
		aliveMonsterList.Add(monster);
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
    }

	public void RemoveAliveMonster(Monster monster)
    {
		aliveMonsterList.Remove(monster);
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
	}
}
