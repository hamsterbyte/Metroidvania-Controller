using Godot;

public class CharacterGroundedState : CharacterBaseState{
    public CharacterGroundedState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
    }

    public override void EnterState(){
        GD.Print("Entering Grounded State");
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