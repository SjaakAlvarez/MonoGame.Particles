using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class Body
    {
        public Vector2 position;
        public Vector2 velocity;        

        public float angularVelocity;
        public float torque;
        public float orient;
        public Vector2 Force { get; set; }
        
        public float I;  // moment of inertia
        public float iI; // inverse inertia
        public float m;  // mass
        public float im; // inverse masee

        public float staticFriction;
        public float dynamicFriction;
        public float restitution;
        public bool IsParticle;
        public float Age=0;
        public AABB aabb;
        
        
       
        public Shape shape;
        private static Random rand = new Random();

        public List<int> gridIndex { get; set; }

        public Body(Shape shape, Vector2 pos)
        {
            this.shape = shape;
            this.shape.Body = this;
            this.position = pos;
            this.velocity = Vector2.Zero;
            
            orient = rand.Next(-3, 3);
            staticFriction = 0.5f;
            dynamicFriction = 0.3f;
            restitution = 0.2f;
            Force = Vector2.Zero;
            angularVelocity = 0;

            gridIndex = new List<int>();

            this.shape.Initialize();
        }

        public void calculateAABB()
        {
            if(shape is Circle c)
            {
                aabb = c.getAABB(orient);               
            }
            else if(shape is PolygonShape p)
            {
                aabb = p.getAABB(orient);
            }
            aabb.min += position;
            aabb.max += position;
        }

        public void SetOrientation(float radians)
        {
            orient = radians;
            shape.SetOrientation(radians);
            
        }

        public void ApplyForce(Vector2 f)
        {
            Force += f;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {            
            velocity += im * impulse;
            angularVelocity += iI * VectorMath.Cross(contactVector, impulse);
        }

        public void SetStatic()
        {
            I = 0.0f;
            iI = 0.0f;
            m = 0.0f;
            im = 0.0f;
        }

        

    }
}
