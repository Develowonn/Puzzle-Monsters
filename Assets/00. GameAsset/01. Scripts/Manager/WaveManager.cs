// # System
using System;
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
	}

	private void ChangeNextWave()
	{
		// ó������ ���������� ���̺긦 ����
		currentWaveIndex++;

		// ������ ���̺갡 ������ waveIndexs �� ���� �ٲ�
		// ���̺���� ������ �Ȱ��� ������� �ʰ� ��
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
}
