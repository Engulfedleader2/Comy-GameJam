using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float WalkSpeed = 200f;

	[Export]
	public float RunSpeed = 320f;

	private AnimatedSprite2D _animatedSprite;
	private bool _isJumping = false;
	private string _currentAnimation = "";
	private string _facingDirection = "right";

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = GetMovementInput();
		bool isSprinting = Input.IsActionPressed("sprint");

		HandleMovement(direction, isSprinting);
		UpdateFacing(direction);
		HandleJumpInput();
		UpdateAnimation(direction, isSprinting);
		UpdateRenderOrder();
	}

	private Vector2 GetMovementInput()
	{
		return Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}

	private void UpdateRenderOrder()
	{
		ZAsRelative = false;
		ZIndex = Mathf.RoundToInt(GlobalPosition.Y);
	}

	private void HandleMovement(Vector2 direction, bool isSprinting)
	{
		float currentSpeed = isSprinting ? RunSpeed : WalkSpeed;
		Velocity = direction * currentSpeed;
		MoveAndSlide();
	}

	private void HandleJumpInput()
	{
		if (Input.IsActionJustPressed("jump") && !_isJumping)
		{
			StartJump();
		}
	}

	private void StartJump()
	{
		_isJumping = true;
		PlayAnimation($"jump_{_facingDirection}");
	}

	private void UpdateFacing(Vector2 direction)
	{
		if (direction.X < 0)
		{
			_facingDirection = "left";
		}
		else if (direction.X > 0)
		{
			_facingDirection = "right";
		}
	}

	private void UpdateAnimation(Vector2 direction, bool isSprinting)
	{
		if (_isJumping)
		{
			return;
		}

		if (direction == Vector2.Zero)
		{
			PlayAnimation($"idle_{_facingDirection}");
			return;
		}

		if (isSprinting)
		{
			PlayAnimation($"run_{_facingDirection}");
			return;
		}

		PlayAnimation($"walk_{_facingDirection}");
	}

	private void PlayAnimation(string animationName)
	{
		if (_currentAnimation == animationName && _animatedSprite.IsPlaying())
		{
			return;
		}

		_currentAnimation = animationName;
		_animatedSprite.Play(animationName);
	}

	private void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "jump_left" || _animatedSprite.Animation == "jump_right")
		{
			_isJumping = false;
			Vector2 direction = GetMovementInput();
			bool isSprinting = Input.IsActionPressed("sprint");
			UpdateAnimation(direction, isSprinting);
		}
	}
}
