using Godot;

public class CharacterIdleState: CharacterBaseState{
    
    public CharacterIdleState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
        IsGroundedSubState = true;
    }
    public override void EnterState(){
        GD.Print("Enter Idle");
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        GD.Print("Exit Idle");
    }

    public override void CheckSwitchStates(){
        if (Context.Velocity.X != 0){
            if (!Context.IsRunPressed){
                SwitchState(Factory.Walk());
            }
            else{
                SwitchState(Factory.Run());
            }
        }
    }

    public override void InitializeSubState(){
        
    }

}