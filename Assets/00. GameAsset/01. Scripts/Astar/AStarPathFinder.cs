// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
	private readonly Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

	private PriorityQueue<Node> openQueue;
	private HashSet<Node>		openSet;
	private HashSet<Node>		closeSet;

	private int sizeX;
	private int sizeY;

	public List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos, Node[,] grid)
	{
		sizeX = grid.GetLength(0);
		sizeY = grid.GetLength(1);

		Node startNode  = grid[startPos.x, startPos.y];
		Node targetNode = grid[targetPos.x, targetPos.y];	

		openQueue  = new PriorityQueue<Node>(grid.Length);
		openQueue.Enqueue(startNode);

		openSet  = new HashSet<Node>() { startNode };
		closeSet = new HashSet<Node>();

		while(openQueue.Count > 0)
		{
			// FCost�� ���� ���� ��带 ���� 
			// FCost�� ���� �� HCost�� �� ���� ���� �켱���� �Ѵ�.
			Node currentNode = openQueue.Dequeue();
			openSet.Remove(currentNode);
			closeSet.Add(currentNode);

			// ��ǥ ��忡 �����ϸ� ��θ� �������ؼ� ��ȯ
			// ��θ� TargetNode ���� �Ųٷ� ������ �������� �ؾ� StartNode ~ TargetNode ������ �����
			if(currentNode == targetNode)
			{
				return ReconstructPath(startNode, targetNode);
			}

			// ������ ���� �˻� (�̿� ������ �˻��Ѵ�.)
			foreach(Node neighborNode in GetNeighbors(currentNode, grid))
			{
				// �̿��� ��尡 (��)��� �Ǵ� closeList�� �������� �� �ǳʶڴ�.
				if(neighborNode.isWall || closeSet.Contains(neighborNode))
					continue;

				int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighborNode);

				// �� ª�� ��η� �����ϰų� ó�� �湮�ϴ� ���� ����
				if(moveCost < neighborNode.gCost || !openSet.Contains(neighborNode))
				{
					neighborNode.gCost      = moveCost;
					neighborNode.hCost      = GetManhattanDistance(neighborNode, targetNode);
					neighborNode.parentNode = currentNode;

					if (!openSet.Contains(neighborNode))
					{
						openQueue.Enqueue(neighborNode);
						openSet.Add(neighborNode);
					}
				}
			}
		}

		// ��θ� ã�� ������ ��� �� ����Ʈ ��ȯ
		return new List<Node>(); 
	}

	private int GetManhattanDistance(Node nodeA, Node nodeB)
	{
		// X�� Y ��ǥ ������ ���簪�� ���
		int distanceX = Mathf.Abs(nodeA.nodePosition.x - nodeB.nodePosition.x);
		int distanceY = Mathf.Abs(nodeA.nodePosition.y - nodeB.nodePosition.y);

		// �Ÿ��� ���
		return distanceX + distanceY;
	}

	private List<Node> ReconstructPath(Node startNode, Node targetNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;

		while(currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parentNode;
		}

		// �����͸� ������ startNode ~ targetNode ������ Node�� ���� 
		path.Reverse();
		return path;
	}

	private List<Node> GetNeighbors(Node currentNode, Node[,] grid)
	{
		List<Node> neighbors = new List<Node>();

		// ���� ���� ( �� -> �Ʒ� -> ���� -> ������ )
		foreach(Vector2Int direction in directions)
		{
			Vector2Int checkDirection = currentNode.nodePosition + direction;

			if(checkDirection.x >= 0 && checkDirection.x < sizeX &&
			   checkDirection.y >= 0 && checkDirection.y < sizeY)
			{
				neighbors.Add(grid[checkDirection.x, checkDirection.y]);
			}
		}

		return neighbors;
	}
}
