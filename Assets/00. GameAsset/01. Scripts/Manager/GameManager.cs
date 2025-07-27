// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Header("Game")]
	[SerializeField]
	private bool			isGamePlaying;

	[Header("Path Finder")]
	[SerializeField]
	private PathFinderType	pathFinderType;
	[SerializeField]
	private float			repathInterval;
	[SerializeField]
	private bool			isGroupSystem;

	public float GetRepathInterval()
	{
		return repathInterval;
	}

	public void SetPathFinderType(PathFinderType pathFinderType)
	{
		this.pathFinderType = pathFinderType;
	}

	public void SetGroupSystemEnabled(bool isEnabled)
	{
		isGroupSystem = isEnabled;
	}

	public IPathFinder CreatePathFinder()
	{
		return pathFinderType switch
		{
			PathFinderType.AStar    => new AStartPathFinder(),
			PathFinderType.BFS      => null,
			PathFinderType.Dijkstra => null,
			_ => null
		};
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