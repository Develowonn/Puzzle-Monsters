// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class PathGizmosRenderer : MonoBehaviour
{
	[SerializeField]
	private bool	   isGizmos;

	private List<Node> pathList;
	private Color      gizmoColor;

	private void Awake()
	{
		pathList = new List<Node>();
	}

	public void Initialize(List<Node> pathList, Color gizmoColor)
	{
		this.pathList   = pathList;
		this.gizmoColor = gizmoColor;
	}

	private void OnDrawGizmos()
	{
		if(!isGizmos) return;

		Gizmos.color = gizmoColor;

		if (pathList != null && pathList.Count > 0)
		{
			for(int i = 0; i < pathList.Count - 1; i++)
			{
				Vector3 from = (Vector3Int)pathList[i].nodePosition;
				Vector3 to   = (Vector3Int)pathList[i + 1].nodePosition;

				Gizmos.DrawLine(from, to);
			}
		}
	}
}
