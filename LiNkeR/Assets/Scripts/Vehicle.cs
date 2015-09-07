using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Vehicle : MonoBehaviour {

	[SerializeField] public int playerID = 0;
	[SerializeField] float health = 100f;
	[SerializeField] float speed;
	[SerializeField] float maxAcceleration;
	[SerializeField] float minAcceleration;
	[SerializeField] float accelerationSpeed = 0.02f;
	[SerializeField] float slowDownMultiplyer = 0.8f;
	[SerializeField] float bounciness = 100f;
	public float boostSpeed = 1.0f;
	public float boostTimer = 1.0f;
	public float boostCounter = 0.0f;
	public float terrainMultiplyer = 1.0f;
	bool isAccelerating = false;
	float acceleration;
	Vector2 inputDirection;
	Rigidbody2D rigid;

	GamePadState prevState;
	GamePadState currState;

	void Start()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate ()
	{
		SetInputStates();

		CalculateAcceleration();

		CalculateBoosts();

		SetRotation();

		ApplyForces();
	}

	void SetInputStates()
	{
		prevState = currState;
		currState = GamePad.GetState((PlayerIndex)playerID);
		if (!currState.IsConnected)
		{
			//print("Invalid Controller: " + playerID);
			return;
		}

		inputDirection = new Vector2(currState.ThumbSticks.Left.X, currState.ThumbSticks.Left.Y);
	}

	void CalculateAcceleration()
	{
		if (inputDirection.magnitude != 0)
		{
			acceleration += inputDirection.magnitude * accelerationSpeed;
			isAccelerating = true;
		}
		else
		{
			acceleration -= 0.02f;
			isAccelerating = false;
		}

		if (acceleration > maxAcceleration)
			acceleration = maxAcceleration;
		else if (acceleration < minAcceleration)
			acceleration = minAcceleration;
	}

	void CalculateBoosts()
	{
		boostCounter += Time.deltaTime;
		if(boostCounter >= boostTimer)
		{
			boostSpeed = 1.0f;
		}
	}

	void SetRotation()
	{
		float angle = Mathf.Atan2(-inputDirection.y, -inputDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	void ApplyForces()
	{
		rigid.AddForce(inputDirection * speed * acceleration * terrainMultiplyer * boostSpeed);

		if (!isAccelerating)
		{
			rigid.AddForce(-rigid.velocity * slowDownMultiplyer);
		}
	}

	public void SetTerrainModifier(float value)
	{
		terrainMultiplyer = value;
	}

	public void SetBoostSpeed(float boost, float timer)
	{
		boostSpeed = boost;
		boostTimer = timer;
		boostCounter = 0.0f;
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Player" || col.gameObject.tag == "Wall")
		{
			Vector2 vecDiff = transform.position - col.transform.position;
			rigid.AddForce(vecDiff * bounciness);
		}
		else if(col.gameObject.tag == "Bullet")
		{
			health -= 1;
			Destroy(col.gameObject);
		}
	}
}
