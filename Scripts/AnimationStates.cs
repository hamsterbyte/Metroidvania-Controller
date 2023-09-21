public enum AnimationStates{
    Idle,
    Walk,
    Run,
    Jump,
    DoubleJump,
    Fall,
    WallSlide,
    Hit
}

public struct AnimationState{
    private string _name;
    private float _speed;
}