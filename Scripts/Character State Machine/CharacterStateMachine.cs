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
        InitializeInterpolation();
        InitializeStates();
        CalculateGravity();
        CalculateJumpVelocity();
        CalculateDashForce();
    }

    /// <summary>
    /// FIXED UPDATE METHOD; CALLED EACH PHYSICS UPDATE
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta){
        _fixedDelta = delta;
        _lastPhysicsPosition = GlobalPosition;
        MoveAndSlide();
        UpdateCollisionMessages();
    }

    /// <summary>
    /// UPDATE METHOD; CALLED EACH FRAME
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta){
        _delta = delta;
        GatherInput();
        InterpolatePlayerPosition();
        UpdateJumpTime(delta);
        UpdateStates();
    }

    #endregion

    #region STATES

    #region MEMBERS

    private CharacterBaseState _currentState;
    private CharacterStateManager _states;

    #endregion

    #region PROPERTIES

    public CharacterBaseState CurrentState{
        get => _currentState;
        set => _currentState = value;
    }

    #endregion

    #region EVENTS

    public delegate void OnStateEnter(CharacterBaseState state);

    public OnStateEnter onStateEnter;

    public delegate void OnStateExit(CharacterBaseState state);

    public OnStateExit onStateExit;

    #endregion

    private void UpdateStates(){
        _currentState.UpdateStates();
    }

    private void InitializeStates(){
        _states = new CharacterStateManager(this);
        _currentState = _states.Airborne();
        _currentState.EnterState();
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
            _sprite.GlobalPosition.Slerp(GlobalPosition, (float)Engine.GetPhysicsInterpolationFraction());
    }

    /// <summary>
    /// Initialize members required for interpolation
    /// </summary>
    private void InitializeInterpolation(){
        _sprite = GetNode<Node2D>("Sprite");
    }

    #endregion

    #region INPUT

    #region MEMBERS

    private Vector2 _moveInput;
    private float _directionX;
    private bool _didJump;
    private bool _isRunPressed;

    #endregion

    #region PROPERTIES

    public bool IsRunPressed => _isRunPressed;
    public Vector2 MoveInput => _moveInput;

    public bool DidJump{
        get => _didJump;
        set => _didJump = value;
    }

    #endregion


    /// <summary>
    /// Gather all user input
    /// </summary>
    private void GatherInput(){
        _moveInput = Input.GetVector("Left", "Right", "Up", "Down");
        _moveInput = _moveInput.GetRaw();
        _directionX = Input.GetAxis("Left", "Right");
        _isRunPressed = Input.IsActionPressed("Run");
        if (Input.IsActionJustPressed("Jump") && _currentJumps < maxJumps){
            _didJump = true;
        }

        if (Input.IsActionJustReleased("Jump")){
            _didJump = false;
            _isJumping = false;
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

    #endregion

    #region BASIC MOVEMENT

    #region MEMBERS

    [Export] public float moveSpeed = 128.0f;
    [Export] public float runSpeedMultiplier = 2f;
    [Export] public float acceleration = 10f;
    [Export] public float deceleration = 10f;
    [Export] public float airAcceleration = 5f;
    [Export] public float airDeceleration = 5f;
    private Vector2 _previousVelocity;
    private Vector2 _currentVelocity;
    private double _delta;
    private double _fixedDelta;

    #endregion

    #region PROPERTIES

    public Vector2 PreviousVelocity{
        get => _previousVelocity;
        set => _previousVelocity = value;
    }

    public Vector2 CurrentVelocity{
        get => _currentVelocity;
        set => _currentVelocity = value;
    }

    public double Delta => _delta;
    public double FixedDelta => _fixedDelta;

    #endregion

    #endregion

    #region GRAVITY

    [Export] public float maxFallSpeed = 512f;
    private float _gravity;
    public float Gravity => _gravity;

    /// <summary>
    /// Calculate gravity force to be applied to the character based on his chosen jump height and time to jump apex
    /// </summary>
    private void CalculateGravity(){
        _gravity = 2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
    }

    #endregion

    #region ABILITIES

    #region SHARED METHODS

    /// <summary>
    /// Calculate velocity to be applied by abilities; this overwrites any previously calculated velocity
    /// </summary>
    /// <param name="vel">Reference to _currentVelocity</param>
    /// /// <param name="delta">Delta from _PhysicsProcess</param>
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
            _wallClingTimer += delta;
            if (_wallClingTimer <= wallClingTime){
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

    #region MEMBERS

    [Export] public float jumpHeight = 32;
    [Export] public float timeToJumpApex = 0.25f;
    [Export] public float fallSpeedMultiplier = 2f;
    [Export] public uint maxJumps = 2;
    private float _jumpVelocity;
    private uint _currentJumps;
    private bool _isJumping;
    private double _jumpTimer;

    #endregion

    #region PROPERTIES

    public double JumpTimer{
        get => _jumpTimer;
        set => _jumpTimer = value;
    }


    public float JumpVelocity => _jumpVelocity;

    public uint CurrentJumps{
        get => _currentJumps;
        set => _currentJumps = value;
    }

    public bool IsJumping{
        get => _isJumping;
        set => _isJumping = value;
    }

    #endregion

    /// <summary>
    /// Apply a jump force to the character
    /// </summary>
    /// <param name="vel"></param>
    private void Jump(ref Vector2 vel){
        vel.Y = _jumpVelocity;
        _currentJumps++;
        _didJump = false;
    }

    private void UpdateJumpTime(double delta){
        if (_jumpTimer > 0){
            _jumpTimer -= delta;
        }
        else{
            _isJumping = false;
        }
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
    public bool canWallCling = true;
    private double _wallClingTimer;
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
        _wallClingTimer = 0;
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