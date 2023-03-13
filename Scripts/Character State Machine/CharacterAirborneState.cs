using Godot;

public class CharacterAirborneState : CharacterBaseState, IRootState{
    public CharacterAirborneState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
        IsRootState = true;
    }

    public override void EnterState(){
        InitializeSubState();
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
    }

    public override void CheckSwitchStates(){
        CheckSwitchSubState();

        if (Context.Grounded){
            SwitchState(Manager.Grounded());
        }

        if (Context.OnWall && Context.canWallCling){
            SwitchState(Manager.WallCling());
        }
    }

    public override void InitializeSubState(){
        CheckSwitchSubState();
    }

    public void CheckSwitchSubState(){
        if (Context.DidDive){
            SetSubState(Manager.Dive());
        }

        if (Context.DidDash){
            SetSubState(Manager.Dash());
        }

        if (Context.IsDashing) return;

        if (Context.DidJump){
            //Jumping
            SetSubState(Manager.DoubleJump());
        }

        if (!Context.IsJumping || Context.Velocity.Y > 0){
            SetSubState(Manager.Fall());
        }
    }


    /// <summary>
    /// Calculate horizontal velocity to apply to the character; uses different acceleration/deceleration values than when grounded
    /// </summary>
    protected override void CalculateVelocityX(ref Vector2 vel){
        if (Context.IsDashing) return;
        //Calculate target velocity
        float targetVelocity = Context.IsRunPressed
            ? Context.DirectionX * Context.moveSpeed * Context.runSpeedMultiplier
            : Context.DirectionX * Context.moveSpeed;

        if (Context.DirectionX != 0){
            vel.X = Mathf.MoveToward(
                vel.X,
                targetVelocity,
                Context.DirectionX == Mathf.Sign(vel.X) ? Context.airAcceleration : Context.airDeceleration
            );
        }
        else{
            vel.X = Mathf.MoveToward(Context.Velocity.X, 0, Context.airDeceleration);
        }
    }

    /// <summary>
    /// Calculate vertical velocity to apply to the character
    /// </summary>
    protected override void CalculateVelocityY(ref Vector2 vel, double delta){
        ApplyGravity(delta, ref vel);
    }

    /// <summary>
    /// Apply gravity to the character
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="vel"></param>
    private void ApplyGravity(double delta, ref Vector2 vel){
        if (Context.IsDashing) return;
        //Smooth gravity using Verlet integration for improved accuracy
        Context.PreviousVelocity = vel;
        vel.Y += Context.Gravity * (float)delta;
        float appliedVelocityY = (vel.Y + Context.PreviousVelocity.Y) * 0.5f;
        //Clamp applied velocity to max fall speed
        vel.Y = Mathf.Clamp(appliedVelocityY, -Context.maxFallSpeed, Context.maxFallSpeed);
    }
}