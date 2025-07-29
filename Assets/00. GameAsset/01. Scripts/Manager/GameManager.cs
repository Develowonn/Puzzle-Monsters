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
	private Vector2			repathIntervalOffsetRange;
	[SerializeField]
	private bool			isGroupSystem;

	private void Start()
	{
		Application.targetFrameRate = 720;
	}

	public float GetRepathInterval()
	{
		float repathInterval = this.repathInterval + Random.Range(repathIntervalOffsetRange.x, repathIntervalOffsetRange.y);
		return Mathf.Max(repathInterval, this.repathInterval - repathIntervalOffsetRange.x);
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