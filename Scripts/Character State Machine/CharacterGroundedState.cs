using Godot;

public class CharacterGroundedState : CharacterBaseState, IRootState{
    public CharacterGroundedState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
        IsRootState = true;
    }

    public override void EnterState(){
        GD.Print("Enter Grounded");
        InitializeSubState();
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        GD.Print("Exit Grounded");
    }

    public override void CheckSwitchStates(){
        if (!Context.Grounded && !Context.OnWall){
            SwitchState(Factory.Airborne());
        }
        
        CheckSwitchSubState();
    }

    public sealed override void InitializeSubState(){
        CheckSwitchSubState();
    }

    public void CheckSwitchSubState(){
        if (Context.Velocity.X == 0){
            SetSubState(Factory.Idle());
        }
        else{
            SetSubState(Context.IsRunPressed ? Factory.Run() : Factory.Walk());
        }
    }
}