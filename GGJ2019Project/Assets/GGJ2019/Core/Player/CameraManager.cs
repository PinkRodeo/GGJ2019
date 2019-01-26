using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

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
	}

	void Start()
	{
		_currentCamera = Camera.main;

		JunkerGameMode.instance.cameraManager = this;
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
