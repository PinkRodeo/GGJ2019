using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class PortalShot : MonoBehaviour
{
	private ClawTarget _clawTarget;

	private PortalTarget _portalTarget;

	protected void Awake()
	{
		_portalTarget = GetComponentInChildren<PortalTarget>();
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

		JunkerGameMode.instance.player.rect_transform.DOBlendableMoveBy(_portalTarget.transform.position, length * 0.03f, false).SetEase(Ease.OutExpo).onComplete += () =>
		{
			player.rigidBody.simulated = true;
		};


	}
}
