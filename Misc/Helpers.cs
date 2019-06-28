using System;
using SFML.System;

namespace Match3.Misc
{
    public static class Helpers
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static Vector2f Normalized(Vector2f v)
        {
            return v / Length(v);
        }

        public static float Length(Vector2f v)
        {
            return (float) Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }
    }
}
