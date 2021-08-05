using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class Contact
    {        
        private readonly Body A;
        private readonly Body B;

        public float penetration;     // Depth of penetration from collision
        public Vector2 normal;          // From A to B
        public Vector2[] contacts=new Vector2[2];     // Points of contact during collision
        public int contact_count; // Number of contacts that occured during collision
        private float e;               // Mixed restitution
        private float df;              // Mixed dynamic friction
        private float sf;              // Mixed static friction

        //TODO fix this
        const float dt = 1.0f / 60.0f;        

        public Contact(Body a, Body b)
        {
            A = a;
            B = b;
        }

        public void Solve()
        {
            if (!A.IsParticle || !B.IsParticle)
            {                
                if (A.shape is Circle && B.shape is Circle)
                {
                    Collision.CircleToCircle(this, A, B);
                }
                else if (A.shape is PolygonShape && B.shape is Circle)
                {
                    Collision.PolygontoCircle(this, A, B);
                }
                else if (A.shape is Circle && B.shape is PolygonShape)
                {
                    Collision.CircletoPolygon(this, A, B);
                }
                else if (A.shape is PolygonShape && B.shape is PolygonShape)
                {
                    Collision.PolygontoPolygon(this, A, B);
                }                
            }
        }       

        private bool Equal(float a, float b)
        {
            // <= instead of < for NaN comparison safety
            return Math.Abs(a - b) <= VectorMath.EPSILON;
        }        

        public void Initialize()
        {
            // Calculate average restitution
            e = Math.Min(A.restitution, B.restitution);

            // Calculate static and dynamic friction
            sf = (float)Math.Sqrt(A.staticFriction * B.staticFriction);
            df = (float)Math.Sqrt(A.dynamicFriction * B.dynamicFriction);

            for (int i = 0; i < contact_count; ++i)
            {
                // Calculate radii from COM to contact
                Vector2 ra = contacts[i] - A.position;
                Vector2 rb = contacts[i] - B.position;

                Vector2 rv = B.velocity + VectorMath.Cross(B.angularVelocity, rb) -
                          A.velocity - VectorMath.Cross(A.angularVelocity, ra);


                // Determine if we should perform a resting collision or not
                // The idea is if the only thing moving this object is gravity,
                // then the collision should be performed without any restitution
                if (rv.LengthSquared() < (dt * VectorMath.gravity).LengthSquared() + VectorMath.EPSILON)
                    e = 0.0f;
            }
        }

        public void ApplyImpulse()
        {
            // Early out and positional correct if both objects have infinite mass
            if (Equal(A.im + B.im, 0))
            {
                InfiniteMassCorrection();
                return;
            }

            for (int i = 0; i < contact_count; ++i)
            {
                // Calculate radii from COM to contact
                Vector2 ra = contacts[i] - A.position;
                Vector2 rb = contacts[i] - B.position;

                // Relative velocity
                Vector2 rv = B.velocity + VectorMath.Cross(B.angularVelocity, rb) -
                          A.velocity - VectorMath.Cross(A.angularVelocity, ra);

                // Relative velocity along the normal
                float contactVel = VectorMath.Dot(rv, normal);

                // Do not resolve if velocities are separating
                if (contactVel > 0)
                    return;

                float raCrossN = VectorMath.Cross(ra, normal);
                float rbCrossN = VectorMath.Cross(rb, normal);
                float invMassSum = A.im + B.im + (float)Math.Pow(raCrossN,2) * A.iI + (float)Math.Pow(rbCrossN,2) * B.iI;

                // Calculate impulse scalar
                float j = -(1.0f + e) * contactVel;
                j /= invMassSum;
                j /= (float)contact_count;

                // Apply impulse
                Vector2 impulse = normal * j;
                A.ApplyImpulse(-impulse, ra);
                B.ApplyImpulse(impulse, rb);

                // Friction impulse
                rv = B.velocity + VectorMath.Cross(B.angularVelocity, rb) -
                     A.velocity - VectorMath.Cross(A.angularVelocity, ra);

                Vector2 t = rv - (normal * VectorMath.Dot(rv, normal));
                t=VectorMath.Normalize(t);

                

                // j tangent magnitude
                float jt = -VectorMath.Dot(rv, t);
                jt /= invMassSum;
                jt /= (float)contact_count;

                // Don't apply tiny friction impulses
                if (Equal(jt, 0.0f))
                    return;

                // Coulumb's law
                Vector2 tangentImpulse;
                if (Math.Abs(jt) < j * sf)
                    tangentImpulse = t * jt;
                else
                    tangentImpulse = t * -j * df;

                // Apply friction impulse
                A.ApplyImpulse(-tangentImpulse, ra);
                B.ApplyImpulse(tangentImpulse, rb);
            }
        }

        public void PositionalCorrection()
        {
            const float k_slop = 0.05f; // Penetration allowance
            const float percent = 0.4f; // Penetration percentage to correct
            Vector2 correction = (Math.Max(penetration - k_slop, 0.0f) / (A.im + B.im)) * normal * percent;            
            A.position -= correction * A.im;
            B.position += correction * B.im;            
        }        

        public void InfiniteMassCorrection()
        {
            A.velocity=Vector2.Zero;
            B.velocity=Vector2.Zero;
        }


    }
}
