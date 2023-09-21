using Godot;

public class CharacterWallJumpState : CharacterBaseState{
    private int _collisionCount;
    private Vector2 _wallNormal;
    
    
    public CharacterWallJumpState(CharacterStateMachine currentContext, CharacterStateManager characterStateManager) :
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

    /// <summary>
    /// Calculate Y component of the jump velocity
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    protected override void CalculateVelocityY(ref Vector2 vel, double delta){
        
    }

    /// <summary>
    /// Calculate X component of the velocity
    /// </summary>
    /// <param name="vel"></param>
    protected override void CalculateVelocityX(ref Vector2 vel){
        
    }
    
    private void GetWallCollisions(){
        _collisionCount = Context.GetSlideCollisionCount();
        for (int i = 0; i < _collisionCount; i++){
            _wallNormal = Context.GetSlideCollision(i).GetNormal();
        }
    }

    private void Jump(){
        CancelVelocity();
        velocity = Vector2.Zero;
        Context.JumpTimer = Context.timeToJumpApex;
        Context.IsJumping = true;
        GetWallCollisions();
        velocity.X = Context.JumpVelocity * -_wallNormal.X;
        velocity.Y = Context.JumpVelocity;
        Context.CurrentJumps++;
        Context.DidJump = false;
        AddImpulse(velocity);
    }
}