using Godot;

public class CharacterAirborneState : CharacterBaseState, IRootState{
    public CharacterAirborneState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
        IsRootState = true;
    }

    public override void EnterState(){
        GD.Print("Enter Airborne");
        InitializeSubState();
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        GD.Print("Exit Airborne");
    }

    public override void CheckSwitchStates(){
        if (Context.Grounded){
            SwitchState(Factory.Grounded());
        }
        CheckSwitchSubState();
    }

    public override void InitializeSubState(){
        CheckSwitchSubState();
    }

    public void CheckSwitchSubState(){
        if (Context.Velocity.Y >= 0){
            //Falling
            SetSubState(Factory.Fall());
        }
    }
    
    /// <summary>
    /// Calculate horizontal velocity to apply to the character
    /// </summary>
    protected override void CalculateVelocityX(ref Vector2 vel){
        //Calculate target velocity
        float targetVelocity = Context.IsRunPressed
            ? Context.MoveInput.X * Context.moveSpeed * Context.runSpeedMultiplier
            : Context.MoveInput.X * Context.moveSpeed;

        if (Context.MoveInput.X != 0){
            vel.X = Mathf.MoveToward(
                vel.X,
                targetVelocity,
                Context.MoveInput.X == Mathf.Sign(vel.X) ? Context.airAcceleration : Context.airDeceleration
            );
        }
        else{
            vel.X = Mathf.MoveToward(Context.Velocity.X, 0, Context.airDeceleration);
        }
    }
}