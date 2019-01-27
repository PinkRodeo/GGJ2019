using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_Level
{
	MainMenu,
	Intro,
	Rift,
	X,
	Y,
	Z,
	None,
}

[System.Serializable]
public class LevelMusicData
{
	public E_Level level;
	[EventRef]
	public String levelMusic = "";
}

public class SceneLoader : MonoBehaviour
{
	public delegate void ChangeLevel(E_Level p_CurrentLevel, E_Level p_NextLevel);
	public delegate void EnterLevel(E_Level p_CurrentLevel);

	public ChangeLevel OnChangeLevel;
	public ChangeLevel OnChangeLevelOnce;
	public EnterLevel OnEnterLevel;
	public EnterLevel OnEnterLevelOnce;

	public static SceneLoader instance;

	public LevelMusicData[] levelMusicData;

	public E_Level currentLevel { get; private set; }

	protected void Awake()
	{
		currentLevel = E_Level.None;

		if (instance != null)
		{
			GameObject.Destroy(this);
			return;
		}
		else
		{
			instance = this;
			GameObject.DontDestroyOnLoad(this);
		}
	}

	private void ClearScene(E_Level p_NextLevel)
	{
		if (OnChangeLevel != null)
		{
			OnChangeLevel.Invoke(currentLevel, p_NextLevel);
		}
		if (OnChangeLevelOnce != null)
		{
			OnChangeLevelOnce.Invoke(currentLevel, p_NextLevel);
			OnChangeLevelOnce = null;
		}


		currentLevel = p_NextLevel;
	}

	private void Enter()
	{
		if (OnEnterLevel != null)
		{
			OnEnterLevel.Invoke(currentLevel);
		}
		if (OnEnterLevelOnce != null)
		{
			OnEnterLevelOnce.Invoke(currentLevel);
			OnEnterLevelOnce = null;
		}
	}

	public void LoadMenuScene()
	{
		ClearScene(E_Level.MainMenu);
		SceneManager.LoadScene("Menu", LoadSceneMode.Single);

		Enter();
	}

	public void LoadSceneIntro()
	{
		ClearScene(E_Level.Intro);
		SceneManager.LoadScene("IntroScene", LoadSceneMode.Single);

		Enter();
	}

	public void LoadSceneRift()
	{
		ClearScene(E_Level.Rift);
		SceneManager.LoadScene("L_Rift", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Rift_Art", LoadSceneMode.Additive);
		SceneManager.LoadScene("L_Rift_Level", LoadSceneMode.Additive);

		Enter();
	}

	public void LoadSceneX()
	{
		ClearScene(E_Level.X);

		SceneManager.LoadScene("L_X", LoadSceneMode.Single);
		SceneManager.LoadScene("L_X_Art", LoadSceneMode.Additive);

		Enter();
	}

	public void LoadSceneY()
	{
		ClearScene(E_Level.Y);

		SceneManager.LoadScene("L_Y", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Y_Art", LoadSceneMode.Additive);

		Enter();
	}

	public void LoadSceneZ()
	{
		ClearScene(E_Level.Z);

		SceneManager.LoadScene("L_Z", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Z_Art", LoadSceneMode.Additive);

		Enter();
	}

	public void LoadSceneMainMenu()
	{
		ClearScene(E_Level.MainMenu);

		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

		Enter();
	}

	protected void Update()
	{
		if (!Application.isEditor && !Input.GetKey(KeyCode.L))
			return;

		if (Input.GetKeyDown(KeyCode.Alpha1))
			LoadSceneRift();
		else if (Input.GetKeyDown(KeyCode.Alpha2))
			LoadSceneX();
		else if (Input.GetKeyDown(KeyCode.Alpha3))
			LoadSceneY();
		else if (Input.GetKeyDown(KeyCode.Alpha4))
			LoadSceneZ();
		else if (Input.GetKeyDown(KeyCode.Alpha6))
			LoadSceneMainMenu();
	}

	public LevelMusicData FindLevelData(E_Level p_level)
	{
		foreach (var levelData in levelMusicData)
		{
			if (levelData.level == p_level)
				return levelData;
		}

		return null;
	}
}
