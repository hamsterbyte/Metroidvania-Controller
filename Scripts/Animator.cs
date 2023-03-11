using Godot;
using System;

public partial class Animator : Node2D{
    #region GODOT

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        InitializeStates();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        _sprite.FlipH = _controller.Velocity.X < 0;
        switch (_currentState){
            case AnimationStates.Idle:
                _sprite.Play("Idle");
                break;
            case AnimationStates.Walk:
                _sprite.Play("Walk");
                break;
            case AnimationStates.Run:
                _sprite.Play("Walk", 2f);
                break;
            case AnimationStates.Jump:
                _sprite.Play("Jump");
                break;
            case AnimationStates.DoubleJump:
                _sprite.Play("Double Jump");
                break;
            case AnimationStates.Fall:
                _sprite.Play("Fall");
                break;
            case AnimationStates.WallSlide:
                _sprite.Play("Wall Slide");
                break;
            case AnimationStates.Hit:
                _sprite.Play("Hit");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    #region STATES

    private AnimationStates _currentState;
    private AnimationStates _previousState;
    private AnimatedSprite2D _sprite;
    private CharacterStateMachine _controller;

    private void InitializeStates(){
        _sprite = GetParent().GetNode<AnimatedSprite2D>("Sprite");
        _controller = GetParent() as CharacterStateMachine;
        _currentState = AnimationStates.Idle;
        if (_controller != null) _controller.onStateChanged += SwitchState;
    }

    private void SwitchState(AnimationStates newState){
        _previousState = _currentState;
        _currentState = newState;
    }

    #endregion
}