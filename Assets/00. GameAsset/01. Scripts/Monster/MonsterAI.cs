// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterAI
{
	private IPathFinder			 pathFinder;
	
	private Node[,]				 grid;

	private List<Node>			 currentPath	   = new List<Node>();
	private int					 currentPathIndex;
	private Queue<Vector2Int>	 leaderNodeHistory = new Queue<Vector2Int>();

	private Monster				 monster;

	public void Initialize(Monster monster, IPathFinder pathFinder)
    {
		this.monster	   = monster;
		this.pathFinder    = pathFinder;
		grid               = GridMapManager.Instance.GetGrid();
	}

	public void Initialize(Monster monster)
	{
		this.monster     = monster;
		grid             = GridMapManager.Instance.GetGrid();
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
			currentPath      = path;
			currentPathIndex = 0;
		}
	}

	public void RecordLeaderNode(Vector2Int node)
	{
		leaderNodeHistory.Enqueue(node);
	}

	public Vector2Int? GetNextLeaderNode()
	{
		if(leaderNodeHistory.Count > 0)
		{
			return leaderNodeHistory.Dequeue();
		}
		return null;
	}

	public List<Node> GetPath() 
	{ 
		return currentPath;
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

	public bool HasLeaderNodeHistory()
	{
		return leaderNodeHistory.Count > 0;
	}
}