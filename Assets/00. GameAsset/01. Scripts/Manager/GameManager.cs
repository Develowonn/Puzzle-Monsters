// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Header("Path Finder")]
	[SerializeField]
	private IPathFinder		currentPathFinder;
	[SerializeField]
	private float			repathInterval;

	public float GetRepathInterval()
	{
		return repathInterval;
	}

	public IPathFinder GetPathFinder()
    {
		return currentPathFinder;
    }
}