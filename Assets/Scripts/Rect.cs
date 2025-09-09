using UnityEngine;

public class Rect
{
    private Vector2 center;
    private Vector2 dimensions;

    public Rect(Vector2 center, Vector2 dimensions)
    {
        this.center = center;
        this.dimensions = dimensions;
    }

    public Rect(Vector2 topCenter, float height, float width)
    {
        dimensions = new Vector2(width, height);
        center = new Vector2(topCenter.x, topCenter.y - dimensions.y / 2);
    }

    public Rect(float leftX, float rightX, float topY, float height)
    {
        dimensions = new Vector2(rightX - leftX, height);
        center = new Vector2(leftX + dimensions.x / 2, topY - dimensions.y / 2);
    }

    public void ShiftX(float amount)
    {
        center.x += amount;
    }

    public void ShiftY(float amount)
    {
        center.y += amount;
    }

    public void ShiftTopEdgeY(float amount)
    {
        if (GetTopY() + amount < GetBottomY() + 1) amount = GetBottomY() + 1 - GetTopY();
        center.y += amount / 2f;
        dimensions.y += amount;
    }

    public void SetDimensionX(float value)
    {
        dimensions.x = value;
    }

    public float GetTopY()
    {
        return center.y + dimensions.y / 2;
    }

    public float GetBottomY()
    {
        return center.y - dimensions.y / 2;
    }

    public float GetLeftX()
    {
        return center.x - dimensions.x / 2;
    }

    public float GetRightX()
    {
        return center.x + dimensions.x / 2;
    }

    public Vector3 GetCenter()
    {
        return (Vector3)center;
    }

    public Vector3 GetDimensions()
    {
        return new Vector3(dimensions.x, dimensions.y, 1);
    }

    public float GetWidth()
    {
        return dimensions.x;
    }
}
