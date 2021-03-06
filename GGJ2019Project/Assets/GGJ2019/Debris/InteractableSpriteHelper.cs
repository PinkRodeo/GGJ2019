﻿using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
public class InteractableSpriteHelper : SpaceSpriteHelper
{
	public override void SetupMaterial()
	{
		if (material == null)
		{
			var rend = GetComponent<SpriteRenderer>();

			if (spaceShader == null)
			{
				return;
			}

			material = new Material(spaceShader);
			rend.material = material;
		}

		material.SetFloat("_LightDirection", lightDirection);
		material.SetColor("_HighlightColor", highlightColor);
		material.SetFloat("_HighlightSize", highlightSize);

		material.SetFloat("_BlurAmount", blurAmount);

		if (bRandomizeValue)
		{
			material.SetFloat("_LevelAdjustB", Mathf.Max(0f, Random.Range(brightness*0.9f, brightness *1.05f)));
			material.SetFloat("_LevelAdjustC", Mathf.Max(0f, Random.Range(contrast *0.9f, contrast *1.05f)));
		}
		else
		{
			material.SetFloat("_LevelAdjustB", brightness);
			material.SetFloat("_LevelAdjustC", contrast);
		}

		if (bRandomizeRotation)
		{
			transform.parent.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
		}
		else
		{
			transform.parent.rotation = Quaternion.Euler(0, 0, rotation);
		}


	}
}
