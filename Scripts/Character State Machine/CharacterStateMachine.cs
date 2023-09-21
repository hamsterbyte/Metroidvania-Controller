using System;
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
        UpdateDashTime(delta);
        DelayedActions.IncrementActions(delta);
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
            _sprite.GlobalPosition.Lerp(GlobalPosition, (float)Engine.GetPhysicsInterpolationFraction());
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

    public float DirectionX => _directionX;

    #endregion


    /// <summary>
    /// Gather all user input
    /// </summary>
    private void GatherInput(){
        _moveInput = Input.GetVector("Left", "Right", "Up", "Down");
        _directionX = Mathf.Sign(_moveInput.X) switch{
            1 => 1,
            -1 => -1,
            _ => 0
        };
        _isRunPressed = Input.IsActionPressed("Run");
        if (Input.IsActionJustPressed("Jump") && _currentJumps < maxJumps){
            _didJump = true;
        }

        if (Input.IsActionJustReleased("Jump")){
            _didJump = false;
            _isJumping = false;
        }

        if (_moveInput.LengthSquared() != 0){
            if (Input.IsActionJustPressed("Dash") && !_didDash && !_isDashing){
                _didDash = true;
                _isDashing = true;
            }
        }


        if (Input.IsActionJustPressed("Dive") && !_didDive && !Grounded){
            _didDive = true;
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
    [Export] public float fallSpeedMultiplier = 2f;
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

    #region JUMP

    #region MEMBERS

    [Export] public float jumpHeight = 32;
    [Export] public float timeToJumpApex = 0.25f;
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

    #endregion

    #region WALL CLING

    [Export] public float wallClingTime = .5f;
    [Export] public float wallClingGravityModifier = .2f;
    public bool canWallCling = true;

    #endregion

    #region WALL JUMP

    #endregion

    #region DASH

    //MEMBERS
    [Export] public float dashUnits = 128f;
    [Export] public float dashTime = .25f;
    private bool _canDash = true;
    private double _dashCounter;
    private float _dashForce;
    private bool _didDash;
    private bool _isDashing;

    //PROPERTIES
    public bool DidDash{
        get => _didDash;
        set => _didDash = value;
    }

    public bool IsDashing{
        get => _isDashing;
        set => _isDashing = value;
    }

    public float DashForce => _dashForce;

    /// <summary>
    /// Calculate the dash force using the number of units to move / the time it should take to move there
    /// </summary>
    private void CalculateDashForce(){
        _dashForce = dashUnits / dashTime;
    }

    private void UpdateDashTime(double delta){
        if (!_isDashing) return;
        _dashCounter += delta;
        if (_dashCounter >= dashTime){
            ResetDash();
        }
    }

    public void ResetDash(){
        _isDashing = false;
        _dashCounter = 0;
        Velocity = Vector2.Zero;
    }

    #endregion

    #region DIVE

    //MEMBERS
    private bool _canDive = true;
    private bool _didDive = false;
    private bool _isDiving;

    public bool DidDive{
        get => _didDive;
        set => _didDive = value;
    }

    //PROPERTIES

    #endregion

    #endregion
}