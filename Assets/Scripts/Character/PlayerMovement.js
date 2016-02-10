private var controller :CharacterController;
controller = gameObject.GetComponent(CharacterController);

private var moveDirection = Vector3.zero;

// The following variables are defined once, in Start()
private var forward = Vector3.zero;
private var right = Vector3.zero;
private var left = Vector3.left;
private var down = Vector3.zero;

// The followig variable is updated each frame
private var currForward;

private var rotateVec;

private var horizontalInput;
private var verticalInput;

private var targetDirection;

public var rotateBy = 200;

function Start()
{
	//forward is the blue axis of the transform in world space. Or, (0,0,1), into the computer screen.
	forward = transform.forward;
	//right is a different vector that is determined based on forward. Or (1,0,0).
	right = Vector3(1, 0, 0);
	left = Vector3(-1,0,0);
	down = Vector3(0,-1,0);

	rotateVec = Vector3.zero;
}

function FixedUpdate()
{

	//Get some good old inputs
	horizontalInput = Input.GetAxisRaw("Horizontal");
	verticalInput = Input.GetAxisRaw("Vertical");
	//Debug.Log(horizontalInput * right + verticalInput * forward);

	//Take both inputs and combine them
	targetDirection = horizontalInput * right + verticalInput * forward;

	//Perform some rotations based on the targetDirection
	if ( targetDirection.x > 0 ) rotateVec = Vector3.RotateTowards(transform.forward, forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
	else if ( targetDirection.x < 0 ) rotateVec = Vector3.RotateTowards(transform.forward, -forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
        if (rotateVec != Vector3.zero) transform.rotation = Quaternion.LookRotation(rotateVec);

	//If we aren't touching the ground, factor in some downward motion
	if ((controller.collisionFlags & CollisionFlags.Below)==0)
	{
		// Debug.Log("FLOATING");
		targetDirection = targetDirection + down;
	}

	//Actually apply the movement
	controller.Move(targetDirection * Time.deltaTime * 5);

	if (Input.GetKeyDown(KeyCode.P)){
		transform.rotation.y = 180;
	}


}

@script RequireComponent(CharacterController)
