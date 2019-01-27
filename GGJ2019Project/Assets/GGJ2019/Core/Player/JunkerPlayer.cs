using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class JunkerPlayer : MonoBehaviour
{
    public RectTransform rect_transform;
	public SpriteRenderer art;
	public TrailRenderer portalTrail;

	public Rigidbody2D rigidBody
	{
		get
		{
			return _rigidBody;
		}
	}

    private Rigidbody2D _rigidBody;
	public FixedJoint2D fixedJoint;

	public float rotationForce = 150f;
	public float initialRotationImpulse = 10f;
	public float detachImpulse = 50f;

	protected void Awake ()
    {
		portalTrail.emitting = false;
		rect_transform = this.transform as RectTransform;
        _rigidBody = this.GetComponent<Rigidbody2D>();
		fixedJoint = this.GetComponent<FixedJoint2D>();

	}

    // Start is called before the first frame update
    protected void Start()
    {
        JunkerGameMode.instance.player = this;
    }

    public void AddVelocity(Vector2 velocity)
    {
        _rigidBody.AddForce(velocity, ForceMode2D.Force);
    }

    protected void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        var input = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            input.y = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            input.x = -1f;
        }

		if (Input.GetKey(KeyCode.LeftShift))
		{
			AddVelocity(input * 50f);

		}
		else
		{
			AddVelocity(input * 15f);

		}

		if (fixedJoint.connectedBody != null)
		{
			fixedJoint.connectedBody.AddTorque(rotationForce * Time.fixedDeltaTime, ForceMode2D.Force);



			
			//DebugExtension.DebugPoint(anchorPoint, Color.cyan, 10f, 0f, false);

		}
	}

	protected void OnCollisionEnter2D(Collision2D p_collision)
	{
		//Debug.Log("Triggered the claw");

		if (p_collision.collider.tag != ClawTarget.TAG)
		{
			return;
		}
		var hitClawTarget = p_collision.collider.GetComponent<ClawTarget>();
		if (JunkerGameMode.instance.claw.currentTarget != hitClawTarget)
		{
			return;
		}

		JunkerGameMode.instance.claw.ArrivedAtTarget();

		fixedJoint.connectedBody = hitClawTarget.GetComponent<Rigidbody2D>();
		fixedJoint.enabled = true;
		fixedJoint.connectedBody.AddTorque(initialRotationImpulse, ForceMode2D.Impulse);

		DebugExtension.DebugPoint(fixedJoint.connectedAnchor, Color.yellow, 5f, 5f, false);


	}

	public void Detach()
	{
		var otherPosition = fixedJoint.connectedBody.worldCenterOfMass;
		var ownPosition = rigidBody.worldCenterOfMass;
		var direction = (ownPosition - otherPosition).normalized;

		fixedJoint.enabled = false;
		fixedJoint.connectedBody = null;

		rigidBody.AddForce(direction * detachImpulse, ForceMode2D.Impulse);


		DebugExtension.DebugArrow(otherPosition, direction * 10f, Color.yellow, 0f, false);
	}
}
