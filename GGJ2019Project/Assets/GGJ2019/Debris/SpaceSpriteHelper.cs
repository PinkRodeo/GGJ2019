using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
public class SpaceSpriteHelper : MonoBehaviour
{
	private Shader spaceShader;

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

	private Material material;

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

	private void SetupMaterial()
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

		material.SetFloat("_LevelAdjustB", brightness);
		material.SetFloat("_LevelAdjustC", contrast);

		transform.rotation = Quaternion.Euler(0,0, rotation);

	}
}
