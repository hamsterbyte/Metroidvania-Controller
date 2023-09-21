using System.ComponentModel.Design.Serialization;
using Godot;

public class CharacterWallClingState : CharacterBaseState{
    private int _collisionCount;
    private bool _shouldCancelY;
    private Vector2 _wallNormal;
    private double _wallClingTimer;
    private bool _didWallJump;

    public CharacterWallClingState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
        IsRootState = true;
    }

    public override void EnterState(){
        ResetAbilities();
        GetWallCollisions();
    }

    public override void UpdateState(){
        CheckSwitchSubstates();
        CheckSwitchStates();
    }

    public override void ExitState(){
        
    }

    public override void CheckSwitchStates(){
        if (Context.Grounded){
            SwitchState(Manager.Grounded());
        }
        else if (!Context.OnWall){
            SwitchState(Manager.Airborne());
        }
    }

    private void CheckSwitchSubstates(){
        if (Context.DidJump){
            _didWallJump = true;
            SetSubState(Manager.WallJump());
        }

        if (!Context.DidDash) return;
        Context.DidDash = false;
        Context.ResetDash();
    }

    public override void InitializeSubState(){
    }

    protected override void CalculateVelocityY(ref Vector2 vel, double delta){
        //Cancel any upward movement
        CancelYVelocity(ref vel);
        //Apply wall cling gravity to slide down the wall
        ApplyGravity(delta, ref vel);
        _wallClingTimer -= delta;
    }

    protected override void CalculateVelocityX(ref Vector2 vel){
        if (_didWallJump) return;
        if (_wallClingTimer > 0){
            vel.X = -_wallNormal.X; 
        }
        else{
            //If wall cling timer is exhausted, cannot wall cling again until the character has landed
            Context.canWallCling = false;
            vel.X = _wallNormal.X;
        }
    }

    private void ApplyGravity(double delta, ref Vector2 vel){
        //Smooth gravity using Verlet integration for improved accuracy
        Context.PreviousVelocity = vel;
        vel.Y += Context.Gravity * (float)delta * Context.wallClingGravityModifier;
        float appliedVelocityY = (vel.Y + Context.PreviousVelocity.Y) * 0.5f;
        //Clamp applied velocity to max fall speed
        vel.Y = Mathf.Clamp(appliedVelocityY, -Context.maxFallSpeed, Context.maxFallSpeed);
    }

    private void CancelYVelocity(ref Vector2 vel){
        if (!_shouldCancelY) return;
        vel.Y = 0;
        _shouldCancelY = false;
    }

    private void ResetAbilities(){
        //Reset current jumps when entering wall slide
        Context.CurrentJumps = 0;
        //Allowing Y velocity cancel on wall cling start
        _shouldCancelY = true;
        //Reset wall cling timer to max
        _wallClingTimer = Context.wallClingTime;
        //Reset did wall jump
        _didWallJump = false;
        if (!Context.IsDashing) return;
        Context.ResetDash();
    }

    private void GetWallCollisions(){
        _collisionCount = Context.GetSlideCollisionCount();
        for (int i = 0; i < _collisionCount; i++){
            _wallNormal = Context.GetSlideCollision(i).GetNormal();
        }
    }
}