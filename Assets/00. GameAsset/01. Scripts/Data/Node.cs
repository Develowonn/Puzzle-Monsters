// System
using System;

// Unity
using UnityEngine;

[System.Serializable]
public class Node : IComparable<Node>
{
	public Node(bool isWall, Vector2Int nodePosition)
	{
		this.isWall		  = isWall;
		this.nodePosition = nodePosition;

		gCost = int.MaxValue;
		hCost = 0;
	}

	public bool		  isWall;
	public Node		  parentNode;
	public Vector2Int nodePosition;
	
	public int		  gCost; // �̵��ߴ� �Ÿ�
	public int		  hCost; // |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ� 
	public int		  fCost { get { return gCost + hCost; } } // F = GCost + HCost

	public int CompareTo(Node other)
    {
		if (fCost == other.fCost)
		{
			return hCost < other.hCost ? 1 : -1;
		}
		return fCost < other.fCost ? 1 : -1;
    }
}