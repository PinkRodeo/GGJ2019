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

	private RectTransform _transform;

	private Camera _currentCamera;

	protected void Awake()
	{
		_transform = this.transform as RectTransform;
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
		targetZoomLevel = zoomedOutDistance;
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

		_currentCamera.DOShakeRotation(1000f, 0.08f, 1, 80f, false).SetLoops(-1).SetEase(Ease.InOutCubic);

		var state = JunkerGameMode.junkerState;
		musicXComplete = state.x_Recovered ? 1f : 0f;
		musicYComplete = state.y_Recovered ? 1f : 0f;
		musicZComplete = state.z_Recovered ? 1f : 0f;
	}

	// Update is called once per frame
	protected void Update()
	{
		musicComponent.SetParameter("X Completed", musicXComplete);
		musicComponent.SetParameter("Y Completed", musicYComplete);
		musicComponent.SetParameter("Z Completed", musicZComplete);

		var scroll = Input.mouseScrollDelta.y;
		zoomOffset += scroll * -0.25f;
		zoomOffset = Mathf.Clamp(zoomOffset, -1, 1);

		zoomOffset = Mathf.MoveTowards(zoomOffset, 0f, 1f / 12f * Time.deltaTime);

		var factor = (targetZoomLevel - zoomedInDistance) / (zoomedOutDistance - zoomedInDistance);
		var variIn = GetVar(zoomedInDistanceVar);
		var variOut = GetVar(zoomedOutDistanceVar);


		var currentPos = _transform.localPosition;
		currentPos.z = -(targetZoomLevel + Mathf.Lerp(variIn, variOut, factor));
		_transform.localPosition = currentPos;
	}

	private float GetVar(Vector2 zoomVar)
	{
		if (zoomOffset < 0)
			return - Mathf.Lerp(0, zoomVar.x, zoomOffset * -1f);
		else
			return Mathf.Lerp(0, zoomVar.y, zoomOffset * 1f);
	}

	protected void FixedUpdate()
	{

		

		
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
	public Vector2 zoomedInDistanceVar = new Vector2(.5f, 2f);

	public float zoomedInDistance = 7.5f;
	public Vector2 zoomedOutDistanceVar = new Vector2(10f, 15f);
	public float zoomedOutDistance = 65f;

	private float targetZoomLevel;
	private float zoomOffset = 0f;

	public void ZoomIn()
	{
		if (b_ZoomingIn)
			return;

		b_ZoomingIn = true;
		CleanZoom();
		var oldZoom = targetZoomLevel;
		zoomTween = DOTween.To(()=> { return oldZoom; }, (float value)=> { targetZoomLevel = value; }, zoomedInDistance, 0.8f).SetEase(Ease.OutCirc);

		musicComponent.SetParameter("Dancing", 1f);
	}

	public void ZoomOut()
	{
		if (!b_ZoomingIn)
			return;

		b_ZoomingIn = false;
		CleanZoom();
		var oldZoom = targetZoomLevel;
		zoomTween = DOTween.To(() => { return oldZoom; }, (float value) => { targetZoomLevel = value; }, zoomedOutDistance, 0.8f).SetEase(Ease.OutBack);

		musicComponent.SetParameter("Dancing", 0f);
		zoomOutComponent.Play(); 


	}
}
