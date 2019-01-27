using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class PortalShot : MonoBehaviour
{
	public E_Level targetLevel = E_Level.None;
	
	public Color portalColor;
	private ClawTarget _clawTarget;

	private PortalTarget _portalTarget;

	protected void Awake()
	{
		_portalTarget = GetComponentInChildren<PortalTarget>();
		var pos = _portalTarget.transform.position;
		pos.z = 0f;
		_portalTarget.transform.position = pos;

		_clawTarget = GetComponent<ClawTarget>();

		if (_clawTarget == null)
		{
			Debug.LogError("Not a valid portal");
			return;
		}

		_clawTarget.OnShipUnDocked += OnShipLeave;
	}

	protected void OnShipLeave()
	{
		Debug.Log("Doing leave");

		var player = JunkerGameMode.instance.player;
		var length = (_portalTarget.transform.position - player.rect_transform.position).magnitude;
		player.rigidBody.velocity = Vector2.zero;
		player.rigidBody.angularVelocity = 0f;
		player.rigidBody.simulated = false;

		player.art.material.SetFloat("_FlowSpeed", 0.5f);
		player.art.color = portalColor;
		player.portalTrail.emitting = true;

		var currentPos = player.rect_transform.position;
		var tween = DOTween.To(() => { return currentPos; }, (Vector3 vec) => { player.rect_transform.position = vec; }, _portalTarget.transform.position, length * 0.03f).SetEase(Ease.InOutCubic);
		Camera.main.DOShakePosition(0.6f, 3, 10, 90f, true);

		float seconds = 0f;
		bool startedFade = false;

		tween.onUpdate += () =>
		{
			if (targetLevel != E_Level.None)
			{
				seconds += Time.deltaTime;
				if (seconds > 1.5f && startedFade == false)
				{
					Camera.main.DOFarClipPlane(0f, 1f);
					Camera.main.backgroundColor = Color.black;
				}
				

				if (seconds > 2.5f)
				{
					tween.Kill();
					switch (targetLevel)
					{
						case E_Level.Rift:
							SceneLoader.instance.LoadSceneRift();
							break;
						case E_Level.X:
							SceneLoader.instance.LoadSceneX();
							break;
						case E_Level.Y:
							SceneLoader.instance.LoadSceneY();
							break;
						case E_Level.Z:
							SceneLoader.instance.LoadSceneZ();
							break;
						case E_Level.MainMenu:
							SceneLoader.instance.LoadSceneMainMenu();
							break;
					}
				}
			}
			

			player.rigidBody.simulated = false;
			player.rigidBody.velocity = Vector2.zero;
			player.rigidBody.angularVelocity = 0f;
		};

		tween.onComplete += () =>
		{
			player.portalTrail.emitting = false;
			player.rigidBody.simulated = true;
			player.rigidBody.velocity = Vector2.zero;
			player.rigidBody.angularVelocity = 0f;

			player.art.material.SetFloat("_FlowSpeed", 0f);
			player.art.color = Color.white;
		};


	}
}
