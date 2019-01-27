using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader instance;
	
	protected void Awake()
	{
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

	private void ClearScene()
	{
		var count = SceneManager.sceneCount;
		for (int i = 0; i < count; i++)
		{
			//SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex);
		}
	}

	public void LoadMenuScene()
	{
		ClearScene();
		SceneManager.LoadScene("Menu", LoadSceneMode.Single);
	}

	public void LoadSceneRift()
	{
		ClearScene();
		SceneManager.LoadScene("L_Rift", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Rift_Art", LoadSceneMode.Additive);
	}

	public void LoadSceneX()
	{
		ClearScene();
		SceneManager.LoadScene("L_X", LoadSceneMode.Single);
		SceneManager.LoadScene("L_X_Art", LoadSceneMode.Additive);
	}

	public void LoadSceneY()
	{
		ClearScene();
		SceneManager.LoadScene("L_Y", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Y_Art", LoadSceneMode.Additive);
	}

	public void LoadSceneZ()
	{
		ClearScene();
		SceneManager.LoadScene("L_Z", LoadSceneMode.Single);
		SceneManager.LoadScene("L_Z_Art", LoadSceneMode.Additive);
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
		else if (Input.GetKeyDown(KeyCode.Alpha5))
			LoadSceneZ();
	}
}
