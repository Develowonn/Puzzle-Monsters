// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public interface IPathFinder 
{
	public abstract List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos, Node[,] grid);
}
