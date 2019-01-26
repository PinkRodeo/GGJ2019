using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    protected void Awake ()
    {
        rect_transform = this.transform as RectTransform;
        _rigidBody = this.GetComponent<Rigidbody2D>();
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
}
