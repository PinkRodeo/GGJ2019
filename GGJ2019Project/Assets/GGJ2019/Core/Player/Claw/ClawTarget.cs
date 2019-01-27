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

	public bool tweakSprite = false;

	public Color spriteTint = Color.green;
	public Color lightTint = Color.green;

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

		if (tweakSprite)
		{
			var spriteHelper = GetComponentInChildren<SpaceSpriteHelper>();

			if (spriteHelper != null)
			{
				if (lightTint != Color.green)
					spriteHelper.highlightColor = lightTint;

				if (spriteTint != Color.green)
				{
					spriteHelper.GetComponent<SpriteRenderer>().color = spriteTint;
				}

				spriteHelper.SetupMaterial();
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
