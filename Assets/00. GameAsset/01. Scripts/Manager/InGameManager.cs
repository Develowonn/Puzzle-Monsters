// # Unity
using UnityEngine;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; }

	[SerializeField]
	private Transform  playerTransform;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
	}

	public Transform GetPlayerTransform()
	{
		return playerTransform;
	}
}
