using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public static class Collision
    {
        

        public static bool BiasGreaterThan(float a, float b)
        {
            const float k_biasRelative = 0.95f;
            const float k_biasAbsolute = 0.01f;
            return a >= b * k_biasRelative + a * k_biasAbsolute;
        }

        


        public static void CircleToCircle(Manifold m, Body a, Body b)
        {
            Circle A = (Circle)a.shape;
            Circle B = (Circle)b.shape;

            // Calculate translational vector, which is normal
            Vector2 normal = b.position - a.position;

            float dist_sqr = normal.LengthSquared();
            float radius = A.radius + B.radius;

            // Not in contact
            if (dist_sqr >= radius * radius)
            {
                m.contact_count = 0;
                return;
            }

            float distance = (float)Math.Sqrt(dist_sqr);

            m.contact_count = 1;

            if (distance == 0.0f)
            {
                m.penetration = A.radius;
                m.normal = new Vector2(1, 0);
                m.contacts[0] = a.position;
            }
            else
            {
                m.penetration = radius - distance;
                m.normal = normal / distance; // Faster than using Normalized since we already performed sqrt
                m.contacts[0] = m.normal * A.radius + a.position;
            }
        }

        public static void CircletoPolygon(Manifold m, Body a, Body b)
        {
            Circle A = (Circle)a.shape;
            PolygonShape B = (PolygonShape)b.shape;

            m.contact_count = 0;

            // Transform circle center to Polygon model space
            Vector2 center = a.position;
            center = VectorMath.Mult(Matrix.Transpose(B.u), (center - b.position));

            // Find edge with minimum penetration
            // Exact concept as using support points in Polygon vs Polygon
            float separation = float.MinValue;
            int faceNormal = 0;
            for (int i = 0; i < B.m_vertexCount; ++i)
            {
                float s = VectorMath.Dot(B.m_normals[i], center - B.m_vertices[i]);

                if (s > A.radius)
                    return;

                if (s > separation)
                {
                    separation = s;
                    faceNormal = i;
                }
            }

            // Grab face's vertices
            Vector2 v1 = B.m_vertices[faceNormal];
            int i2 = faceNormal + 1 < B.m_vertexCount ? faceNormal + 1 : 0;
            Vector2 v2 = B.m_vertices[i2];

            // Check to see if center is within polygon
            if (separation < VectorMath.EPSILON)
            {
                m.contact_count = 1;
                m.normal = -VectorMath.Mult(B.u , B.m_normals[faceNormal]);
                m.contacts[0] = m.normal * A.radius + a.position;
                m.penetration = A.radius;
                return;
            }

            // Determine which voronoi region of the edge center of circle lies within
            float dot1 = VectorMath.Dot(center - v1, v2 - v1);
            float dot2 = VectorMath.Dot(center - v2, v1 - v2);
            m.penetration = A.radius - separation;

            // Closest to v1
            if (dot1 <= 0.0f)
            {
                if (VectorMath.DistSqr(center, v1) > A.radius * A.radius)
                    return;

                m.contact_count = 1;
                Vector2 n = v1 - center;
                n = VectorMath.Mult(B.u , n);
                n=VectorMath.Normalize(n);
                m.normal = n;
                v1 = VectorMath.Mult(B.u , v1) + b.position;
                m.contacts[0] = v1;
            }

            // Closest to v2
            else if (dot2 <= 0.0f)
            {
                if (VectorMath.DistSqr(center, v2) > A.radius * A.radius)
                    return;

                m.contact_count = 1;
                Vector2 n = v2 - center;
                v2 = VectorMath.Mult(B.u, v2)  + b.position;
                m.contacts[0] = v2;
                n = VectorMath.Mult(B.u , n);
                n=VectorMath.Normalize(n);
                m.normal = n;
            }

            // Closest to face
            else
            {
                Vector2 n = B.m_normals[faceNormal];
                if (VectorMath.Dot(center - v1, n) > A.radius)
                    return;

                n = VectorMath.Mult( B.u ,n);
                m.normal = -n;
                m.contacts[0] = m.normal * A.radius + a.position;
                m.contact_count = 1;
            }
        }

        public static void PolygontoCircle(Manifold m, Body a, Body b)
        {
            CircletoPolygon(m, b, a);
            m.normal = -m.normal;
        }

        public static float FindAxisLeastPenetration(ref int faceIndex, PolygonShape A, PolygonShape B)
        {
            float bestDistance = float.MinValue;
            int bestIndex=0;

            for (int i = 0; i < A.m_vertexCount; ++i)
            {
                // Retrieve a face normal from A
                Vector2 n = A.m_normals[i];
                Vector2 nw = VectorMath.Mult( A.u, n);

                // Transform face normal into B's model space
                Matrix buT = Matrix.Transpose(B.u);
                n = VectorMath.Mult(buT, nw);

                // Retrieve support point from B along -n
                Vector2 s = B.GetSupport(-n);

                // Retrieve vertex on face from A, transform into
                // B's model space
                Vector2 v = A.m_vertices[i];
                v = VectorMath.Mult(A.u, v) + A.Body.position;
                v -= B.Body.position;
                v = VectorMath.Mult(buT , v);

                // Compute penetration distance (in B's model space)
                float d = VectorMath.Dot(n, s - v);

                // Store greatest distance
                if (d > bestDistance)
                {
                    bestDistance = d;
                    bestIndex = i;
                }
            }

            faceIndex = bestIndex;
            return bestDistance;
        }

        public static void FindIncidentFace(Vector2[] v, PolygonShape RefPoly, PolygonShape IncPoly, int referenceIndex)
        {
            Vector2 referenceNormal = RefPoly.m_normals[referenceIndex];

            // Calculate normal in incident's frame of reference
            referenceNormal = VectorMath.Mult(RefPoly.u ,referenceNormal); // To world space
            referenceNormal = VectorMath.Mult(Matrix.Transpose(IncPoly.u), referenceNormal); // To incident's model space

            // Find most anti-normal face on incident polygon
            int incidentFace = 0;
            float minDot = float.MaxValue;
            for (int i = 0; i < IncPoly.m_vertexCount; ++i)
            {
                float dot = VectorMath.Dot(referenceNormal, IncPoly.m_normals[i]);
                if (dot < minDot)
                {
                    minDot = dot;
                    incidentFace = i;
                }
            }

            // Assign face vertices for incidentFace
            v[0] = VectorMath.Mult(IncPoly.u , IncPoly.m_vertices[incidentFace]) + IncPoly.Body.position;
            incidentFace = incidentFace + 1 >= IncPoly.m_vertexCount ? 0 : incidentFace + 1;
            v[1] = VectorMath.Mult(IncPoly.u ,IncPoly.m_vertices[incidentFace]) + IncPoly.Body.position;
        }

        public static int Clip(Vector2 n, float c, Vector2[] face)
        {
            int sp = 0;            
            Vector2[] outface = new Vector2[2] { face[0], face[1] };

            // Retrieve distances from each endpoint to the line
            // d = ax + by - c
            float d1 = VectorMath.Dot(n, face[0]) - c;
            float d2 = VectorMath.Dot(n, face[1]) - c;

            // If negative (behind plane) clip
            if (d1 <= 0.0f) outface[sp++] = face[0];
            if (d2 <= 0.0f) outface[sp++] = face[1];

            // If the points are on different sides of the plane
            if (d1 * d2 < 0.0f) // less than to ignore -0.0f
            {
                // Push interesection point
                float alpha = d1 / (d1 - d2);
                outface[sp] = face[0] + alpha * (face[1] - face[0]);
                ++sp;
            }

            // Assign our new converted values
            face[0] = outface[0];
            face[1] = outface[1];            

            return sp;
        }

        public static void PolygontoPolygon(Manifold m, Body a, Body b)
        {
            PolygonShape A = (PolygonShape)a.shape;
            PolygonShape B = (PolygonShape)b.shape;
            m.contact_count = 0;

            // Check for a separating axis with A's face planes
            int faceA=0;
            float penetrationA = FindAxisLeastPenetration(ref faceA, A, B);
            if (penetrationA >= 0.0f)
                return;

            // Check for a separating axis with B's face planes
            int faceB=0;
            float penetrationB = FindAxisLeastPenetration(ref faceB, B, A);
            if (penetrationB >= 0.0f)
                return;

            int referenceIndex;
            bool flip; // Always point from a to b

            PolygonShape RefPoly; // Reference
            PolygonShape IncPoly; // Incident

            // Determine which shape contains reference face
            if (BiasGreaterThan(penetrationA, penetrationB))
            {
                RefPoly = A;
                IncPoly = B;
                referenceIndex = faceA;
                flip = false;
            }

            else
            {
                RefPoly = B;
                IncPoly = A;
                referenceIndex = faceB;
                flip = true;
            }

            // World space incident face
            Vector2[] incidentFace=new Vector2[2];
            FindIncidentFace(incidentFace, RefPoly, IncPoly, referenceIndex);

            //        y
            //        ^  ->n       ^
            //      +---c ------posPlane--
            //  x < | i |\
            //      +---+ c-----negPlane--
            //             \       v
            //              r
            //
            //  r : reference face
            //  i : incident poly
            //  c : clipped point
            //  n : incident normal

            // Setup reference face vertices
            Vector2 v1 = RefPoly.m_vertices[referenceIndex];
            referenceIndex = referenceIndex + 1 == RefPoly.m_vertexCount ? 0 : referenceIndex + 1;
            Vector2 v2 = RefPoly.m_vertices[referenceIndex];

            // Transform vertices to world space
            v1 = VectorMath.Mult(RefPoly.u, v1) + RefPoly.Body.position;
            v2 = VectorMath.Mult(RefPoly.u,  v2) + RefPoly.Body.position;

            // Calculate reference face side normal in world space
            Vector2 sidePlaneNormal = (v2 - v1);
            sidePlaneNormal= VectorMath.Normalize(sidePlaneNormal);

            // Orthogonalize
            Vector2 refFaceNormal=new Vector2(sidePlaneNormal.Y, -sidePlaneNormal.X );

            // ax + by = c
            // c is distance from origin
            float refC = VectorMath.Dot(refFaceNormal, v1);
            float negSide = -VectorMath.Dot(sidePlaneNormal, v1);
            float posSide = VectorMath.Dot(sidePlaneNormal, v2);

            // Clip incident face to reference face side planes
            if (Clip(-sidePlaneNormal, negSide, incidentFace) < 2)
                return; // Due to floating point error, possible to not have required points

            if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
                return; // Due to floating point error, possible to not have required points

            // Flip
            m.normal = flip ? -refFaceNormal : refFaceNormal;

            // Keep points behind reference face
            int cp = 0; // clipped points behind reference face
            float separation = VectorMath.Dot(refFaceNormal, incidentFace[0]) - refC;
            if (separation <= 0.0f)
            {
                m.contacts[cp] = incidentFace[0];
                m.penetration = -separation;
                ++cp;
            }
            else
                m.penetration = 0;

            separation = VectorMath.Dot(refFaceNormal, incidentFace[1]) - refC;
            if (separation <= 0.0f)
            {
                m.contacts[cp] = incidentFace[1];

                m.penetration += -separation;
                ++cp;

                // Average penetration
                m.penetration /= (float)cp;
            }

            m.contact_count = cp;
        }

    }
}
