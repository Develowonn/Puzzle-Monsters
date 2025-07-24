// # System
using System.IO;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEditor;

// # Etc
using Newtonsoft.Json;

public class WaveEditor : EditorWindow
{
	// 생성한 윈도우 정보 저장할 변수 
	private static WaveEditor window;

	private const string   jsonPath = "Resources/Json/Waves.json";

	private List<WaveData> waves;
	private List<bool>	   waveFoldouts;

	private Vector2	       scrollPos          = Vector2.zero;
	private Color		   editorDefaultColor;

	[MenuItem("Editor/Monster Stage Editor")]
	private static void SetUp()
	{
		window = GetWindow<WaveEditor>();

		// 윈도우 제목 설정 
		window.titleContent = new GUIContent("Wave Editor");

		// 윈도우 최소/최대 크기 설정 
		window.minSize = new Vector2(450, 750);
		window.maxSize = new Vector2(1920, 1080);
	}

	private void OnEnable()
	{
		waves              = new List<WaveData>();	
		waveFoldouts       = new List<bool>();

		editorDefaultColor = GUI.backgroundColor;
		LoadWaveDataFromJson();
	}

	private void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		
		DrawAddWaveButtonGroup();
		DrawWaveList();
		DrawSaveAndRestButton();

		EditorGUILayout.EndScrollView();
	}

	private void DrawAddWaveButtonGroup()
	{
		GUILayout.BeginVertical(GUI.skin.box);

		// Label 
		GUILayout.Label("Wave Editor Window", EditorStyles.boldLabel);
		GUILayout.Space(5);

		// Add Wave Button
		if(GUILayout.Button("[+] Add Wave", GUILayout.Height(25)))
		{
			waves.Add(new WaveData());
		}
		GUILayout.EndVertical();
	}

	private void DrawWaveList()
	{
		GUILayout.BeginVertical(GUI.skin.box);
		for(int i = 0; i < waves.Count; i++)
		{
			if(i >= waveFoldouts.Count)
				waveFoldouts.Add(false);

			GUILayout.BeginVertical(GUI.skin.box);
			// Foldout
			waveFoldouts[i] = EditorGUILayout.Foldout(waveFoldouts[i], $"Wave {i}");

			// Point Spawn Datas
			if (waveFoldouts[i])
			{
				for(int j = 0;  j < waves[i].pointSpawnDatas.Length; j++)
				{
					DrawPointSpawnData(i, j);
					EditorGUILayout.Space(3);
				}
			}
			GUILayout.EndVertical();
			EditorGUILayout.Space(5);
		}
		GUILayout.EndVertical();
	}

	private void DrawPointSpawnData(int waveIndex, int pointIndex)
	{
		var pointSpawnData = waves[waveIndex].pointSpawnDatas[pointIndex];

		GUILayout.BeginVertical(GUI.skin.box);
		
		// Label
		GUILayout.Label($"Point {pointIndex}", EditorStyles.boldLabel);

		// Monster Type
		GUILayout.BeginHorizontal(GUI.skin.box);
		GUILayout.Label("- Monster Type : ");

		pointSpawnData.type = 
			(MonsterType)EditorGUILayout.EnumPopup(pointSpawnData.type);
		GUILayout.EndHorizontal();

		// Monster Count
		if(pointSpawnData.type != MonsterType.None)
		{
			if(pointSpawnData.monsterCount <= 0)
			{
				pointSpawnData.monsterCount++;
			}

			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label($"- Monster Count :	    [ {pointSpawnData.monsterCount} 마리 ]");

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("+"))
			{
				pointSpawnData.monsterCount++;
			}
			GUI.backgroundColor = editorDefaultColor;

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("-") && pointSpawnData.monsterCount > 0)
			{
				pointSpawnData.monsterCount--;
			}
			GUI.backgroundColor = editorDefaultColor;
			GUILayout.EndHorizontal();
		}
		else
		{
			if(pointSpawnData.monsterCount != 0)
				pointSpawnData.monsterCount = 0;

			EditorGUILayout.HelpBox("Monster Type 을 설정해주세요!", MessageType.Info);
		}
		GUILayout.EndVertical();
	}

	private void DrawSaveAndRestButton()
	{
		if(waves.Count > 0)
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();

			GUI.backgroundColor = Color.yellow;
			if (GUILayout.Button("Export Json"))
			{
				string json = JsonConvert.SerializeObject(waves, Formatting.Indented);
				string path = Path.Combine(Application.dataPath, jsonPath);

				File.WriteAllText(path, json);
			}

			if (GUILayout.Button("Reset Waves"))
			{
				waves.Clear();
			}

			GUI.backgroundColor = editorDefaultColor;
			GUILayout.EndHorizontal();
		}
	}

	private void LoadWaveDataFromJson()
	{
		TextAsset wavesTextAsset = Resources.Load<TextAsset>("Json/Waves");
		string    json           = wavesTextAsset.text;

		waves = JsonConvert.DeserializeObject<List<WaveData>>(json);
	}
}