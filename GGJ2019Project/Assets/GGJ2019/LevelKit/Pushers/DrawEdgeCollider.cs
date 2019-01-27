using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D)), ExecuteInEditMode()]
public class DrawEdgeCollider : MonoBehaviour
{
	protected EdgeCollider2D edgeCollider2D;

	protected void Awake()
	{
		edgeCollider2D = GetComponent<EdgeCollider2D>();

	}

	protected void Start()
	{
		this.enabled = false;
	}
	///*
	// Update is called once per frame
	void Update()
    {
		var trans = this.transform;

		var radius = edgeCollider2D.edgeRadius;
		var points = edgeCollider2D.points;
		for (int i = 0; i < edgeCollider2D.pointCount-1; i++)
		{
			DebugExtension.DebugCapsule(trans.TransformPoint(points[i]), trans.TransformPoint(points[i + 1]), Color.green, radius, 0f, false);
		}
    }
	//*/
}
