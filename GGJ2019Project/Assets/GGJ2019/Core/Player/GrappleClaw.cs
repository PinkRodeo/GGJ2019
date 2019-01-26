using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using FMODUnity;

public enum E_GrappleState
{
	Retracted,
	Targeting,
	Travelling,
	Connecting,
	Retracting,
	Docked,
	Releasing,
}

public class GrappleClaw : MonoBehaviour
{

	public E_GrappleState grappleState
	{
		get
		{
			return _grappleState;
		}
		set
		{
			var oldValue = _grappleState;
			var inputState = value;
			_grappleState = value;

			switch (value)
			{
				case E_GrappleState.Retracted:
					JunkerGameMode.instance.cameraManager.ZoomOut();
					currentTarget = null;

					foreach (var tween in _animationTweens)
					{
						tween.Kill();
					}
					_animationTweens.Clear();

					_trailRenderer.Clear();
					_trailRenderer.emitting = false;

					rigidBody.velocity = Vector2.zero;
					rigidBody.angularVelocity = 0f;

					rigidBody.simulated = false;

					_parentConstraint.enabled = true;
					fixedJoint.connectedBody = null;
					fixedJoint.enabled = false;
					break;
				case E_GrappleState.Targeting:
					_parentConstraint.enabled = false;

					rigidBody.velocity = Vector2.zero;
					rigidBody.angularVelocity = 0f;

					rigidBody.simulated = false;

					break;
				case E_GrappleState.Travelling:
					_trailRenderer.Clear();
					_trailRenderer.emitting = true;

					rigidBody.simulated = true;

					break;
				case E_GrappleState.Connecting:
					rigidBody.simulated = true;

					sfx_hitTarget.Play();
					break;
				case E_GrappleState.Retracting:
					rigidBody.simulated = true;

					sfx_reelTargetIn.Play();

					break;
				case E_GrappleState.Docked:
					JunkerGameMode.instance.cameraManager.ZoomIn();

					fixedJoint.connectedBody = null;
					fixedJoint.enabled = false;
					rigidBody.simulated = false;

					sfx_reelTargetIn.Stop();
					sfx_hitReeledInTarget.Play();

					_parentConstraint.enabled = true;

					break;
				case E_GrappleState.Releasing:
					JunkerGameMode.instance.cameraManager.ZoomOut();

					if (JunkerGameMode.instance.player.fixedJoint.connectedBody != null)
					{
						JunkerGameMode.instance.player.Detach();
					}

					grappleState = E_GrappleState.Retracted;
					break;
				default:

					break;
			}
		}
	}

	public ClawTarget currentTarget { get; private set; }

	public RectTransform rect_transform;
	public FixedJoint2D fixedJoint;

	private E_GrappleState _grappleState = E_GrappleState.Retracted;


	private Rigidbody2D rigidBody;
	private bool _isEnabled = false;

	public RectTransform clawPivotL;
	public RectTransform clawPivotR;

	private Vector2 _fireDirection;

	public float AttachAnimationLength = .4f;
	private Quaternion _currentRotation;
	private Quaternion _targetRotation;

	private TrailRenderer _trailRenderer;
	private ParentConstraint _parentConstraint;

	private List<Tweener> _animationTweens;

	public float preferredDistance;
	public float tightness = 300f;
	public float damping = 200f;

	public StudioEventEmitter sfx_shootHook;
	public StudioEventEmitter sfx_hitTarget;
	public StudioEventEmitter sfx_reelTargetIn;
	public StudioEventEmitter sfx_hitReeledInTarget;

	protected void Awake()
	{

		_animationTweens = new List<Tweener>();

		rect_transform = this.transform as RectTransform;
		rigidBody = this.GetComponent<Rigidbody2D>();
		fixedJoint = this.GetComponent<FixedJoint2D>();

		_parentConstraint = this.GetComponent<ParentConstraint>();
		_trailRenderer = this.GetComponent<TrailRenderer>();

		SetCollisionEnabled(false);
	}

