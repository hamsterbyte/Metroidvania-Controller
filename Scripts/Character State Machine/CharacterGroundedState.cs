using Godot;

public class CharacterGroundedState : CharacterBaseState, IRootState{
    public CharacterGroundedState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
        base(currentContext, characterStateManager){
        IsRootState = true;
    }

    public override void EnterState(){
        InitializeSubState();
        ResetAbilities();
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        
    }

    public override void CheckSwitchStates(){
        if (!Context.Grounded){
            SwitchState(Manager.Airborne());
            return;
        }
        CheckSwitchSubState();
    }

    public sealed override void InitializeSubState(){
        CheckSwitchSubState();
    }

    public void CheckSwitchSubState(){
        if (Context.DidJump){
            SetSubState(Manager.Jump());
        }
        else if(!Context.IsJumping){
            if (Context.Velocity.X == 0){
                SetSubState(Manager.Idle());
            }

            if (Context.Velocity.X != 0) SetSubState(Context.IsRunPressed ? Manager.Run() : Manager.Walk());
        }

        
    }

    protected override void CalculateVelocityX(ref Vector2 vel){
        //Calculate target velocity
        float targetVelocity = Context.IsRunPressed
            ? Context.MoveInput.X * Context.moveSpeed * Context.runSpeedMultiplier
            : Context.MoveInput.X * Context.moveSpeed;

        if (Context.MoveInput.X != 0){
            vel.X = Mathf.MoveToward(
                vel.X,
                targetVelocity,
                Context.MoveInput.X == Mathf.Sign(vel.X) ? Context.acceleration : Context.deceleration
            );
        }
        else{
            vel.X = Mathf.MoveToward(Context.Velocity.X, 0, Context.deceleration);
        }
    }

    /// <summary>
    /// Reset abilities that should be reset when the character becomes grounded
    /// </summary>
    private void ResetAbilities(){
        //Reset current jump counter
        Context.CurrentJumps = 0;
        Context.canWallCling = true;
    }
}