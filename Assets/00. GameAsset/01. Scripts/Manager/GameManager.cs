// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Header("Path Finder")]
	[SerializeField]
	private PathFinderType	pathFinderType;
	[SerializeField]
	private float			repathInterval;

	public float GetRepathInterval()
	{
		return repathInterval;
	}

	public IPathFinder GetPathFinder()
	{
		return pathFinderType switch
		{
			PathFinderType.AStar    => new AStartPathFinder(),
			PathFinderType.BFS      => null,
			PathFinderType.Dijkstra => null,
			_ => null
		};
	}
}