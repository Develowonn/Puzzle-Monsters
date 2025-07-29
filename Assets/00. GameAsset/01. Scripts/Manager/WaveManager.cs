// # System
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Etc
using Newtonsoft.Json;


public class WaveManager : Singleton<WaveManager>
{
	[SerializeField]
	private List<WaveData>	waveDatas;

	private int[]			waveIndexs;
	private int				currentWaveIndex;

	protected override void Awake()
	{
		base.Awake();

		InitializeWaveDatas();
		InitializeWaveIndexs();
	}

    private void InitializeWaveDatas()
	{
		waveDatas = new List<WaveData>();
		LoadWaveDatasFromJson();
	}

	private void InitializeWaveIndexs()
	{
		waveIndexs = new int[waveDatas.Count];
		for (int i = 0; i < waveIndexs.Length; i++)
		{
			waveIndexs[i] = i;
		}
	}

	private void LoadWaveDatasFromJson()
	{
		TextAsset waveDataTextAsset = Resources.Load<TextAsset>(Constants.Path.wavesJson);
		string waveDataJson = waveDataTextAsset.text;

		waveDatas = JsonConvert.DeserializeObject<List<WaveData>>(waveDataJson);
	}

	public void SetNextWaveData()
	{
		ChangeNextWave();
		InGameUIManager.Instance.UpdateWaveUI();
	}

	private void ChangeNextWave()
	{
		// 처음에는 순차적으로 웨이브를 실행
		currentWaveIndex++;

		// 마지막 웨이브가 끝나면 waveIndexs 의 값을 바꿔
		// 웨이브들의 패턴이 똑같이 실행되지 않게 함
		if (currentWaveIndex > waveDatas.Count - 1)
		{
			currentWaveIndex = 0;
			waveIndexs = Utils.GetRandomIndexs(waveDatas.Count);
		}
	}

	public WaveData GetCurrentWaveData()
	{
		return waveDatas[currentWaveIndex];
	}

	public void Restart()
	{
		for (int i = 0; i < waveIndexs.Length; i++)
		{
			waveIndexs[i] = i;
		}
		currentWaveIndex = 0;
	}
}
