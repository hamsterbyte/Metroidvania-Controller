using Godot;

public class CharacterSlideState : CharacterBaseState{
    public CharacterSlideState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
    }

    public override void EnterState(){
        Slide();
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

    private void Slide(){
        CancelVelocity(true);
        Context.IsDashing = true;
        Context.DidDash = false;
        velocity = new Vector2(Context.DirectionX * Context.DashForce, 0);
        AddImpulse(velocity);
    }
}