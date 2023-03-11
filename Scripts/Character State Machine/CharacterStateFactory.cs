using System.Collections.Generic;

public enum CharacterStates{
    Grounded,
    Airborne,
    Idle,
    Walk,
    Run,
    Fall,
    WallSlide,
    WallJump,
    Jump,
    Hit
}

public class CharacterStateFactory{
    private CharacterStateMachine _context;

    private Dictionary<CharacterStates, CharacterBaseState> _states;

    public CharacterStateFactory(CharacterStateMachine currentContext){
        _context = currentContext;
        SetupStateDictionary();
    }

    private void SetupStateDictionary(){
        _states = new Dictionary<CharacterStates, CharacterBaseState>(){
            { CharacterStates.Grounded, new CharacterGroundedState(_context, this) },
            { CharacterStates.Airborne, new CharacterAirborneState(_context, this) },
            { CharacterStates.Idle, new CharacterIdleState(_context, this) },
            { CharacterStates.Walk, new CharacterWalkState(_context, this) },
            { CharacterStates.Run, new CharacterRunState(_context, this) },
            { CharacterStates.Fall, new CharacterFallState(_context, this) },
        };
    }
    
    public CharacterBaseState Grounded(){
        return _states[CharacterStates.Grounded];
    }
    
    public CharacterBaseState Airborne(){
        return _states[CharacterStates.Airborne];
    }
    
    public CharacterBaseState Idle(){
        return _states[CharacterStates.Idle];
    }

    public CharacterBaseState Walk(){
        return _states[CharacterStates.Walk];
    }

    public CharacterBaseState Jump(){
        return _states[CharacterStates.Jump];
    }
    public CharacterBaseState Run(){
        return _states[CharacterStates.Run];
    }

    public CharacterBaseState Fall(){
        return _states[CharacterStates.Fall];
    }

    
}