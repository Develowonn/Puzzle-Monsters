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
	
	public int		  gCost = int.MaxValue;	// �̵��ߴ� �Ÿ�
	public int		  hCost = 0;			// |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ� 
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