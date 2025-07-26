// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterAI
{
	private float				 repathInterval = 0.0f;
	private float				 repathTimer    = 0.0f; 

	private IPathFinder			 pathFinder;
	
	private Node[,]				 grid;

	private List<Node>			 currentPath	 = new List<Node>();
	private int					 currentPathIndex;

	private Monster				 monster;

	public void Initialize(Monster monster, IPathFinder pathFinder)
    {
		this.monster	   = monster;
		this.pathFinder    = pathFinder;

		grid               = GridMapManager.Instance.GetGrid();
		repathInterval     = GameManager.Instance.GetRepathInterval();
		repathTimer        = repathInterval;
	}

	public void Initialize(Monster monster)
	{
		this.monster     = monster;

		grid             = GridMapManager.Instance.GetGrid();
		repathInterval   = GameManager.Instance.GetRepathInterval();
		repathTimer      = repathInterval;
	}

	public void UpdatePath()
	{
		if(monster.GetMonsterRole() == MonsterRole.Leader)
		{
			UpdatePathFromPathFinder();
		}
	}

	private void UpdatePathFromPathFinder()
	{
		Vector2Int startPos  = GridMapManager.Instance.WorldToNode(monster.transform.position);
		Vector2Int targetPos = GridMapManager.Instance.WorldToNode(InGameManager.Instance.GetPlayerTransform().position);

		List<Node> path = pathFinder.FindPath(startPos, targetPos, grid);

		if (path != null)
		{
			currentPath = path;
			currentPathIndex = 0;

			// Follow Monster 한테도 경로 알려주기 
		}
	}


	public Vector2Int GetTargetNodePosition()
	{
		return currentPath[currentPathIndex].nodePosition;
	}

	public void MoveNextNode()
	{
		if(currentPathIndex < currentPath.Count - 1)
			currentPathIndex++;
	}

	public bool HasPath()
    {
		return currentPath.Count > 0;
    }
}