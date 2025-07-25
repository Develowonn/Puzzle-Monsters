// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
	private float				 repathInterval = 0.0f;
	private float				 repathTimer    = 0.0f; 

	private IPathFinder			 pathFinder;
	
	private Node[,]				 grid;
	private PathGizmosRenderer   pathGizmosRenderer;

	private List<Node>			 currentPath	 = new List<Node>();
	private int					 currentPathIndex;

	private Monster				 monster;

    private void Start()
    {
		pathGizmosRenderer = GetComponent<PathGizmosRenderer>();

		grid			= GridMapManager.Instance.GetGrid();
		repathInterval  = GameManager.Instance.GetRepathInterval();
		repathTimer     = repathInterval;
	}

    private void Update()
	{
		if (pathFinder == null) return;

		repathTimer += Time.deltaTime;
		if (repathTimer >= repathInterval && !monster.IsMove)
		{ 
			repathTimer = 0.0f;
			UpdatePath();
		}
	}

	public void Initialize(Monster monster, IPathFinder pathFinder)
    {
		this.monster	   = monster;
		this.pathFinder    = pathFinder;
	}

	private void UpdatePath()
	{
		Vector2Int startPos  = GridMapManager.Instance.WorldToNode(transform.position);
		Vector2Int targetPos = GridMapManager.Instance.WorldToNode(InGameManager.Instance.GetPlayerTransform().position);

		List<Node> path = pathFinder.FindPath(startPos, targetPos, grid);

        if (path != null)
        {
            currentPath      = path;
			currentPathIndex = 0;
			
			pathGizmosRenderer.Initialize(path, Color.red);
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