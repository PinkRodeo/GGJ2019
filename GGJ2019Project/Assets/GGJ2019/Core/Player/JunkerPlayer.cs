using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class JunkerPlayer : MonoBehaviour
{
    public RectTransform rect_transform;

	public Rigidbody2D rigidBody
	{
		get
		{
			return _rigidBody;
		}
	}

    private Rigidbody2D _rigidBody;
	public FixedJoint2D fixedJoint;


    protected void Awake ()
    {
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

        AddVelocity(input * 10f);
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

		DebugExtension.DebugPoint(fixedJoint.connectedAnchor, Color.yellow, 5f, 5f, false);

		fixedJoint.connectedBody.AddTorque(50f, ForceMode2D.Impulse);

	}
}
