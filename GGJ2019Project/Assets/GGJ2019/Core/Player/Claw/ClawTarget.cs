using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawTarget : MonoBehaviour
{
	public static string TAG = "ClawTarget";

	protected void Awake()
	{
		gameObject.tag = TAG;
	}
	
	public void OnClawConnect()
	{

	}

	public void OnClawDocked()
	{

	}
}
