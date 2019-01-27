using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
public class SpaceSpriteHelper : MonoBehaviour
{
	protected Shader spaceShader;

	[Range(0, 360f)]
	public float rotation = 0f;

	[Range(0, 2f)]
	public float brightness = 0.5f;
	[Range(0, 1f)]
	public float contrast = 0.5f;

	[Range(0, 6.28318530717958f)]
	public float lightDirection = .04f;
	public Color highlightColor = new Color(1,1,1,0.5f);
	[Range(0, 0.1f)]
	public float highlightSize = .1f;

	[Range(0, 0.06f)]
	public float blurAmount = 0f;

	public bool bRandomizeValue = false;
	public bool bRandomizeRotation = false;

	protected Material material;

	protected void OnValidate()
	{
#if UNITY_EDITOR
		spaceShader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/GGJ2019/Debris/S_SpaceSprite.shader", typeof(Shader));
#endif
		SetupMaterial();
	}

	protected void Awake()
	{
		//SetupMaterial();
	}

	public virtual void SetupMaterial()
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
			material.SetFloat("_LevelAdjustB", Mathf.Max(0f, Random.Range(brightness * 0.95f, brightness * 1.05f)));
			material.SetFloat("_LevelAdjustC", Mathf.Max(0f, Random.Range(contrast * 0.95f, contrast * 1.05f)));
		}
		else
		{
			material.SetFloat("_LevelAdjustB", brightness);
			material.SetFloat("_LevelAdjustC", contrast);
		}

		if (bRandomizeRotation)
		{
			transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
			var rend = GetComponent<SpriteRenderer>();
		}
		else
		{
			transform.rotation = Quaternion.Euler(0, 0, rotation);
		}


	}
}
