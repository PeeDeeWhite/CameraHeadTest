// ReSharper disable CompareOfFloatsByEqualityOperator  - For simplicity allow floating point equality

namespace Vitec.CameraHead.Models;

using System;

/// <summary>
///     Immutable class representing a Pan/Tilt position in degrees
/// </summary>
public sealed class Position(double pan, double tilt)
{
    public double Pan { get; } = pan;

    public double Tilt { get; } = tilt;

    public override int GetHashCode()
    {
        return HashCode.Combine(Pan, Tilt);
    }

    public override bool Equals(object obj)
    {
        if (obj is Position otherPosition) return Equals(otherPosition);

        return false;
    }

    private bool Equals(Position other)
    {
        return other != null && other.Pan == Pan && other.Tilt == Tilt;
    }

    public override string ToString()
    {
        return $"[{Pan:N4}, {Tilt:N4}]";
    }
}