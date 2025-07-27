public class MonsterGroupKeyGenerator 
{
	private int groupKey = -1;

	public int GetNextGroupKey()
	{
		return groupKey++;
	}

	public int GetGroupKey()
	{
		return groupKey;
	}
}
