using Godot;

public class CharacterDiveState : CharacterBaseState{
    public CharacterDiveState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
    }

    public override void EnterState(){
        Dive();
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

    private void Dive(){
        CancelVelocity();
        Context.DidDive = false;
        velocity = Vector2.Down * Context.DashForce * 2;
        AddImpulse(velocity);
    }
}