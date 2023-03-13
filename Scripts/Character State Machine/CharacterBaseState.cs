using System.ComponentModel;
using System.Diagnostics;
using Godot;

public abstract class CharacterBaseState{
    #region MEMBERS

    private CharacterStateMachine _context;
    private CharacterStateManager _manager;
    private CharacterBaseState _currentSubState;
    private CharacterBaseState _currentSuperState;
    private bool _isRootState = false;
    private bool _isAirborneSubState = false;
    private bool _isGroundedSubState = false;
    protected Vector2 velocity;

    #endregion

    #region PROPERTIES

    protected CharacterStateMachine Context => _context;
    protected CharacterStateManager Manager => _manager;
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
    /// <param name="characterStateManager">Reference to the current state factory</param>
    public CharacterBaseState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager){
        _context = currentContext;
        _manager = characterStateManager;
    }

    /// <summary>
    /// Update all current root and sub states
    /// </summary>
    public void UpdateStates(){
        Velocity = Context.Velocity;
        CalculateVelocity();
        UpdateState();
        _currentSubState?.UpdateStates();
    }

    /// <summary>
    /// Exit all states and substates
    /// </summary>
    protected void ExitStates(){
        //Invoke callbacks for this state and all substates
        Context.onStateExit?.Invoke(_currentSubState);
        Context.onStateExit?.Invoke(this);
        //Exit this state and all sub states
        _currentSubState?.ExitStates();
        ExitState();
    }

    /// <summary>
    /// Switch a state and apply logic pertaining to the switch
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(CharacterBaseState newState){
        //Exit current state and sub states
        ExitStates();
        //Enter new state
        newState.EnterState();
        //Invoke callback to notify other scripts of the state change
        Context.onStateEnter?.Invoke(newState);
        //Switch context state to the new state only if the new state is a root state
        if (_isRootState && newState._isRootState){
            //First, reset the this state for a fresh start next time
            Reset();
            _context.CurrentState = newState;
        }
        else{
            _currentSuperState?.SetSubState(newState);
        }
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
    public void SetSubState(CharacterBaseState newSubState){
        if (_currentSubState == newSubState) return;
        _currentSubState?.ExitStates();
        _currentSubState = newSubState;
        Context.onStateEnter?.Invoke(newSubState);
        newSubState.EnterState();
        newSubState.SetSuperState(this);
    }

    /// <summary>
    /// Calculate velocity to be applied to the character
    /// </summary>
    private void CalculateVelocity(){
        velocity = Context.Velocity;
        CalculateVelocityX(ref velocity);
        CalculateVelocityY(ref velocity, Context.Delta);
        Context.Velocity = velocity;
    }
    
    /// <summary>
    /// Calculate vertical velocity to apply to the character
    /// Base state applies no vertical velocity
    /// This method exists to be overridden by concrete states
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    protected virtual void CalculateVelocityY(ref Vector2 vel, double delta){
        
    }
    
    /// <summary>
    /// Calculate horizontal velocity to apply to the character
    /// Base state will apply no horizontal velocity
    /// This method exists to be overridden by concrete states
    /// </summary>
    protected virtual void CalculateVelocityX(ref Vector2 vel){

    }

    /// <summary>
    /// Set the velocity of the context to the velocity of the root state, else set the velocity of the root state
    /// </summary>
    /// <param name="vel"></param>
    protected void SetVelocity(Vector2 vel){
        Context.Velocity = vel;
    }

    protected void AddImpulse(Vector2 vel){
        Context.Velocity += vel;
    }

    protected void CancelVelocity(bool cancelX = false){
        Vector2 newVelocity = Vector2.Zero;
        if (!cancelX) newVelocity.X = Context.Velocity.X;
        SetVelocity(newVelocity);
    }

    protected virtual void Reset(){
        //Clear states
        _currentSuperState = null;
        _currentSubState = null;
    }
}