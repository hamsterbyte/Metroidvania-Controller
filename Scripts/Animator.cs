using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Animator : Node2D{
    #region GODOT

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        GatherRequirements();
        AssignCallback();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        GatherInput();
        Flip();
        PlayAnimation();
    }

    #endregion
    
    #region INPUT

    private Vector2 _rawInput;
    private void GatherInput(){
        _rawInput = Input.GetVector("Left", "Right", "Up", "Down");
        _rawInput = _rawInput.GetRaw();
    }
    #endregion

    #region STATES

    private CharacterStateMachine _characterStateMachine;
    private AnimatedSprite2D _animatedSprite2D;
    private AudioStreamPlayer _jumpAudio;
    private AudioStreamPlayer _doubleJumpAudio;
    private CharacterStates _animationState;

    /// <summary>
    /// Gather scripts/nodes required for this animator to function
    /// The CharacterStateMachine is not dependent on this script
    /// </summary>
    private void GatherRequirements(){
        _characterStateMachine = GetParent() as CharacterStateMachine;
        _animatedSprite2D = GetParent().GetNode("Sprite") as AnimatedSprite2D;
        _jumpAudio = GetParent().GetNode("Audio Controller").GetNode<AudioStreamPlayer>("Jump");
        _doubleJumpAudio = GetParent().GetNode("Audio Controller").GetNode<AudioStreamPlayer>("Double Jump");
    }
    
    /// <summary>
    /// Assign the state change callback to the CharacterStateMachine
    /// </summary>
    private void AssignCallback(){
        _characterStateMachine.onStateEnter += SwitchState;
    }

    /// <summary>
    /// Set the animation state based on the current CharacterBaseState
    /// in the CharacterStateMachine
    /// </summary>
    /// <param name="state"></param>
    private void SwitchState(CharacterBaseState state){
        _animationState = state switch{
            CharacterIdleState => CharacterStates.Idle,
            CharacterWalkState => CharacterStates.Walk,
            CharacterRunState => CharacterStates.Run,
            CharacterFallState => CharacterStates.Fall,
            CharacterJumpState => CharacterStates.Jump,
            CharacterDoubleJumpState => CharacterStates.DoubleJump,
            CharacterWallJumpState => CharacterStates.WallJump,
            CharacterWallClingState => CharacterStates.WallCling,
            CharacterDashState => CharacterStates.Dash,
            CharacterSlideState => CharacterStates.Slide,
            _ => _animationState
        };
    }
    #endregion
    
    #region ANIMATION

    /// <summary>
    /// Play the animation associated with the current character state
    /// All these animations are controlled by sub states,
    /// but could also be controlled by a root state
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void PlayAnimation(){
        switch (_animationState){
            case CharacterStates.Idle:
                _animatedSprite2D.Play("Idle");
                break;
            case CharacterStates.Walk:
                _animatedSprite2D.Play("Walk");
                break;
            case CharacterStates.Run:
                _animatedSprite2D.Play("Walk", 2);
                break;
            case CharacterStates.Jump:
                _animatedSprite2D.Play("Jump");
                if(_jumpAudio.Playing) return;
                _jumpAudio.Play();
                break;
            case CharacterStates.Fall:
                _animatedSprite2D.Play("Fall");
                break;
            case CharacterStates.WallCling:
                _animatedSprite2D.Play("Wall Slide");
                break;
            case CharacterStates.WallJump:
                _animatedSprite2D.Play("Jump");
                if(_jumpAudio.Playing) return;
                _jumpAudio.Play();
                break;
            case CharacterStates.DoubleJump:
                _animatedSprite2D.Play("Double Jump");
                if (_doubleJumpAudio.Playing) return;
                _doubleJumpAudio.Play();
                break;
            case CharacterStates.Hit:
                _animatedSprite2D.Play("Hit");
                break;
            case CharacterStates.Dash:
                _animatedSprite2D.Play("Dash");
                break;
            case CharacterStates.Slide:
                _animatedSprite2D.Play("Slide");
                break;
        }
    }

    /// <summary>
    /// Flip the character sprite to math the input direction or velocity
    /// </summary>
    private void Flip(){
        if (_animationState == CharacterStates.WallCling){
            //Flip based on velocity if wall clinging
            _animatedSprite2D.FlipH = _characterStateMachine.Velocity.X switch{
                < 0 => true,
                > 0 => false,
                _ => _animatedSprite2D.FlipH
            };
        }
        else{
            //Flip based on input if not wall clinging
            _animatedSprite2D.FlipH = _rawInput.X switch{
                < 0 => true,
                > 0 => false,
                _ => _animatedSprite2D.FlipH
            };
        }
        
    }
    #endregion
}