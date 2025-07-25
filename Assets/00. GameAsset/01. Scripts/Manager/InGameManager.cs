// # Unity
using UnityEngine;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; }

	[SerializeField]
	private Transform  playerTransform;
	[SerializeField]
	private int		   aliveMonsterCount;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
	}

	public Transform GetPlayerTransform()
	{
		return playerTransform;
	}

	public int GetAliveMonsterCount()
    {
		return aliveMonsterCount;
    }

	public void IncreaseAliveMonsterCount()
    {
		aliveMonsterCount++;
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
    }

	public void DecreaseAliveMonsterCount()
    {
		aliveMonsterCount--;
		InGameUIManager.Instance.UpdateAliveMonsterCountUI();
	}
}
