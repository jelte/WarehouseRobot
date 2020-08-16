using System.Collections;
using UnityEngine;

public class Robot : MonoBehaviour, IGridObject
{
	[Header("Speeds")]
	public float rotationSpeed = 1f;
	public float movementSpeed = 1f;
	public float liftSpeed = 1f;

	public Vector3Int Position { get; set; }
	private float rotation;

	public Vector3Int Forward
	{
		get => Position + forward;
		set => forward = value;
	}

	public bool HasCrate => crate != null;

	private Vector3Int forward;
	private Crate crate;

	private Coroutine executionCoroutine;

	private Vector3Int? targetPosition;
	private float? targetRotation;
	private bool lifting = false;
	private bool dropping = false;
	private float elapsedTime = 0f;

	public void MoveTo(Vector3Int target)
	{
		targetPosition = target;
	}

	public bool FinishedMove()
	{
		return !targetPosition.HasValue;
	}

	public void RotateToward(float angle)
	{
		if (rotation == angle)
		{
			return;
		}
		targetRotation = angle;
	}

	public bool FinishedRotating()
	{
		return !targetRotation.HasValue;
	}
	
	public void Grab(Crate crate)
	{
		this.crate = crate;
		crate.transform.parent = transform;
		lifting = true;
	}

	public bool CrateLifted()
	{
		return !lifting;
	}

	public Crate Drop()
	{
		dropping = crate != null;
		return crate;
	}

	public bool CrateDropped()
	{
		return !dropping;
	}

	void Update()
	{
		if (TryRotate() || TryMove() || TryLiftCrate() || TryDropCrate())
		{
			elapsedTime += Time.deltaTime;
		}
	}
g
	private bool TryRotate()
	{
		if (!targetRotation.HasValue)
		{
			return false;
		}

		float ratio = Mathf.Clamp01(elapsedTime / (1/rotationSpeed));
		transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(rotation, targetRotation.Value, ratio), 0);
		if (elapsedTime > 1f/rotationSpeed)
		{
			rotation = targetRotation.Value;
			targetRotation = null;
			elapsedTime = 0f;
			return false;
		}

		return true;
	}

	private bool TryMove()
	{
		if (!targetPosition.HasValue)
		{
			return false;
		}

		float ratio = Mathf.Clamp01(elapsedTime / (1/movementSpeed));
		transform.position = Vector3.Lerp(Position, targetPosition.Value, ratio);
		if (elapsedTime > 1f/movementSpeed)
		{
			Position = targetPosition.Value;
			targetPosition = null;
			elapsedTime = 0f;
			return false;
		}

		return true;
	}

	private bool TryLiftCrate()
	{
		if (!lifting || crate == null)
		{
			return false;
		}
		
		lifting = Lift(Forward, Position + Vector3.up, 1/liftSpeed);
		return lifting;
	}

	private bool Lift(Vector3 source, Vector3 target, float liftDuration)
	{
		float ratio = Mathf.Clamp01(elapsedTime / liftDuration);
		crate.transform.position = Vector3.Lerp(source, target, ratio);
		if (elapsedTime > liftDuration)
		{
			elapsedTime = 0f;
			return false;
		}

		return true;
	}

	private bool TryDropCrate()
	{
		if (!dropping || crate == null)
		{
			return false;
		}

		dropping = Lift(Position + Vector3.up, Forward, 1/liftSpeed);
		if (!dropping)
		{
			crate = null;
		}
		return dropping;
	}
}