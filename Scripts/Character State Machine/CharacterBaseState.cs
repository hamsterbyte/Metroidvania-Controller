using Godot;

public abstract class CharacterBaseState{
    #region MEMBERS

    private CharacterStateMachine _context;
    private CharacterStateFactory _factory;
    private CharacterBaseState _currentSubState;
    private CharacterBaseState _currentSuperState;
    private bool _isRootState = false;
    private bool _isAirborneSubState = false;
    private bool _isGroundedSubState = false;
    protected Vector2 velocity;

    #endregion

    #region PROPERTIES

    protected CharacterStateMachine Context => _context;
    protected CharacterStateFactory Factory => _factory;
    protected CharacterBaseState CurrentSubState => _currentSubState;
    protected CharacterBaseState CurrentSuperState => _currentSuperState;

    public Vector2 Velocity{
        get => velocity;
        set => velocity = value;
    }
    
    protected bool IsRootState{
        get => _isRootState;
        set => _isRootState = value;
    }

    public bool IsAirborneSubState{
        get => _isAirborneSubState;
        set => _isAirborneSubState = value;
    }

    public bool IsGroundedSubState{
        get => _isGroundedSubState;
        set => _isGroundedSubState = value;
    }

    #endregion

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="currentContext">Reference to the current context</param>
    /// <param name="characterStateFactory">Reference to the current state factory</param>
    public CharacterBaseState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory){
        _context = currentContext;
        _factory = characterStateFactory;
    }

    /// <summary>
    /// Update all current root and sub states
    /// </summary>
    public void UpdateStates(){
        CalculateVelocity();
        UpdateState();
        _currentSubState?.UpdateStates();
    }

    /// <summary>
    /// Exit all states and substates
    /// </summary>
    protected void ExitStates(){
        _currentSubState?.ExitStates();
        ExitState();
    }

    /// <summary>
    /// Switch a state and apply logic pertaining to the switch
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(CharacterBaseState newState){
        //Exit current state and substates
        ExitStates();
        //Switch context state to the new state only if the new state is a root state
        if (newState._isRootState){
            //First, reset the root state for a fresh start
            newState.Reset();
            _context.CurrentState = newState;
        }
        else{
            _currentSuperState?.SetSubState(newState);
        }
        //Enter new state
        newState.EnterState();
    }

    /// <summary>
    /// Set the current super state
    /// </summary>
    /// <param name="newSuperState"></param>
    protected void SetSuperState(CharacterBaseState newSuperState){
        _currentSuperState = newSuperState;
    }

    /// <summary>
    /// Set the current sub state if different than the current sub state
    /// </summary>
    /// <param name="newSubState">New sub state to check and switch if different</param>
    protected void SetSubState(CharacterBaseState newSubState){
        if (_currentSubState == newSubState) return;
        _currentSubState?.ExitStates();
        _currentSubState = newSubState;
        newSubState.EnterState();
        newSubState.SetSuperState(this);
    }

    /// <summary>
    /// Calculate velocity to be applied to the character
    /// </summary>
    private void CalculateVelocity(){
        CalculateVelocityX(ref velocity);
        CalculateVelocityY(ref velocity, Context.Delta);
        SetVelocity(velocity);
    }
    
    /// <summary>
    /// Calculate vertical velocity to apply to the character
    /// Base state applies no vertical velocity
    /// This method exists to be overridden by sub states
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    protected virtual void CalculateVelocityY(ref Vector2 vel, double delta){
        
    }
    
    /// <summary>
    /// Calculate horizontal velocity to apply to the character
    /// Base state will apply walk or run velocity depending on input
    /// </summary>
    protected virtual void CalculateVelocityX(ref Vector2 vel){
        //Calculate target velocity
        float targetVelocity = Context.IsRunPressed
            ? Context.MoveInput.X * Context.moveSpeed * Context.runSpeedMultiplier
            : Context.MoveInput.X * Context.moveSpeed;

        if (Context.MoveInput.X != 0){
            vel.X = Mathf.MoveToward(
                vel.X,
                targetVelocity,
                Context.MoveInput.X == Mathf.Sign(vel.X) ? Context.acceleration : Context.deceleration
            );
        }
        else{
            vel.X = Mathf.MoveToward(Context.Velocity.X, 0, Context.deceleration);
        }
    }

    /// <summary>
    /// Set the velocity of the context to the velocity of the root state, else set the velocity of the root state
    /// </summary>
    /// <param name="vel"></param>
    protected void SetVelocity(Vector2 vel){
        if (_isRootState){
            Context.Velocity = vel;
        }
        else{
            _currentSuperState.velocity = vel;
        }
    }

    protected virtual void Reset(){
        _currentSubState = null;
        _currentSuperState = null;
    }
}