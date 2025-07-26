public class MonsterGroupKeyGenerator 
{
	private int groupKey = 0;

	public int GetNextGroupKey()
	{
		return groupKey++;
	}
}
