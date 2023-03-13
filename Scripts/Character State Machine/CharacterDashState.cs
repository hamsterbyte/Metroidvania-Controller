using Godot;

public class CharacterDashState : CharacterBaseState{
    public CharacterDashState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
    }

    public override void EnterState(){
        Dash();
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

    private void Dash(){
        CancelVelocity(true);
        Context.IsDashing = true;
        Context.DidDash = false;
        if (Context.Grounded){
            velocity = new Vector2(Context.DirectionX * Context.DashForce, 0);
        }
        else{
            velocity = Context.MoveInput.GetDirection().Vector * Context.DashForce;
        }

        AddImpulse(velocity);
    }
}