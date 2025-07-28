using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _00_Lobby : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.EndGame();
    }
}
