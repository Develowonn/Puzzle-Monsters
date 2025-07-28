// System
using System;

// Unity
using UnityEngine;

[System.Serializable]
public class Node : IComparable
{
	public Node(bool isWall, Vector2Int nodePosition)
	{
		this.isWall		  = isWall;
		this.nodePosition = nodePosition;
	}

	public bool		  isWall;
	public Node		  parentNode;
	public Vector2Int nodePosition;
	
	public int		  gCost = int.MaxValue;	// 이동했던 거리
	public int		  hCost = 0;			// |가로|+|세로| 장애물 무시하여 목표까지의 거리 
	public int		  fCost { get { return gCost + hCost; } } // F = GCost + HCost

    public int CompareTo(Node other)
    {
		if(fCost == other.fCost)
        {
			return 0;
        }

		return fCost < other.fCost ? 1 : -1;
    }
}