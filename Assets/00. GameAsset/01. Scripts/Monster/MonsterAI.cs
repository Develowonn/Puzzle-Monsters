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
	private Transform			 playerTransform;

	private List<Node>			 currentPath;
	private int					 currentPathIndex;

	private Monster				 monster;

	private void Start()
	{
		pathFinder         = new AStartPathFinder();

		pathGizmosRenderer = GetComponent<PathGizmosRenderer>();
		monster			   = GetComponent<Monster>();

		grid			   = GameManager.Instance.GetGrid();
		repathInterval     = GameManager.Instance.GetRepathInterval();
		playerTransform    = InGameManager.Instance.GetPlayerTransform();

		UpdatePath();
	}

	private void Update()
	{
		repathTimer += Time.deltaTime;

		if (repathTimer > repathInterval && !monster.IsMove)
		{ 
			repathTimer = 0.0f;
			UpdatePath();
		}
	}

	private void UpdatePath()
	{
		Vector2Int startPos  = GameManager.Instance.WorldToNode(transform.position);
		Vector2Int targetPos = GameManager.Instance.WorldToNode(playerTransform.position);

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
		if(currentPath == null)
		{
			return Vector2Int.zero;
		}

		if (currentPathIndex > currentPath.Count)
		{
			return currentPath[currentPath.Count - 1].nodePosition;
		}

		return currentPath[currentPathIndex].nodePosition;
	}

	public void MoveNextNode()
	{
		currentPathIndex++;
	}
}