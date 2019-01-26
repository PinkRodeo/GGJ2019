using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkerPlayer : MonoBehaviour
{
    public RectTransform rect_transform;

    private Rigidbody2D rigidBody;

    protected void Awake ()
    {
        rect_transform = this.transform as RectTransform;
        rigidBody = this.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        JunkerGameMode.instance.player = this;
    }


    public void AddVelocity(Vector2 velocity)
    {
        rigidBody.AddForce(velocity, ForceMode2D.Force);
    }
}
