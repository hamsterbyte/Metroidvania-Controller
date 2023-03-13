using Godot;

public class CharacterJumpState : CharacterBaseState{
    private bool _jumpExecuted;
    public CharacterJumpState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
    }
    public override void EnterState(){
        
        Jump();
        AddImpulse(velocity);
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
    }
}