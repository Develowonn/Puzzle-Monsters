[System.Serializable]
public class WaveData 
{
	public PointSpawnData[] pointSpawnDatas;

	public WaveData()
	{
		pointSpawnDatas = new PointSpawnData[Constants.towerMaxCount];

		for(int i = 0; i < pointSpawnDatas.Length; i++)
		{
			pointSpawnDatas[i] = new PointSpawnData();
		}
	}
}
