// # Unity
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMapManager : MonoBehaviour
{
    public static GridMapManager Instance { get; private set; }

	[SerializeField]
	private Tilemap		wallTileMap;

	[Header("Map Range")]
	[SerializeField]
	private Vector2Int  bottomLeft;
	[SerializeField]
	private Vector2Int  topRight;

	private Node[,]		grid;
	private int			sizeX;
	private int			sizeY;

	public bool[,]		WallMap { get; private set; }

	private void Awake()
    {
        if(Instance == null)
            Instance = this;

		// 0 ~ 29 까지 총 타일이 30개 존재해 + 1 을 더해 Size를 30으로 만들어준다.
		sizeX = topRight.x - bottomLeft.x + 1;
		sizeY = topRight.y - bottomLeft.y + 1;

		InitializeWallMap();
		InitializeGrid();
	}

	private void InitializeWallMap()
	{
		WallMap = new bool[sizeX, sizeY];

		for (int x = wallTileMap.cellBounds.xMin; x < wallTileMap.cellBounds.xMax; x++)
		{
			for (int y = wallTileMap.cellBounds.yMin; y < wallTileMap.cellBounds.yMax; y++)
			{
				Vector3Int tilePosition = new Vector3Int(x, y, 0);

				if (wallTileMap.HasTile(tilePosition))
				{
					WallMap[x + (-wallTileMap.cellBounds.xMin), y + (-wallTileMap.cellBounds.yMin)] = true;
				}
			}
		}
	}

	private void InitializeGrid()
	{
		grid = new Node[sizeX, sizeY];

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				grid[x, y] = new Node(WallMap[x, y], new Vector2Int(x, y));
			}
		}
	}

	public Node[,] GetGrid()
	{
		return grid;
	}
	public Vector2Int WorldToNode(Vector3 pos)
	{
		int x = Mathf.Clamp(Mathf.RoundToInt(pos.x), 0, sizeX - 1);
		int y = Mathf.Clamp(Mathf.RoundToInt(pos.y), 0, sizeY - 1);

		return new Vector2Int(x, y);
	}

	public bool IsWallAtPosition(Vector3 pos)
	{
		Vector2Int nodePos = WorldToNode(pos);

		// 범위를 벗어날 경우 벽으로 판단 
		if (nodePos.x < 0 || nodePos.x > sizeX - 1
		|| nodePos.y < 0 || nodePos.y > sizeY - 1)
			return true;

		return grid[nodePos.x, nodePos.y].isWall;
	}
}
