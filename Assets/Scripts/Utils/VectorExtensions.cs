
using UnityEngine;

public static class VectorExtensions
{
    public static float ToAngle(this Vector2 vector)
    {
        /*
         * Basic trig:
         * tan (a) = y / x
         * a = arctan (y / x)
         */
        //float angle = Mathf.Atan(vector.y / vector.x);
        float angle = Mathf.Atan2(vector.y, vector.x);
        return angle * Mathf.Rad2Deg;
    }

    public static Vector2 ToAbs(this Vector2 vector)
    {
        Vector2 v = new Vector2();
        v.x = Mathf.Abs(vector.x);
        v.y = Mathf.Abs(vector.y);

        return v;
    }

    public static Vector3 ToAbs(this Vector3 vector)
    {
        Vector3 v = new Vector3();
        v.x = Mathf.Abs(vector.x);
        v.y = Mathf.Abs(vector.y);
        v.z = Mathf.Abs(vector.z);

        return v;
    }

    public static Vector2 ToInt(this Vector2 vector)
    {
        vector.x = (int)vector.x;
        vector.y = (int)vector.y;

        return vector;
    }

    public static Vector3 ToInt(this Vector3 vector)
    {
        vector.x = (int)vector.x;
        vector.y = (int)vector.y;
        vector.z = (int)vector.z;

        return vector;
    }

    /// <summary>
    /// Returns the direction vector based on an angle in degrees.
    /// </summary>
    /// <param name="angle">The angle, in degrees, to get the direction vector from.</param>
    /// <returns></returns>
    public static Vector2 ToDirection(this float angle)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector2(x, y); // No need to normalize, because maths!
    }

    public static Vector2 FlipX(this Vector2 vector)
    {
        vector.x = vector.x * -1f;
        return vector;
    }

    public static Vector2 FlipY(this Vector2 vector)
    {
        vector.y = vector.y * -1f;
        return vector;
    }

    public static void ClampTo(this ref Rect self, Rect container)
    {
        self.x = Mathf.Max(container.x, self.x);
        self.y = Mathf.Max(container.y, self.y);

        self.x = Mathf.Min(container.x + container.width, self.x + self.width) - self.width;
        self.y = Mathf.Min(container.y + container.height, self.y + self.height) - self.height;
    }
}