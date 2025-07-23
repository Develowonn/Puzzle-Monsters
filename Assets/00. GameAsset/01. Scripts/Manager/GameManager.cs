// # System
using System.Collections.Generic;


// # Unity
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
	[SerializeField]
	private Tilemap		wallTileMap;

	[Header("Map Range")]
	[SerializeField]
	private Vector2Int  bottomLeft;
	[SerializeField]
	private Vector2Int  topRight;

	[Header("Path Finder")]
	[SerializeField]
	private float		repathInterval;

	public bool[,]	    WallMap { get; private set; }

	private Node[,]     grid;
	private int		    sizeX;
	private int         sizeY;

	public override void Awake()
	{
		base.Awake();

		// 0 ~ 29 까지 총 타일이 30개 존재해 + 1 을 더해 Size를 30으로 만들어준다.
		sizeX = topRight.x - bottomLeft.x + 1;
		sizeY = topRight.y - bottomLeft.y + 1;

		InitializeWallMap();
		InitializeGrid();
	}

	private void InitializeWallMap()
	{
		WallMap = new bool[sizeX, sizeY];

		for(int x = wallTileMap.cellBounds.xMin; x < wallTileMap.cellBounds.xMax; x++)
		{
			for(int y = wallTileMap.cellBounds.yMin; y < wallTileMap.cellBounds.yMax; y++) 
			{
				Vector3Int tilePosition = new Vector3Int(x, y, 0);

				if (wallTileMap.HasTile(tilePosition))
				{
					WallMap[x - wallTileMap.cellBounds.xMin, y - wallTileMap.cellBounds.yMin] = true;
				}
			}
		}
	}

	private void InitializeGrid()
	{
		grid = new Node[sizeX, sizeY];

		for(int x = 0; x < sizeX; x++)
		{
			for(int y = 0; y < sizeY; y++)
			{
				grid[x, y] = new Node(WallMap[x, y], new Vector2Int(x, y));
			}
		}
	}

	public Node[,] GetGrid()
	{
		return grid;
	}

	public float GetRepathInterval()
	{
		return repathInterval;
	}

	public Vector2Int WorldToNode(Vector3 pos)
	{
		int x = Mathf.Clamp(Mathf.RoundToInt(pos.x), 0, sizeX - 1);
		int y = Mathf.Clamp(Mathf.RoundToInt(pos.y), 0, sizeY - 1);

		return new Vector2Int(x, y);
	}

	public Vector3 NodeToWorld(Vector2Int pos)
	{
		// 0.5f 를 더하는 이유는 노드의 중심에서 움직일 수 있게 
		return new Vector3(pos.x * 1.0f, pos.y * 1.0f, 0);
	}
}