// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterGroupManager : Singleton<MonsterGroupManager>
{
	// Monster Group 을 재사용하기 위한 큐 
	private Queue<MonsterGroup>			  monsterGroupPool       = new Queue<MonsterGroup>(); 
	private Dictionary<int, MonsterGroup> monsterGroupDictionary = new Dictionary<int, MonsterGroup>();
	
	private void RegisterMonsterGroup(int groupID)
	{
		MonsterGroup monsterGroup = monsterGroupPool.Count > 0 
								  ? monsterGroupPool.Dequeue()
								  : new MonsterGroup();

		monsterGroupDictionary.Add(groupID, monsterGroup);
		monsterGroupDictionary[groupID].Initialize(groupID);
	}

	public void RegisterToMonsterGroup(int groupID, Monster monster)
	{
		if (!monsterGroupDictionary.ContainsKey(groupID))
			RegisterMonsterGroup(groupID);

		monsterGroupDictionary[groupID].AddMonster(monster);
	}

	public void ReleaseGroup(int groupID)
	{
		if (!monsterGroupDictionary.ContainsKey(groupID))
			return;

		monsterGroupDictionary[groupID].Cleanup();
		monsterGroupPool.Enqueue(monsterGroupDictionary[groupID]);
		monsterGroupDictionary.Remove(groupID);
	}

	public MonsterGroup GetMonsterGroup(int groupID)
	{
		return monsterGroupDictionary[groupID];
	}
}
