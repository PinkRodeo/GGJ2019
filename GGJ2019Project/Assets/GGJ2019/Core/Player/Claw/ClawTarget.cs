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

	public bool bEnableLights = false;
	public List<SpriteRenderer> lights = new List<SpriteRenderer>();

	protected void OnValidate()
	{
		var sprites = GetComponentsInChildren<SpriteRenderer>();
		lights.Clear();
		foreach (var sprite in sprites)
		{
			if (sprite.tag == "Blinker")
			{
				lights.Add(sprite);
				sprite.enabled = bEnableLights;
			}
		}

		gameObject.tag = TAG;

	}

	public void SetLightsVisible(bool p_visible)
	{
		foreach (var light in lights)
		{
			light.enabled = p_visible;
		}
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
		if (OnShipUnDocked != null)
			OnShipUnDocked();
	}

	
}
