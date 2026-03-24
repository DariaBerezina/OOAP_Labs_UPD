namespace SpaceBattle.Lib.Tests;

public class VectorTests
{
    [Fact]
    public void Constructor_CoordinatesAreNull_ThrowsArgumentNullException()
    {
        int[]? nullable = null;
        Assert.Throws<ArgumentNullException>(() => new Vector(nullable!));
    }

    [Fact]
    public void Constructor_VectorIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Vector(Array.Empty<int>()));
    }

    [Fact]
    public void Iterator_GetItemByIndex_Correctly()
    {
        var vector = new Vector(1, 0);
        Assert.Equal(0, vector[1]);
    }

    [Fact]
    public void OperatorPlus_AddVectorsWithDifferentDimensions_ThrowsArgumentException()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 1, 1);

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
    }

    [Fact]
    public void OperatorPlus_AddVectors_Correctly()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(0, -1);

        var sumResult = vector1 + vector2;

        Assert.Equal(new Vector(1, 0), sumResult);
    }

    [Fact]
    public void OperatorDoubleEqual_CompareVectors_EqualVectors()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 1);

        Assert.True(vector1 == vector2);
    }

    [Fact]
    public void OperatorDoubleEqual_CompareVectorWithItself_True()
    {
        var vector = new Vector(1, 1);

#pragma warning disable CS1718
        Assert.True(vector == vector);
    }

    [Fact]
    public void OperatorDoubleEqual_CompareVectorWithNull_ReturnFalse()
    {
        var vector = new Vector(1, 1);
        Vector? nullable = null;

        Assert.False(vector == nullable!);
        Assert.False(nullable! == vector);
    }

    [Fact]
    public void OperatorNotEqual_CompareVectors_NotEqualVectors()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 0);

        Assert.True(vector1 != vector2);
    }

    [Fact]
    public void Equals_CompareVectors_True()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 1);

        Assert.True(vector1.Equals(vector2));
    }

    [Fact]
    public void Equals_CompareVectorWithOtherObject_False()
    {
        var vector = new Vector(1);
        var str = "1";

        Assert.False(vector.Equals(str));
    }

    [Fact]
    public void GetHashCode_CompareVectors_Correctly()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 1);

        Assert.True(vector1.GetHashCode() == vector2.GetHashCode());
    }
    [Fact]
    public void GetHashCode_CompareNotEqualVectors_HashDoesNotMatch()
    {
        var vector1 = new Vector(1, 1);
        var vector2 = new Vector(1, 0);

        Assert.True(vector1.GetHashCode() != vector2.GetHashCode());
    }

}
