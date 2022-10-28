using UnityEngine;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 4.0f;
	[Tooltip("Rotation speed of the character")]
	public float RotationSpeed = 1.0f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;
	[Tooltip("Rotation the camera will experience when moving side to side")]
	public float movementRotationMaxAngle_ = 10.0f;
	[Tooltip("Rotation the camera will experience when moving side to side")]
	public float movementRotationSmoothness_ = 1.0f;
	[Tooltip("Move speed of the crouched character in m/s")]
	public float crouchSpeed_;
	[Tooltip("Speed at which the player crouches")]
	public float crouchAnimationSpeed_;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;

	[Header("Weapons")]
	public WeaponInventory weaponInventory_;

	[Header("Health")]
	public HealthComponent healthComponent_;

	// cinemachine
	private float _cinemachineTargetPitch;
	private float cinemachineTargetYaw_;
	private float cinemachineCurrentYaw_;
	private float movementRotationLerpFactor_;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	private PlayerInput _playerInput;
#endif
	private CharacterController _controller;
	private StarterAssetsInputs _input;
	private GameObject _mainCamera;

	private const float _threshold = 0.01f;

	private bool IsCurrentDeviceMouse
	{
		get
		{
			#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			return _playerInput.currentControlScheme == "KeyboardMouse";
			#else
			return false;
			#endif
		}
	}

	private void Awake()
	{
		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

		weaponInventory_ = GetComponent<WeaponInventory>();
		healthComponent_ = GetComponent<HealthComponent>();
	}

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		_playerInput = GetComponent<PlayerInput>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	private void Update()
	{
		ManageWeaponInventory();
		JumpAndGravity();
		GroundedCheck();
		Move();
		Crouch();
		Shoot();
	}

	private void Crouch()
    {
		if(Grounded)
        {
			float desiredHeight = _input.crouch ? 1.0f : 2.0f;
			float desiredCameraHeight = _input.crouch ? 0.6875f : 1.375f;
			AdjustHeight(desiredHeight, desiredCameraHeight);
		}
	}

	private void AdjustHeight(float height, float cameraHeight)
    {
		float center = height * 0.5f;
		_controller.height = Mathf.Lerp(_controller.height, height, crouchAnimationSpeed_ * Time.deltaTime);
		_controller.center = Vector3.Lerp(_controller.center, new Vector3(0.0f, center, 0.0f), crouchAnimationSpeed_ * Time.deltaTime);
		transform.GetChild(0).localPosition = 
			Vector3.Lerp(transform.GetChild(0).localPosition, 
				new Vector3(transform.GetChild(0).localPosition.x, cameraHeight, transform.GetChild(0).localPosition.z),
				crouchAnimationSpeed_ * Time.deltaTime);
	}

	private void Shoot()
       {
		if(weaponInventory_.CurrentWeapon != null)
           {
			if(_input.shoot)
			{
				weaponInventory_.CurrentWeapon.Shoot();
				if(weaponInventory_.IsSemiAutomaticWeapon()) _input.shoot = false;
			}

			if (_input.showAnimation)
			{
				weaponInventory_.CurrentWeapon.ShowAnimation();
				_input.showAnimation = false;
			}
		}
	}
	private void ManageWeaponInventory()
	{
		// TODO: Change Weapon only if not shooting
		// Add an input queue so if weapon is to be changed, change it after animation ends
		if (weaponInventory_ != null)
		{
			if(_input.scrollUp)
            {
				weaponInventory_.SwapToNextWeapon();
				_input.scrollUp = false;
            }

			if (_input.scrollDown)
			{
				weaponInventory_.SwapToPreviousWeapon();
				_input.scrollDown = false;
			}
			if (_input.selectedWeapon != -1)
			{
				weaponInventory_.SwapToWeapon(_input.selectedWeapon);
				_input.selectedWeapon = -1;
			}
		}
	}


	private void LateUpdate()
	{
		CameraRotation();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

	private void CameraRotation()
	{
		// if there is an input
		if (_input.look.sqrMagnitude >= _threshold)
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
			
			_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
			_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}

		cinemachineCurrentYaw_ = Mathf.Lerp(cinemachineCurrentYaw_, cinemachineTargetYaw_, movementRotationSmoothness_ * Time.deltaTime);
		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, cinemachineCurrentYaw_);
	}

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = _input.crouch ? crouchSpeed_ : MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.move != Vector2.zero)
		{
			// move
			inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
		}

		if(_input.move.x != 0.0f)
        {
			cinemachineTargetYaw_ = _input.move.x > 0.0f ? -movementRotationMaxAngle_ : movementRotationMaxAngle_;
		}
		else
		{
			cinemachineTargetYaw_ = 0.0f;
		}

		_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void JumpAndGravity()
	{
		if (Grounded && !_input.crouch)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			_input.jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}
}