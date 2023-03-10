using System;
using Godot;

public static class Extensions{
    /// <summary>
    /// Return a raw variant of the input vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2 GetRaw(this ref Vector2 vector){
        Vector2 rawVector = Vector2.Zero;
        rawVector.X = vector.X switch{
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        rawVector.Y = vector.Y switch{
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        return rawVector;
    }

    /// <summary>
    /// Get direction of vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Direction GetDirection(this Vector2 vector){
        return new Direction(vector);
    }
}

/// <summary>
/// Struct for storing and returning cardinal/ordinal directions based on player input
/// </summary>
public readonly struct Direction{
    private readonly Vector2 _vector;
    public Vector2 Vector => _vector;
    private readonly Directions _heading;
    public Directions Heading => _heading;
    private const float DEADZONE = .33f;

    /// <summary>
    /// Default constructor, used to calculate the direction based on an input Vector2
    /// </summary>
    /// <param name="vector"></param>
    public Direction(Vector2 vector){
        Vector2 raw = vector.GetRaw();
        //Set cardinal/ordinal heading based on input vector
        _heading = vector.X switch{
            > DEADZONE => vector.Y switch{
                > DEADZONE => Directions.SouthEast,
                < -DEADZONE => Directions.NorthEast,
                _ => Directions.East
            },
            < -DEADZONE => vector.Y switch{
                > DEADZONE => Directions.SouthWest,
                < -DEADZONE => Directions.NorthWest,
                _ => Directions.West
            },
            _ => vector.Y switch{
                > DEADZONE => Directions.South,
                < -DEADZONE => Directions.North,
                _ => Directions.None
            }
        };
        _vector = Vector2.Zero;
        //Set output vector based on heading
        switch (_heading){
            case Directions.None:
                _vector.X = 0;
                _vector.Y = 0;
                break;
            case Directions.North:
                _vector.X = 0;
                _vector.Y = -1;
                break;
            case Directions.NorthEast:
                _vector.X = 1;
                _vector.Y = -1;
                break;
            case Directions.East:
                _vector.X = 1;
                _vector.Y = 0;
                break;
            case Directions.SouthEast:
                _vector.X = 1;
                _vector.Y = 1;
                break;
            case Directions.South:
                _vector.X = 0;
                _vector.Y = 1;
                break;
            case Directions.SouthWest:
                _vector.X = -1;
                _vector.Y = 1;
                break;
            case Directions.West:
                _vector.X = -1;
                _vector.Y = 0;
                break;
            case Directions.NorthWest:
                _vector.X = -1;
                _vector.Y = -1;
                break;
            default:
                _vector = Vector2.Zero;
                break;
        }
        //Normalize the output vector
        _vector = _vector.Normalized();
    }
}

/// <summary>
/// Enumeration containing the cardinal and intermediate directions
/// </summary>
public enum Directions{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
    None
}