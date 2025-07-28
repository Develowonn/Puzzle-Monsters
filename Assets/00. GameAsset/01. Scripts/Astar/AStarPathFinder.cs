// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
	private List<Node> openList;
	private List<Node> closeList;

	private int sizeX;
	private int sizeY;

	public List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos, Node[,] grid)
	{
		sizeX = grid.GetLength(0);
		sizeY = grid.GetLength(1);

		Node startNode  = grid[startPos.x, startPos.y];
		Node targetNode = grid[targetPos.x, targetPos.y];	

		openList  = new List<Node>() { startNode };
		closeList = new List<Node>();

		while(openList.Count > 0)
		{
			// FCost�� ���� ���� ��带 ���� 
			// FCost�� ���� �� HCost�� �� ���� ���� �켱���� �Ѵ�.
			Node currentNode = openList[0];
			for(int i = 1; i < openList.Count; i++)
			{
				if (openList[i].fCost < currentNode.fCost ||
				   (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
				{
					currentNode = openList[i];
				}
			}
			openList.Remove(currentNode);
			closeList.Add(currentNode);

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
				if(neighborNode.isWall || closeList.Contains(neighborNode))
					continue;

				int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighborNode);

				// �� ª�� ��η� �����ϰų� ó�� �湮�ϴ� ���� ����
				if(moveCost < neighborNode.gCost || !openList.Contains(neighborNode))
				{
					neighborNode.gCost      = moveCost;
					neighborNode.hCost      = GetManhattanDistance(neighborNode, targetNode);
					neighborNode.parentNode = currentNode;

					if (!openList.Contains(neighborNode))
						openList.Add(neighborNode);
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
		Vector2Int[] directions = {
			Vector2Int.up, 
			Vector2Int.down, 
			Vector2Int.left, 
			Vector2Int.right,
		};

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
