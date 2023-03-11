using Godot;

/// <summary>
/// This character controller was designed to give Unity users a more familiar approach to controlling a character in Godot.
/// All assets are provided as is with no guarantee of their functionality; I do not provide one on one support for free assets.
/// Extending this asset will require intermediate knowledge of C# and software architecture
/// I hope that this will help you in your game dev journey
/// </summary>
public partial class CharacterStateMachine : CharacterBody2D{
    #region GODOT

    /// <summary>
    /// START METHOD; CALLED ONCE AT START
    /// </summary>
    public override void _Ready(){
        _sprite = GetNode<Node2D>("Sprite");
        _states = new CharacterStateFactory(this);
        CalculateGravity();
        CalculateJumpVelocity();
        CalculateDashForce();
        AssignCollisionCallbacks();
    }

    /// <summary>
    /// FIXED UPDATE METHOD; CALLED EACH PHYSICS UPDATE
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta){
        _lastPhysicsPosition = GlobalPosition;
        CalculateVelocity(delta);
        MoveAndSlide();
        UpdateCollisionMessages();
    }

    /// <summary>
    /// UPDATE METHOD; CALLED EACH FRAME
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta){
        GatherInput();
        InterpolatePlayerPosition();
        UpdateStates();
        DelayedActions.IncrementActions(delta);
    }

    #endregion

    #region STATES

    #region MEMBERS

    private CharacterBaseState _currentState;
    private CharacterStateFactory _states;
    private Vector2 _previousVelocity;
    private Vector2 _currentVelocity;

    #endregion

    #region EVENTS

    public delegate void OnStateChanged(AnimationStates state);

    public OnStateChanged onStateChanged;

    #endregion

    private void UpdateStates(){
        _currentVelocity = Velocity;
        if (Grounded){
            //Check idle
            if (_previousVelocity.X != 0 && _currentVelocity.X == 0){
                onStateChanged?.Invoke(AnimationStates.Idle);
            }

            //Check began walking
            if (_previousVelocity.X == 0 && _currentVelocity.X != 0 && !_isRunPressed){
                onStateChanged?.Invoke(AnimationStates.Walk);
            }
        }

        _previousVelocity = Velocity;
    }

    #endregion

    #region INTERPOLATION

    private Node2D _sprite;
    private Vector2 _lastPhysicsPosition;

    /// <summary>
    /// This fixes jittery movement when player is controlled in _PhysicsProcess
    /// It does so by breaking the sprite away from the controller and interpolating the sprite position to the controller position
    /// This is required as Godot does not interpolate physics.
    /// </summary>
    private void InterpolatePlayerPosition(){
        _sprite.TopLevel = true;
        _sprite.GlobalPosition =
            _sprite.GlobalPosition.Slerp(_lastPhysicsPosition, (float)Engine.GetPhysicsInterpolationFraction());
    }

    #endregion

    #region INPUT

    //VARIABLES
    private Vector2 _moveInput;
    private float _directionX;
    private bool _didJump;
    private bool _isRunPressed;


    /// <summary>
    /// Gather all user input
    /// </summary>
    private void GatherInput(){
        _moveInput = Input.GetVector("Left", "Right", "Up", "Down");
        _directionX = Input.GetAxis("Left", "Right");
        _isRunPressed = Input.IsActionPressed("Run");
        if (Input.IsActionJustPressed("Jump") && _currentJumps < maxJumps){
            _didJump = true;
        }

        if (_moveInput.LengthSquared() != 0){
            if (Input.IsActionJustPressed("Dash") && !_didDash && !_isDashing &&
                _moveInput.GetDirection().Vector.Y <= 0){
                _didDash = true;
            }

            if (Input.IsActionJustPressed("Dash") && !_didDive && !_isDiving &&
                _moveInput.GetDirection().Vector.Y > 0 &&
                !Grounded){
                _didDive = true;
            }
        }
    }

    #endregion
    
    #region COLLISIONS

    #region FLAGS

    public bool Grounded => IsOnFloor();
    public bool OnWall => IsOnWall();
    public bool OnCeiling => IsOnCeiling();

    private bool _wasOnWall;
    private bool _wasOnCeiling;
    private bool _wasOnGround;

    #endregion

    #region EVENTS

    public delegate void OnCollisionEnter();

    public delegate void OnCollisionExit();

    public OnCollisionEnter onGroundEnter;
    public OnCollisionEnter onCeilingEnter;
    public OnCollisionEnter onWallEnter;

    public OnCollisionExit onGroundExit;
    public OnCollisionExit onCeilingExit;
    public OnCollisionExit onWallExit;

    #endregion

    /// <summary>
    /// Update and send collision even messages
    /// </summary>
    private void UpdateCollisionMessages(){
        //CHECK ENTER MESSAGES
        if (!_wasOnGround && Grounded) onGroundEnter?.Invoke();
        if (!_wasOnWall && OnWall) onWallEnter?.Invoke();
        if (!_wasOnCeiling && OnCeiling) onCeilingEnter?.Invoke();

        //CHECK EXIT MESSAGES
        if (_wasOnGround && !Grounded) onGroundExit?.Invoke();
        if (_wasOnCeiling && !OnCeiling) onCeilingExit?.Invoke();
        if (_wasOnWall && !OnWall) onWallExit?.Invoke();

        //RESET CHECK VARIABLES
        _wasOnGround = Grounded;
        _wasOnWall = OnWall;
        _wasOnCeiling = OnCeiling;
    }

    /// <summary>
    /// Assign callback methods that depend on collision state
    /// </summary>
    private void AssignCollisionCallbacks(){
        onGroundEnter += ResetDive;
        onGroundEnter += ResetJump;
        onGroundEnter += ResetWallCling;
        onWallExit += ResetWallCling;
        onWallEnter += WallCling;
        onWallEnter += ResetDive;
        onWallEnter += ResetJump;
    }

    #endregion

    #region BASIC MOVEMENT

    #region MEMBERS

    //Basic Movement
    [Export] public float moveSpeed = 128.0f;
    [Export] public float runSpeedMultiplier = 2f;
    [Export] public float acceleration = 10f;
    [Export] public float deceleration = 10f;
    [Export] public float airAcceleration = 5f;
    [Export] public float airDeceleration = 5f;

    #endregion

    /// <summary>
    /// Calculate directional velocity, then apply to the character
    /// </summary>
    /// <param name="delta"></param>
    private void CalculateVelocity(double delta){
        Vector2 velocity = Velocity;
        CalculateVelocityX(ref velocity);
        CalculateVelocityY(ref velocity, delta);
        CalculateAbilityVelocity(ref velocity, delta);
        Velocity = velocity;
    }


    /// <summary>
    /// Calculate the X velocity to apply to the character
    /// </summary>
    /// <param name="vel"></param>
    private void CalculateVelocityX(ref Vector2 vel){
        //Calculate target velocity
        float targetVelocity = _isRunPressed ? _moveInput.X * moveSpeed * runSpeedMultiplier : _moveInput.X * moveSpeed;
        if (!_isDashing){
            if (_moveInput.X != 0){
                vel.X = Mathf.MoveToward(
                    vel.X,
                    targetVelocity,
                    _moveInput.X == Mathf.Sign(vel.X) ? acceleration : deceleration
                );
            }
            else{
                vel.X = Mathf.MoveToward(Velocity.X, 0, deceleration);
            }
        }
    }

    /// <summary>
    /// Calculate the Y velocity to apply the the character
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="vel"></param>
    private void CalculateVelocityY(ref Vector2 vel, double delta){
        ApplyGravity(delta, ref vel);
    }

    #endregion

    #region GRAVITY

    [Export] public float maxFallSpeed = 512f;
    private float _gravity;

    /// <summary>
    /// Calculate gravity force to be applied to the character based on his chosen jump height and time to jump apex
    /// </summary>
    private void CalculateGravity(){
        _gravity = 2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
    }

    /// <summary>
    /// Apply gravity to the character
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="vel"></param>
    private void ApplyGravity(double delta, ref Vector2 vel){
        //Apply gravity only if not grounded and not dashing
        if (Grounded || _isDashing) return;
        //Smooth gravity using Verlet integration for improved accuracy
        float previousVelocityY = vel.Y;
        vel.Y += vel.Y >= 0 ? _gravity * fallSpeedMultiplier * (float)delta : _gravity * (float)delta;
        float appliedVelocityY = (vel.Y + previousVelocityY) * 0.5f;
        //Clamp applied velocity to max fall speed
        vel.Y = Mathf.Clamp(appliedVelocityY, -maxFallSpeed, maxFallSpeed);
    }

    #endregion

    #region ABILITIES

    #region SHARED METHODS

    /// <summary>
    /// Calculate velocity to be applied by abilities; this overwrites any previously calculated velocity
    /// </summary>
    /// <param name="vel"></param>
    private void CalculateAbilityVelocity(ref Vector2 vel, double delta){
        if (_moveInput.GetDirection().Vector.Y <= 0){
            //Dash
            if (_didDash) Dash(ref vel);
        }
        else{
            //Dive
            if (_didDive) Dive(ref vel);
        }

        //Normal jump
        if (_didJump && _currentJumps < maxJumps && !_isWallClinging){
            Jump(ref vel);
        }

        //Wall Cling
        if (_isWallClinging){
            wallClingTimer += delta;
            if (wallClingTimer <= wallClingTime){
                //Slow Character fall
                vel.Y *= wallClingGravityModifier;
                //Force character against wall
                vel.X = -_wallNormal.X;
            }
        }

        //Wall Jump
        if (_isWallClinging && _didJump){
            //Can't wall jump unless clinging to a wall
            WallJump(ref vel);
        }
    }

    #endregion

    #region JUMP

    //Jumping
    [Export] public float jumpHeight = 32;
    [Export] public float timeToJumpApex = 0.25f;
    [Export] public float fallSpeedMultiplier = 2f;
    [Export] public uint maxJumps = 2;
    private float _jumpVelocity;
    private uint _currentJumps;

    /// <summary>
    /// Apply a jump force to the character
    /// </summary>
    /// <param name="vel"></param>
    private void Jump(ref Vector2 vel){
        vel.Y = _jumpVelocity;
        _currentJumps++;
        _didJump = false;
    }

    /// <summary>
    /// Calculate the jump velocity required to achieve a jump with a given height and time to jump apex
    /// </summary>
    private void CalculateJumpVelocity(){
        _jumpVelocity = -_gravity * timeToJumpApex;
    }

    /// <summary>
    /// Reset jump ability
    /// </summary>
    private void ResetJump(){
        _currentJumps = 0;
    }

    #endregion

    #region WALL CLING

    [Export] public float wallClingTime = .5f;
    [Export] public float wallClingGravityModifier = .2f;
    private double wallClingTimer;
    private bool _isWallClinging;
    private Vector2 _wallNormal;

    private void WallCling(){
        if (Grounded) return;
        int collisionCount = GetSlideCollisionCount();
        for (int i = 0; i < collisionCount; i++){
            _wallNormal = GetSlideCollision(i).GetNormal();
        }

        _isWallClinging = true;
    }

    private void ResetWallCling(){
        if (!_isWallClinging) return;
        _isWallClinging = false;
        _wallNormal = Vector2.Zero;
        wallClingTimer = 0;
    }

    #endregion

    #region WALL JUMP

    private void WallJump(ref Vector2 vel){
        _didJump = false;
        //Apply jump force
        vel = new Vector2(_jumpVelocity * -_wallNormal.X, _jumpVelocity);
    }

    #endregion

    #region DASH

    //Dash
    [Export] public float dashUnits = 128f;
    [Export] public float dashTime = .25f;
    private bool _canDash = true;
    private float _dashForce;
    private bool _didDash;
    private bool _isDashing;

    /// <summary>
    /// Apply dash force to character
    /// </summary>
    /// <param name="vel">Reference to the current velocity, this will be overwritten</param>
    private void Dash(ref Vector2 vel){
        _isDashing = true;
        _didDash = false;

        if (!Grounded){
            vel = _moveInput.GetDirection().Vector * _dashForce;
        }
        else{
            vel.X = _directionX * _dashForce;
        }

        DelayedActions.Add(DashComplete, dashTime);
    }

    /// <summary>
    /// Calculate the dash force using the number of units to move / the time it should take to move there
    /// </summary>
    private void CalculateDashForce(){
        _dashForce = dashUnits / dashTime;
    }

    /// <summary>
    /// Execute this method when dash has completed
    /// </summary>
    private void DashComplete(){
        Vector2 velocity = Velocity;
        //If character moving up, reset Y velocity to 0
        velocity.Y = 0;
        if (_isRunPressed){
            velocity.X = moveSpeed * _directionX * runSpeedMultiplier;
        }
        else{
            velocity.X = moveSpeed * _directionX;
        }

        Velocity = velocity;
        _isDashing = false;
    }

    #endregion

    #region DIVE

    private bool _canDive = true;
    private bool _didDive;
    private bool _isDiving;

    /// <summary>
    /// Apply dive force to the character
    /// </summary>
    /// <param name="vel">Reference to the current velocity, this will be overwritten</param>
    private void Dive(ref Vector2 vel){
        _didDive = false;
        _isDiving = true;
        vel = _moveInput.GetDirection().Vector * _dashForce * 2;
    }

    /// <summary>
    /// Call this method when dive has been completed, this happens when the player hits something as a dive can only take place while airborne
    /// </summary>
    private void DiveComplete(){
    }

    /// <summary>
    /// Call this method to reset the dive ability
    /// </summary>
    private void ResetDive(){
        _isDiving = false;
        DiveComplete();
    }

    #endregion

    #endregion
}