using DG.Tweening;
using FMODUnity;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

	private StudioEventEmitter musicComponent;
	public StudioEventEmitter zoomOutComponent;

	public float cameraDistance
	{
		get
		{
			return _transform.position.z;
		}
	}

	public Camera currentCamera
	{
		get
		{
			return _currentCamera;
		}
	}

	private Transform _transform;

	private Camera _currentCamera;

	protected void Awake()
	{
		_transform = this.transform;
		musicComponent = GetComponent<StudioEventEmitter>();
	}
	
	[Range(0f, 1f)]
	public float musicXComplete = 0f;
	[Range(0f, 1f)]
	public float musicYComplete = 0f;
	[Range(0f, 1f)]
	public float musicZComplete = 0f;

	void Start()
	{
		_currentCamera = Camera.main;

		JunkerGameMode.instance.cameraManager = this;

		SceneLoader.instance.OnChangeLevel += (E_Level currentLevel, E_Level p_NextLevel) =>
		{
			musicComponent.Stop();
		};

		var levelData = SceneLoader.instance.FindLevelData(SceneLoader.instance.currentLevel);
		if (levelData == null)
		{
			return;
		}
		musicComponent.Event = levelData.levelMusic;

		musicComponent.Play();
	}

	// Update is called once per frame
	void Update()
	{
		musicComponent.SetParameter("X Completed", musicXComplete);
		musicComponent.SetParameter("Y Completed", musicYComplete);
		musicComponent.SetParameter("Z Completed", musicZComplete);
	}

	private Tween zoomTween;
	private void CleanZoom()
	{

		if (zoomTween != null)
		{
			zoomTween.Kill();
			zoomTween = null;
		}
	}


	private bool b_ZoomingIn = false;
	public float zoomedInDistance = 7.5f;
	public float zoomedOutDistance = 24.5f;
	
	public void ZoomIn()
	{
		if (b_ZoomingIn)
			return;

		b_ZoomingIn = true;
		CleanZoom();
		zoomTween = _transform.DOMoveZ(-zoomedInDistance, 0.8f, false).SetEase(Ease.OutBack);

		musicComponent.SetParameter("Dancing", 1f);
	}

	public void ZoomOut()
	{
		if (!b_ZoomingIn)
			return;

		b_ZoomingIn = false;
		CleanZoom();
		zoomTween = _transform.DOMoveZ(-zoomedOutDistance, 0.8f, false).SetEase(Ease.OutBack);

		musicComponent.SetParameter("Dancing", 0f);
		zoomOutComponent.Play(); 


	}
}
