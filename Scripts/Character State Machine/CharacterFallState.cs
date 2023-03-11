using Godot;

public class CharacterFallState : CharacterBaseState{
    public CharacterFallState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
    }

    public override void EnterState(){
        GD.Print("Enter Fall");
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){
        GD.Print("Exit Fall");
    }

    public override void CheckSwitchStates(){
    }

    public override void InitializeSubState(){
    }
    

    /// <summary>
    /// Calculate vertical velocity to apply to the character
    /// </summary>
    protected override void CalculateVelocityY(ref Vector2 vel, double delta){
        ApplyGravity(delta, ref vel);
        SetVelocity(vel);
    }

    /// <summary>
    /// Apply gravity to the character
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="vel"></param>
    private void ApplyGravity(double delta, ref Vector2 vel){
        //Smooth gravity using Verlet integration for improved accuracy
        Context.PreviousVelocity = vel;
        vel.Y += Context.Gravity * Context.fallSpeedMultiplier * (float)delta;
        float appliedVelocityY = (vel.Y + Context.PreviousVelocity.Y) * 0.5f;
        //Clamp applied velocity to max fall speed
        vel.Y = Mathf.Clamp(appliedVelocityY, -Context.maxFallSpeed, Context.maxFallSpeed);
    }
}