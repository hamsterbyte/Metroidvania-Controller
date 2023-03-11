using Godot;

public class CharacterRunState : CharacterBaseState{
    public CharacterRunState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
        IsGroundedSubState = true;
    }
    public override void EnterState(){
        GD.Print("Entered Run");
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        GD.Print("Exit Run");
    }

    public override void CheckSwitchStates(){
        
    }

    public override void InitializeSubState(){
        
    }

}