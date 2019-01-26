using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawTarget : MonoBehaviour
{
	public static string TAG = "ClawTarget";

	public delegate void ClawEvent();

	public ClawEvent OnClawConnect;
	public ClawEvent OnShipDocked;
	public ClawEvent OnShipUnDocked;

	protected void Awake()
	{
		gameObject.tag = TAG;
	}
	
	public void ClawConnect()
	{
		if (OnClawConnect != null)
			OnClawConnect.Invoke();
	}

	public void ShipDocked()
	{
		if (OnShipDocked != null)
			OnShipDocked.Invoke();
	}

	public void ShipUnDocked()
	{
		if (OnClawConnect != null)
			OnShipUnDocked();
	}
}
