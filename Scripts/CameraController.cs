using Godot;
using System;

public partial class CameraController : Camera2D{
    [Export] public CharacterBody2D body;
    [Export] public float horizontalPad = 2f;
    [Export] public float verticalPad = 6f;

    private float _horizontalSize;
    private float _verticalSize;
    private float _yMin, _yMax, _xMin, _xMax;
    private float RelativeMinY => Position.Y;
    private float RelativeMaxY => Position.Y + _verticalSize;
    private float RelativeMinX => Position.X;
    private float RelativeMaxX => Position.X + _horizontalSize;

    public override void _Ready(){
        CalculateScreenSize();
    }

    public override void _Process(double delta){
        UpdateCameraPosition();
    }

    private void CalculateScreenSize(){
        _horizontalSize = Mathf.Abs(GetViewportRect().Size.X / Zoom.X);
        _verticalSize = Mathf.Abs(GetViewportRect().Size.Y / Zoom.Y);

        _yMin = _verticalSize / 2;
        _yMax = -_verticalSize / 2;
        _xMin = -_horizontalSize / 2;
        _xMax = _horizontalSize / 2;
    }

    private void UpdateCameraPosition(){
        if (body.Position.X >= RelativeMaxX){
            Vector2 targetPosition = new Vector2(Position.X + _horizontalSize + horizontalPad, Position.Y);
            Position = targetPosition;
        }

        if (body.Position.X <= RelativeMinX){
            Vector2 targetPosition = new Vector2(Position.X - _horizontalSize - horizontalPad, Position.Y);
            Position = targetPosition;
        }

        if (body.Position.Y >= RelativeMaxY){
            Vector2 targetPosition = new Vector2(Position.X, Position.Y + _verticalSize - verticalPad);
            Position = targetPosition;
        }

        if (body.Position.Y <= RelativeMinY){
            Vector2 targetPosition = new Vector2(Position.X, Position.Y - _verticalSize + verticalPad);
            Position = targetPosition;
        }
    }
}