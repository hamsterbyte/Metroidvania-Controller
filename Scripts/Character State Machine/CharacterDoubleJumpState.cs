using Godot;

public class CharacterDoubleJumpState : CharacterBaseState{
    public CharacterDoubleJumpState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
    }
    public override void EnterState(){
        
        Jump();
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

    private void Jump(){
        CancelVelocity();
        velocity = Vector2.Zero;
        Context.JumpTimer = Context.timeToJumpApex;
        Context.IsJumping = true;
        velocity.Y = Context.JumpVelocity;
        Context.CurrentJumps++;
        Context.DidJump = false;
        AddImpulse(velocity);
    }

}