using System.Collections.Generic;

public enum CharacterStates{
    Grounded,
    Airborne,
    Idle,
    Walk,
    Run,
    Fall,
    WallCling,
    WallJump,
    Jump,
    DoubleJump,
    Hit,
    Dash,
    Dive,
    Slide
}

public class CharacterStateManager{
    private CharacterStateMachine _context;

    private Dictionary<CharacterStates, CharacterBaseState> _states;

    public CharacterStateManager(CharacterStateMachine currentContext){
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
            { CharacterStates.Jump, new CharacterJumpState(_context, this) },
            { CharacterStates.DoubleJump, new CharacterDoubleJumpState(_context, this) },
            { CharacterStates.WallCling, new CharacterWallClingState(_context, this) },
            { CharacterStates.WallJump, new CharacterWallJumpState(_context, this) },
            { CharacterStates.Dash, new CharacterDashState(_context, this) },
            { CharacterStates.Dive, new CharacterDiveState(_context, this) },
            { CharacterStates.Slide, new CharacterSlideState(_context, this) }
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
    
    public CharacterBaseState DoubleJump(){
        return _states[CharacterStates.DoubleJump];
    }
    
    public CharacterBaseState WallJump(){
        return _states[CharacterStates.WallJump];
    }
    
    public CharacterBaseState Run(){
        return _states[CharacterStates.Run];
    }

    public CharacterBaseState Fall(){
        return _states[CharacterStates.Fall];
    }
    
    public CharacterBaseState WallCling(){
        return _states[CharacterStates.WallCling];
    }

    public CharacterBaseState Dash(){
        return _states[CharacterStates.Dash];
    }
    
    public CharacterBaseState Dive(){
        return _states[CharacterStates.Dive];
    }

    public CharacterBaseState Slide(){
        return _states[CharacterStates.Slide];
    }

    
}