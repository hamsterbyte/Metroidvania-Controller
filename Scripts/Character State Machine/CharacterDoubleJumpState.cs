using Godot;

public class CharacterDoubleJumpState : CharacterBaseState{
    public CharacterDoubleJumpState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
    }
    public override void EnterState(){
        IsAirborneSubState = true;
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        
    }

    public override void CheckSwitchStates(){
        
    }

    public override void InitializeSubState(){
        
    }

}