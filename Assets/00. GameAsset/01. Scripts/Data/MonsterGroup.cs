// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MonsterGroup : MonoBehaviour
{
	public int			 groupID;

	public Monster		 leader;
	public List<Monster> followers;

	public MonsterGroup()
	{
		followers = new List<Monster>();
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

	public void OnLeaderDead()
	{
		if(followers.Count > 0)
		{
			groupID = -1;
			leader  = followers[0];
			followers.RemoveAt(0);
			leader.PromoteToLeader();
		}
	}

	public void Cleanup()
	{
		leader = null;
		followers.Clear();
	}
}
