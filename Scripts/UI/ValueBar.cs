using Godot;

/// <summary>
/// This is the controller class for the value bar
/// Assign the modification methods in this class to events or signals for use
/// </summary>
public partial class ValueBar : NinePatchRect{
    #region MEMBERS

    [Export] private float _animationSpeed = .5f;
    [Export] private Color _overlayColor = Colors.White;
    [Export] private Color _increasingColor = Color.FromHtml("#72dcbb");
    [Export] private Color _decreasingColor = Color.FromHtml("#ee6a7c");
    private ColorRect _modifiedValueRect;
    private ColorRect _currentValueRect;

    private double _maxValue; //Max value; Length of pixels of the bar
    private double _currentValue; //Actual value//
    private double _modifiedValue; //Visual overlay value//
    private double _targetValue; //Target value to move towards//

    #endregion

    #region GODOT

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        GatherRequirements();
        ApplyDefaults();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        //REMOVE INPUT BEFORE USE; TESTING ONLY
        if (Input.IsActionJustPressed("Jump")){
            ModifyValue(-100f);
        }

        if (Input.IsActionJustPressed("Dive")){
            ModifyValue(100f);
        }

        if (Input.IsActionJustPressed("Run")){
            IncreaseMaxValue(100);
        }

        AnimateValue();
    }

    #endregion

    #region INITIALIZE

    /// <summary>
    /// Gather the requirements for the control to function
    /// </summary>
    private void GatherRequirements(){
        _modifiedValueRect = GetNode<ColorRect>("Modified Value");
        _currentValueRect = GetNode<ColorRect>("Current Value");
    }

    /// <summary>
    /// Apply default values to the control
    /// </summary>
    private void ApplyDefaults(){
        _modifiedValueRect.Color = _overlayColor;
        _maxValue = Size.X;
        _modifiedValue = _maxValue;
        _targetValue = _maxValue;
        _currentValue = _maxValue;
    }

    #endregion

    #region MODIFY BAR

    /// <summary>
    /// Modify the target value of the bar, current value will respond accordingly
    /// </summary>
    /// <param name="amount"></param>
    protected void ModifyValue(float amount){
        _targetValue = Mathf.Clamp(_targetValue + amount, 0, _maxValue);
        if (_targetValue < _modifiedValue){
            _modifiedValue = _targetValue;
            _currentValueRect.Color = _decreasingColor;
        }
        else if (_targetValue > _currentValue){
            _currentValue = _targetValue;
            _currentValueRect.Color = _increasingColor;
        }
    }

    /// <summary>
    /// Increase the max value of the bar
    /// </summary>
    /// <param name="amount"></param>
    protected void IncreaseMaxValue(int amount){
        Vector2 currentSize = Size;
        currentSize.X = Mathf.Clamp(currentSize.X + amount, 64, 1000);
        _maxValue = currentSize.X;
        _targetValue = _maxValue;
        _currentValue = _targetValue;
        _currentValueRect.Size = new Vector2((float)_maxValue, _currentValueRect.Size.Y);
        _currentValueRect.Color = _increasingColor;
        Size = currentSize;
    }

    #endregion

    #region ANIMATION

    /// <summary>
    /// Animate the two bars
    /// </summary>
    private void AnimateValue(){
        if (_currentValue == _modifiedValue) return;
        AnimateCurrent();
        AnimateModified();
    }

    /// <summary>
    /// Animate the modified bar towards the target value
    /// </summary>
    private void AnimateModified(){
        Vector2 modifiedSize = _modifiedValueRect.Size;
        _modifiedValue = Mathf.MoveToward(_modifiedValue, _targetValue, _animationSpeed);
        modifiedSize.X = (float)_modifiedValue;
        _modifiedValueRect.Size = modifiedSize;
    }

    /// <summary>
    /// Animate the current bar towards the target value
    /// </summary>
    private void AnimateCurrent(){
        Vector2 currentSize = _currentValueRect.Size;
        _currentValue = Mathf.MoveToward(_currentValue, _targetValue, _animationSpeed);
        currentSize.X = (float)_currentValue;
        _currentValueRect.Size = currentSize;
    }

    #endregion
}