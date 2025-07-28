// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Header("Game")]
	[SerializeField]
	private bool			isGamePlaying;
	[SerializeField]
	private int				gameStartDelay;

	[Header("Path Finder")]
	[SerializeField]
	private float			repathInterval;
	[SerializeField]
	private bool			isGroupSystem;

	public float GetRepathInterval()
	{
		return repathInterval;
	}

	public int GetGameStartDelay()
	{
		return gameStartDelay;
	}

	public void SetGroupSystemEnabled(bool isEnabled)
	{
		isGroupSystem = isEnabled;
	}

	public bool IsGroupSystem()
	{
		return isGroupSystem;
	}

	public bool IsGamePlaying()
	{
		return isGamePlaying;
	}

	public void StartGame()
	{
		isGamePlaying = true;
	}

	public void EndGame()
	{
		isGamePlaying = false;
	}
}