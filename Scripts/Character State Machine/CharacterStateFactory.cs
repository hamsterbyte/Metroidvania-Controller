public class CharacterStateFactory{
    private CharacterStateMachine _context;

    public CharacterStateFactory(CharacterStateMachine currentContext){
        _context = currentContext;
    }

    public CharacterBaseState Idle(){
        return new CharacterIdleState(_context, this);
    }
    
    public CharacterBaseState Walk(){
        return new CharacterWalkState(_context, this);
    }
    
    public CharacterBaseState Jump(){
        return new CharacterJumpState(_context, this);
    }
    
    public CharacterBaseState Grounded(){
        return new CharacterGroundedState(_context, this);
    }
    
    public CharacterBaseState Run(){
        return new CharacterRunState(_context, this);
    }
    
    public CharacterBaseState Fall(){
        return new CharacterFallState(_context, this);
    }
    
    
}