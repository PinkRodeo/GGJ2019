using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JunkerState
{
	public E_Level currentLevel;

	public bool RiftEstablished = false;
	public bool x_Recovered = false;
	public bool y_Recovered = false;
	public bool z_Recovered = false;
}

public class JunkerGameMode : MonoBehaviour
{
	public static JunkerGameMode instance;

	public static JunkerState junkerState = new JunkerState();

	public JunkerPlayer player
	{
		get
		{
			return _player;
		}
		set
		{
			if (_player != null)
			{
				Debug.LogError("Trying to assign a new JunkerPlayer when one already existed.");
			}

			_player = value;
		}
	}

	public GrappleClaw claw
	{
		get
		{
			return _claw;
		}
		set
		{
			if (_claw != null)
			{
				Debug.LogError("Trying to assign a new GrappleClaw when one already existed.");
			}

			_claw = value;
		}
	}

	public CameraManager cameraManager
	{
		get
		{
			return _cameraManager;
		}
		set
		{
			if (_cameraManager != null)
			{
				Debug.LogError("Trying to assign a new cameraManager when one already existed.");
			}

			_cameraManager = value;
		}
	}

	public EventManager eventManager
	{
		get
		{
			return EventManager.instance;
		}
	}

	private JunkerPlayer _player;
	private GrappleClaw _claw;
	private CameraManager _cameraManager;


	protected void Awake ()
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

	protected void Start ()
	{
		OnStartLevel();
	}

	public void OnStartLevel()
	{
		junkerState.currentLevel = SceneLoader.instance.currentLevel;

		SceneLoader.instance.OnChangeLevel += (E_Level currentLevel, E_Level p_NextLevel) =>
		{
			if (p_NextLevel != E_Level.MainMenu)
			{
				switch (p_NextLevel)
				{
					case E_Level.X:
						junkerState.x_Recovered = true;
						break;
					case E_Level.Y:
						junkerState.y_Recovered = true;
						break;
					case E_Level.Z:
						junkerState.z_Recovered = true;
						break;
					case E_Level.Rift:
						junkerState.RiftEstablished = true;
						break;
					default:
						break;
				}
			}
		};
	}

	protected void Update ()
	{
		
	}
}
