namespace SpaceBattle.Lib;

public sealed class Vector : IEquatable<Vector>
{
    private readonly int[] _coordinates;

    public Vector(params int[] coordinates)
    {
        _coordinates = coordinates?.ToArray() ?? throw new ArgumentNullException(nameof(coordinates));

        if (_coordinates.Length == 0) throw new ArgumentException("Vector cannot be empty");
    }
    public int Dimension => _coordinates.Length;
    public int this[int index] => _coordinates[index];

    public static Vector operator +(Vector x, Vector y)
    {
        if (x.Dimension != y.Dimension) throw new ArgumentException("Vectors must have same dimensions");

        return new Vector(x._coordinates.Zip(y._coordinates, (a, b) => a + b).ToArray());
    }
    public static bool operator ==(Vector? x, Vector? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x._coordinates.SequenceEqual(y._coordinates);
    }
    public static bool operator !=(Vector x, Vector y) => !(x == y);
    public bool Equals(Vector? other_vector) => this == other_vector;
    public override bool Equals(object? obj) => obj is Vector other_vector && Equals(other_vector);
    public override int GetHashCode() => _coordinates.Aggregate(17, (hash, val) => unchecked(hash * 23 + val));
}
