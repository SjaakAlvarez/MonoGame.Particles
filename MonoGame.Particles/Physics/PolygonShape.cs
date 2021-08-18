using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class PolygonShape : Shape
    {
        const int MaxPolyVertexCount = 128;

        public int m_vertexCount;
        public Vector2[] m_vertices = new Vector2[MaxPolyVertexCount];
        public Vector2[] m_normals = new Vector2[MaxPolyVertexCount];
        public Vector2 Centroid { get; set; }

        public override void Initialize(float density = 1.0f)
        {
            ComputeMass(density);
        }

        public override AABB getAABB(float orient)
        {
            Matrix m = Matrix.CreateRotationZ(-orient);
            float minx = float.MaxValue, miny = float.MaxValue;
            float maxx = float.MinValue, maxy = float.MinValue;
            for (int i = 0; i < m_vertexCount; i++)
            {
                Vector2 v = m_vertices[i];

                v = VectorMath.Mult(m, v);

                if (v.X < minx) minx = v.X;
                if (v.Y < miny) miny = v.Y;
                if (v.X > maxx) maxx = v.X;
                if (v.Y > maxy) maxy = v.Y;
            }

            return new AABB(new Vector2(minx, miny), new Vector2(maxx, maxy));
        }

        public void ComputeMass(float density)
        {
            // Calculate centroid and moment of interia
            Vector2 c = Vector2.Zero; // centroid
            float area = 0.0f;
            float I = 0.0f;
            const float k_inv3 = 1.0f / 3.0f;

            for (int i1 = 0; i1 < m_vertexCount; ++i1)
            {
                // Triangle vertices, third vertex implied as (0, 0)
                Vector2 p1 = m_vertices[i1];
                int i2 = i1 + 1 < m_vertexCount ? i1 + 1 : 0;
                Vector2 p2 = m_vertices[i2];

                float D = VectorMath.Cross(p1, p2);
                float triangleArea = 0.5f * D;

                area += triangleArea;

                // Use area to weight the centroid average, not just vertex position
                c += triangleArea * k_inv3 * (p1 + p2);

                float intx2 = p1.X * p1.X + p2.X * p1.X + p2.X * p2.X;
                float inty2 = p1.Y * p1.Y + p2.Y * p1.Y + p2.Y * p2.Y;
                I += (0.25f * k_inv3 * D) * (intx2 + inty2);
            }

            c *= 1.0f / area;
            this.Centroid = c;
            // Translate vertices to centroid (make the centroid (0, 0)
            // for the polygon in model space)
            // Not really necessary, but I like doing this anyway
            for (int i = 0; i < m_vertexCount; ++i)
                m_vertices[i] -= c;

            Body.m = density * area;
            Body.im = (Body.m > 0) ? 1.0f / Body.m : 0.0f;
            Body.I = I * density;
            Body.iI = Body.I > 0 ? 1.0f / Body.I : 0.0f;
        }

        public override void SetOrientation(float radians)
        {
            float c = (float)Math.Cos(radians);
            float s = (float)Math.Sin(radians);
            u.M11 = c;
            u.M12 = -s;
            u.M21 = s;
            u.M22 = c;
        }


        // Half width and half height
        public void SetBox(float hw, float hh)
        {
            m_vertexCount = 4;
            m_vertices[0] = new Vector2(-hw, -hh);
            m_vertices[1] = new Vector2(hw, -hh);
            m_vertices[2] = new Vector2(hw, hh);
            m_vertices[3] = new Vector2(-hw, hh);
            m_normals[0] = new Vector2(0.0f, -1.0f);
            m_normals[1] = new Vector2(1.0f, 0.0f);
            m_normals[2] = new Vector2(0.0f, 1.0f);
            m_normals[3] = new Vector2(-1.0f, 0.0f);
        }

        public void Set(Vector2[] vertices, int count)
        {

            count = Math.Min(count, MaxPolyVertexCount);

            // Find the right most point on the hull
            int rightMost = 0;
            float highestXCoord = vertices[0].X;
            for (int i = 1; i < count; ++i)
            {
                float x = vertices[i].X;
                if (x > highestXCoord)
                {
                    highestXCoord = x;
                    rightMost = i;
                }

                // If matching x then take farthest negative y
                else if (x == highestXCoord)
                    if (vertices[i].Y < vertices[rightMost].Y)
                        rightMost = i;
            }

            int[] hull = new int[MaxPolyVertexCount];
            int outCount = 0;
            int indexHull = rightMost;

            for (; ; )
            {
                hull[outCount] = indexHull;

                // Search for next index that wraps around the hull
                // by computing cross products to find the most counter-clockwise
                // vertex in the set, given the previos hull index
                int nextHullIndex = 0;
                for (int i = 1; i < count; ++i)
                {
                    // Skip if same coordinate as we need three unique
                    // points in the set to perform a cross product
                    if (nextHullIndex == indexHull)
                    {
                        nextHullIndex = i;
                        continue;
                    }

                    // Cross every set of three unique vertices
                    // Record each counter clockwise third vertex and add
                    // to the output hull
                    // See : http://www.oocities.org/pcgpe/math2d.html
                    Vector2 e1 = vertices[nextHullIndex] - vertices[hull[outCount]];
                    Vector2 e2 = vertices[i] - vertices[hull[outCount]];
                    float c = VectorMath.Cross(e1, e2);
                    if (c < 0.0f)
                        nextHullIndex = i;

                    // Cross product is zero then e vectors are on same line
                    // therefor want to record vertex farthest along that line
                    if (c == 0.0f && e2.LengthSquared() > e1.LengthSquared())
                        nextHullIndex = i;
                }

                ++outCount;
                indexHull = nextHullIndex;

                // Conclude algorithm upon wrap-around
                if (nextHullIndex == rightMost)
                {
                    m_vertexCount = outCount;
                    break;
                }
            }

            // Copy vertices into shape's vertices
            for (int i = 0; i < m_vertexCount; ++i)
                m_vertices[i] = vertices[hull[i]];

            // Compute face normals
            for (int i1 = 0; i1 < m_vertexCount; ++i1)
            {
                int i2 = i1 + 1 < m_vertexCount ? i1 + 1 : 0;
                Vector2 face = m_vertices[i2] - m_vertices[i1];

                // Calculate normal with 2D cross product between vector and scalar
                m_normals[i1] = new Vector2(face.Y, -face.X);
                m_normals[i1] = VectorMath.Normalize(m_normals[i1]);
            }
        }

        // The extreme point along a direction within a polygon
        public Vector2 GetSupport(Vector2 dir)
        {
            float bestProjection = float.MinValue;
            Vector2 bestVertex = Vector2.Zero;

            for (int i = 0; i < m_vertexCount; ++i)
            {
                Vector2 v = m_vertices[i];
                float projection = VectorMath.Dot(v, dir);

                if (projection > bestProjection)
                {
                    bestVertex = v;
                    bestProjection = projection;
                }
            }

            return bestVertex;
        }

        public override Object Clone()
        {
            PolygonShape s = new PolygonShape();
            m_normals.CopyTo(s.m_normals, 0);
            m_vertices.CopyTo(s.m_vertices, 0);
            s.m_vertexCount = m_vertexCount;
            return s;
        }
    }


}
