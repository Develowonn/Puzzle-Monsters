// Unity
using UnityEngine;

[System.Serializable]
public class Node
{
	public Node(bool isWall, Vector2Int nodePosition)
	{
		this.isWall		  = isWall;
		this.nodePosition = nodePosition;
	}

	public bool		  isWall;
	public Node		  parentNode;
	public Vector2Int nodePosition;
	
	public int		  gCost;	// �̵��ߴ� �Ÿ�
	public int		  hCost;	// |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ� 
	public int		  fCost { get { return gCost + hCost; } } // F = GCost + HCost
}