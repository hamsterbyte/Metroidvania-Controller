public class CharacterStateFactory{
    private CharacterStateMachine _context;

    public CharacterStateFactory(CharacterStateMachine currentContext){
        _context = currentContext;
    }

    public CharacterBaseState Idle(){
        return new CharacterIdleState();
    }
    
    public CharacterBaseState Walk(){
        return new CharacterWalkState();
    }
    
    public CharacterBaseState Jump(){
        return new CharacterJumpState();
    }
    
    public CharacterBaseState Grounded(){
        return new CharacterGroundedState();
    }
    
    public CharacterBaseState Run(){
        return new CharacterRunState();
    }
    
    public CharacterBaseState Fall(){
        return new CharacterFallState();
    }
    
    
}