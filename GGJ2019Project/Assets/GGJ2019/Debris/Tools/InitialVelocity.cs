using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InitialVelocity : MonoBehaviour
{
	private Rigidbody2D rigidBody;

	public Vector2 direction;
	[Range(0f, 100f)]
	public float force = 10f;

	[Range(0f, 1f)]
	public float randomFactor = 0.1f;
	protected void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
	}

	protected void Start()
	{
		var normalizedDir = direction.normalized;


		rigidBody.AddForce(normalizedDir * force * Random.Range(1f-randomFactor, 1f), ForceMode2D.Impulse);
	}
}
