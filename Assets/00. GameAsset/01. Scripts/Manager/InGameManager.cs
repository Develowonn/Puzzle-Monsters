// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; }

	[SerializeField]
	private Transform     playerTransform;

	private List<Monster> aliveMonsterList;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;

		aliveMonsterList = new List<Monster>();
	}

	private void Update()
	{
		InputDebugModeKey();
	}

	private void InputDebugModeKey()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			InGameUIManager.Instance.OnDebugMode();
		}
	}

    public Transform GetPlayerTransform()
	{
		return playerTransform;
	}

	public int GetAliveMonsterCount()
    {
		return aliveMonsterList.Count;
    }

	public void AddAliveMonster(Monster monster)
    {
		aliveMonsterList.Add(monster);
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
    }

	public void RemoveAliveMonster(Monster monster)
    {
		aliveMonsterList.Remove(monster);
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
	}
}
