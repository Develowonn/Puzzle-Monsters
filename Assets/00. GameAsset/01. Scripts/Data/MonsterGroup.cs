// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterGroup
{
	public int				  groupID;

	public Monster			  leader;
	public List<Monster>	  followers;

	public MonsterGroup()
	{
		followers       = new List<Monster>();
	}

	public void Initialize(int groupID)
	{
		this.groupID = groupID;
	}							

	public void AddMonster(Monster monster)
	{
		if(leader == null)
		{
			leader = monster;
			leader.AssignRole(MonsterRole.Leader);
		}
		else
		{
			followers.Add(monster);
			monster.AssignRole(MonsterRole.Follower);
		}
	}

	public void RecordPathToFollowers(Vector2Int node)
	{
		foreach(Monster monster in followers)
		{ 
			monster.GetMonsterAI().RecordLeaderNode(node);
		}
	}

	public void Cleanup()
	{
		leader = null;
		followers.Clear();
	}
}
