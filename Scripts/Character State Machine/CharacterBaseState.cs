public abstract class CharacterBaseState{

    protected CharacterStateMachine _context;
    protected CharacterStateFactory _factory;
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public CharacterBaseState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory){
        _context = currentContext;
        _factory = characterStateFactory;
    }

    private void UpdateStates(){
        
    }

    /// <summary>
    /// Switch a state and apply logic pertaining to the switch
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(CharacterBaseState newState){
        //Exit current state
        ExitState();
        //Enter new state
        newState.EnterState();
        //Switch context state to the new state
        _context.CurrentState = newState;
    }

    protected void SetSuperState(){
        
    }

    protected void SetSubState(){
        
    }
    
}