using Godot;

public class CharacterWalkState : CharacterBaseState{
    public CharacterWalkState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
        IsGroundedSubState = true;
    }
    public override void EnterState(){
        GD.Print("Enter Walk");
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        GD.Print("Exit Walk");
    }

    public override void CheckSwitchStates(){
        if (Context.Velocity.X == 0){
            SwitchState(Factory.Idle());
        }

        if (Context.IsRunPressed){
            SwitchState(Factory.Run());
        }
        
    }

    public override void InitializeSubState(){
        
    }


}