	// Start is called before the first frame update
	void Start()
	{
		JunkerGameMode.instance.claw = this;
	}

	// Update is called once per frame
	void Update()
	{
		var player = JunkerGameMode.instance.player;
		var worldTargetPos = JunkerGameMode.instance.cameraManager.currentCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -JunkerGameMode.instance.cameraManager.cameraDistance));

		DebugExtension.DebugPoint(worldTargetPos, 10f, 0f, false);
		var playerPos = JunkerGameMode.instance.player.rect_transform.position;
		DebugExtension.DebugArrow(playerPos, worldTargetPos - playerPos);


		if (Input.GetMouseButtonDown(0) && grappleState == E_GrappleState.Retracted)
		{

			FireOff(new Vector2((worldTargetPos - playerPos).x, (worldTargetPos - playerPos).y).normalized, 6f);
		}

		if (Input.GetMouseButtonDown(1))
		{

			if (grappleState == E_GrappleState.Docked)
			{
				Debug.Log("Released from docking");
				grappleState = E_GrappleState.Releasing;
			}
			else
			{
				grappleState = E_GrappleState.Retracted;

			}
		}

		switch (grappleState)
		{
			case E_GrappleState.Retracted:

				break;
			case E_GrappleState.Targeting:

				break;
			case E_GrappleState.Travelling:

				break;
			case E_GrappleState.Connecting:

				// dead facing code
				//var dist = new Vector2((rect_transform.position - playerPos).x, (rect_transform.position - playerPos).y);
				//var dir = dist.normalized;

				//var targetRotation = Angle(dir);
				//var currentRotation = player.rigidBody.rotation;

				//player.rigidBody.MoveRotation(Mathf.MoveTowardsAngle(currentRotation, targetRotation, 200f));
				break;
			case E_GrappleState.Retracting:
				var hookPosition = rect_transform.position;


				var distance = new Vector2((rect_transform.position - playerPos).x, (rect_transform.position - playerPos).y);
				var direction = distance.normalized;
				DebugExtension.DebugArrow(playerPos, distance, Color.black, 0f, false);

				distance -= distance.normalized * preferredDistance;
				
				// TODO: rigidbody for target
				// var force = -distance * tightness - (damping * (JunkerGameMode.instance.player.rigidBody.velocity - b.rigidbody2D.velocity));
				var force = distance * tightness - (damping * (JunkerGameMode.instance.player.rigidBody.velocity));

				JunkerGameMode.instance.player.rigidBody.AddForce(force * Time.deltaTime, ForceMode2D.Force);
				//b.rigidbody2D.AddForce(force * (Time.deltaTime * -1f), ForceMode2D.Force);



				break;
			case E_GrappleState.Docked:

				break;
			case E_GrappleState.Releasing:

				break;
			default:

				break;
		}

		UpdateTrail();
	}

	public static float Angle(Vector2 p_vector2)
	{
		if (p_vector2.x < 0)
		{
			return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
		}
		else
		{
			return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
		}
	}

	private void UpdateTrail()
	{
		var _trailPositions = new Vector3[_trailRenderer.positionCount];
		var indexes = _trailRenderer.GetPositions(_trailPositions);

		var clawPos = rect_transform.position;
		var playerPos = JunkerGameMode.instance.player.rect_transform.position;

		float totalIndexes = indexes;


		for (int i = 0; i < indexes; i++)
		{
			float index = i;
			index /= indexes;
			float factor = Mathf.Pow(((index - 0.5f) * 2f), 2f);

			if (factor < 0.5f)
			{
				_trailPositions[i] = Vector3.Lerp(_trailPositions[i], playerPos, factor*0.5f+0.1f);
			}
			else
			{
				_trailPositions[i] = Vector3.Lerp(_trailPositions[i], clawPos, factor*0.5f+0.1f);

			}
		}

		_trailRenderer.SetPositions(_trailPositions);
	}

	public void FireOff(Vector2 p_direction, float p_velocity)
	{
		if (grappleState != E_GrappleState.Retracted)
		{
			Debug.LogWarning("Tried to fire off the GrappleClaw while it was still active");
			return;

			//grappleState = E_GrappleState.Retracted;

		}


		grappleState = E_GrappleState.Targeting;

		sfx_shootHook.Play();

		_fireDirection = p_direction;
		SetClosedVisual(0f);
		rect_transform.position = JunkerGameMode.instance.player.transform.position;
		rect_transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(p_direction.y, p_direction.x) * 180f / Mathf.PI - 90f);

		grappleState = E_GrappleState.Travelling;
		SetCollisionEnabled(true);
		rigidBody.AddForce(p_direction * p_velocity, ForceMode2D.Impulse);
	}

	public void SetCollisionEnabled(bool p_isEnabled)
	{
		_isEnabled = p_isEnabled;


		if (p_isEnabled == false)
		{
			rigidBody.velocity = Vector2.zero;
			rigidBody.angularVelocity = 0f;
		}

		rigidBody.simulated = p_isEnabled;

	}


	protected void OnCollisionEnter2D(Collision2D p_collision)
	{
		if (grappleState != E_GrappleState.Travelling)
			return;
		

		Debug.Log("Triggered the claw");

		if (p_collision.collider.tag != ClawTarget.TAG)
		{
			Debug.Log("Not valid target");
			Debug.Log(p_collision.collider.tag);
			return;
		}

		grappleState = E_GrappleState.Connecting;
		currentTarget = p_collision.collider.GetComponent<ClawTarget>();

		SetCollisionEnabled(false);
		_animationTweens.Add(DOTween.To((float value) => { SetClosedVisual(value); }, 0f, 1f, AttachAnimationLength).SetEase(Ease.InExpo));

		fixedJoint.connectedBody = currentTarget.GetComponent<Rigidbody2D>();
		fixedJoint.enabled = true;

		var currentPos = rect_transform.position;
		var targetPos = p_collision.GetContact(0).point - _fireDirection * 0.2f;

		//DebugExtension.DebugArrow(collision.GetContact(0).point, collision.GetContact(0).normal * 10f, Color.red, 10f, true);

		this._currentRotation = rect_transform.rotation;
		var hitNormal = p_collision.GetContact(0).normal;
		var normalRotation = Quaternion.Euler(0, 0, Mathf.Atan2(hitNormal.y, hitNormal.x) * 180f / Mathf.PI - 90f);
		this._targetRotation = Quaternion.Lerp(_currentRotation, normalRotation, 0.5f);

		_animationTweens.Add(DOTween.To((float value) => { SetClosedVisual(value); }, 0f, 1f, AttachAnimationLength).SetEase(Ease.InExpo));
		var tween = DOTween.To(() => { return currentPos; }, (Vector3 vec) => { rect_transform.position = vec; }, targetPos, AttachAnimationLength).SetEase(Ease.InExpo);

		tween.onUpdate += () =>
		{
			tween.target = p_collision.GetContact(0).point - _fireDirection * 0.2f;
		};

		tween.onComplete += () =>
		{
			Debug.Log("Attached the claw");
			_trailRenderer.emitting = false;
			grappleState = E_GrappleState.Retracting;
		};

		_animationTweens.Add(tween);
		
	}

	public void ArrivedAtTarget()
	{
		grappleState = E_GrappleState.Docked;
	}

	public void SetClosedVisual(float p_alpha)
	{
		SetClawPivotAngle(Mathf.Lerp(0, 62f, p_alpha));
	}

	public void SetNormalAttenuation(float p_alpha)
	{
		rect_transform.rotation = Quaternion.Lerp(_currentRotation, _targetRotation, p_alpha);
	}

	private void SetClawPivotAngle(float p_angle)
	{
		clawPivotL.localRotation = Quaternion.Euler(0, 0, -p_angle);
		clawPivotR.localRotation = Quaternion.Euler(0, 0, p_angle);
	}
}
