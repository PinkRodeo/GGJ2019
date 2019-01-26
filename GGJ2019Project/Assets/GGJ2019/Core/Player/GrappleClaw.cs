using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrappleClaw : MonoBehaviour
{
  
    public RectTransform rect_transform;
    private Rigidbody2D rigidBody;
    private bool _isEnabled = false;

    public RectTransform clawPivotL;
    public RectTransform clawPivotR;

    private Vector2 _fireDirection;

    public float AttachAnimationLength = .4f;
    protected void Awake()
    {
        rect_transform = this.transform as RectTransform;
        rigidBody = this.GetComponent<Rigidbody2D>();

        SetEnabled(false);  
    }

    // Start is called before the first frame update
    void Start()
    {
        JunkerGameMode.instance.claw = this;
    }

    // Update is called once per frame
    void Update()
    {
        var worldTargetPos = JunkerGameMode.instance.cameraManager.currentCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -JunkerGameMode.instance.cameraManager.cameraDistance));

        DebugExtension.DebugPoint(worldTargetPos, 10f, 0f, false);
        var playerPos = JunkerGameMode.instance.player.rect_transform.position;
        DebugExtension.DebugArrow(playerPos, worldTargetPos - playerPos);


        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Start move");

            //var velocity = new Vector2((worldTargetPos - playerPos).x, (worldTargetPos - playerPos).y);
            //JunkerGameMode.instance.player.AddVelocity(velocity);

            FireOff(new Vector2((worldTargetPos - playerPos).x, (worldTargetPos - playerPos).y).normalized, 6f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetEnabled(false);
        }
    }

    public void FireOff(Vector2 direction, float velocity)
    {
        if (_isEnabled)
        {
            Debug.LogWarning("Tried to fire off the GrappleClaw while it was still active");
            SetEnabled(false);
            //return;
        }

        _fireDirection = direction;
        SetClosedVisual(0f);
        SetEnabled(true);

        rect_transform.position = JunkerGameMode.instance.player.transform.position;
        rect_transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * 180f / Mathf.PI - 90f);

        rigidBody.AddForce(direction * velocity, ForceMode2D.Impulse);
    }

    public void SetEnabled(bool p_isEnabled)
    {
        _isEnabled = p_isEnabled;


        if (p_isEnabled == false)
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
        }

        rigidBody.simulated = p_isEnabled;

    }
    

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        SetEnabled(false);

        Debug.Log("Triggered the claw");

        if (collision.otherCollider.tag == ClawTarget.TAG || true)
        {
            Debug.Log("Hit a valid claw target");

            SetEnabled(false);
            //SetClosedVisual(1f);
            DOTween.To((float value) => { SetClosedVisual(value); }, 0f, 1f, AttachAnimationLength).SetEase(Ease.InExpo);

            var currentPos = rect_transform.position;
            var targetPos = collision.GetContact(0).point - _fireDirection*0.2f;


            DOTween.To(() => { return currentPos; }, (Vector3 vec) => { rect_transform.position = vec; }, targetPos, AttachAnimationLength).SetEase(Ease.InExpo).onComplete += ()=>
            {
                Debug.Log("Attached the claw");
            };
        }
    }

    public void SetClosedVisual(float alpha)
    {
        SetClawPivotAngle(Mathf.Lerp(0, 62f, alpha));
    }

    private void SetClawPivotAngle(float angle)
    {
        clawPivotL.localRotation = Quaternion.Euler(0, 0, -angle);
        clawPivotR.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
