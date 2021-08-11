using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public static class VectorMath
    {
        public const float EPSILON = 0.0001f;

        public const float gravityScale = 20.0f;
        public static Vector2 gravity = new Vector2(0, 10.0f * gravityScale);

        public static float Clamp(float a, float low, float high)
        {
            return Math.Max(low, Math.Min(a, high));
        }

        public static float Cross(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.Y
                   - value1.Y * value2.X;
        }

        public static Vector2 Cross(float a, Vector2 v)
        {
            return new Vector2(-a * v.Y, a * v.X);           
        }

        public static Vector2 Cross(Vector2 v, float a)
        {
            return new Vector2(a * v.Y, -a * v.X);
        }       

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static float DistSqr(Vector2 a, Vector2 b)
        {
            Vector2 c = a - b;
            return Dot(c, c);
        }

        public static Vector2 Mult(Matrix m, Vector2 v)
        {
            return new Vector2(m.M11 * v.X + m.M12 * v.Y, m.M21 * v.X + m.M22 * v.Y);
        }

        public static Matrix Transpose(Matrix m)
        {
            Matrix r =new Matrix();
            r.M11 = m.M11;
            r.M12 = m.M21;
            r.M21 = m.M12;
            r.M22 = m.M22;
            return r;
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float len = v.Length();
            if (len > EPSILON)
            {
                float invLen = 1.0f / len;
                v.X *= invLen;
                v.Y *= invLen;
                return v;
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }
}
