using Godot;

public class CharacterWalkState : CharacterBaseState{
    public CharacterWalkState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
        IsGroundedSubState = true;
    }
    public override void EnterState(){
        
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        
    }

    public override void CheckSwitchStates(){

    }

    public override void InitializeSubState(){
        
    }


}