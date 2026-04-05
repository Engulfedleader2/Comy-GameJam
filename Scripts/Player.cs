using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200f;

	private AnimatedSprite2D _animatedSprite;
	private bool _isJumping = false;
	private string _currentAnimation = "";

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = GetMovementInput();

		HandleMovement(direction);
		HandleJumpInput();
		UpdateAnimation(direction);
	}

	private Vector2 GetMovementInput()
	{
		return Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}

	private void HandleMovement(Vector2 direction)
	{
		Velocity = direction * Speed;
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
		PlayAnimation("jump");
	}

	private void UpdateAnimation(Vector2 direction)
	{
		UpdateFacing(direction);

		// While jumping, don't let walk/idle override the jump animation.
		if (_isJumping)
		{
			return;
		}

		if (direction == Vector2.Zero)
		{
			_animatedSprite.Stop();
			_currentAnimation = "";
			return;
		}

		PlayAnimation("walk");
	}

	private void UpdateFacing(Vector2 direction)
	{
		if (direction.X < 0)
		{
			_animatedSprite.FlipH = true;
		}
		else if (direction.X > 0)
		{
			_animatedSprite.FlipH = false;
		}
	}

	private void PlayAnimation(string animationName)
	{
		if (_currentAnimation == animationName)
		{
			return;
		}

		_currentAnimation = animationName;
		_animatedSprite.Play(animationName);
	}

	private void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "jump")
		{
			_isJumping = false;
		}
	}
}
