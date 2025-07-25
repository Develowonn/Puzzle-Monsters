// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class MonsterPool
{
    public MonsterType  key;
    public Monster      monster;
    public int          objectCount;

    [Space(10)]
    public Transform         parentTransform;
    public Queue<GameObject> pool;
}