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
			// FCost가 가장 낮은 노드를 선택 
			// FCost가 같을 때 HCost가 더 낮은 쪽을 우선으로 한다.
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

			// 목표 노드에 도달하면 경로를 역추적해서 반환
			// 경로를 TargetNode 부터 거꾸로 저장해 역추적을 해야 StartNode ~ TargetNode 순으로 저장됨
			if(currentNode == targetNode)
			{
				return ReconstructPath(startNode, targetNode);
			}

			// 인접한 노드들 검사 (이웃 노드들을 검사한다.)
			foreach(Node neighborNode in GetNeighbors(currentNode, grid))
			{
				// 이웃한 노드가 (벽)노드 또는 closeList에 포함중일 때 건너뛴다.
				if(neighborNode.isWall || closeList.Contains(neighborNode))
					continue;

				int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighborNode);

				// 더 짧은 경로로 도달하거나 처음 방문하는 노드면 갱신
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

		// 경로를 찾지 못했을 경우 빈 리스트 반환
		return new List<Node>(); 
	}

	private int GetManhattanDistance(Node nodeA, Node nodeB)
	{
		// X와 Y 좌표 차이의 절재값을 계산
		int distanceX = Mathf.Abs(nodeA.nodePosition.x - nodeB.nodePosition.x);
		int distanceY = Mathf.Abs(nodeA.nodePosition.y - nodeB.nodePosition.y);

		// 거리를 계산
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

		// 데이터를 뒤집어 startNode ~ targetNode 순으로 Node를 저장 
		path.Reverse();
		return path;
	}

	private List<Node> GetNeighbors(Node currentNode, Node[,] grid)
	{
		List<Node> neighbors = new List<Node>();

		// 방향 순서 ( 위 -> 아래 -> 왼쪽 -> 오른쪽 )
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
