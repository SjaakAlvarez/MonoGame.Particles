using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Particles.Particles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace MonoGame.Particles.Physics
{
    public class World
    {
        private readonly int m_iterations;
        public List<Body> bodies = new List<Body>(100);
        public List<Contact> contacts = new List<Contact>(100);
        public List<PhysicsEmitter> emitters = new List<PhysicsEmitter>(10);
        private SpatialHash hash;

        public Vector2 WorldSize { get; }
        public int CellSize { get; }

        public World(int iterations, Vector2 worldSize, int cellSize)
        {
            this.m_iterations = iterations;
            this.WorldSize = worldSize;
            this.CellSize = cellSize;
            hash = new SpatialHash((int)worldSize.X, (int)worldSize.Y, cellSize);

        }

        private void IntegrateForces(Body b, float dt)
        {
            if (b.im == 0.0f)
            {
                return;
            }

            float dampening = VectorMath.Clamp(1.0f - dt * b.LinearDamping, 0.0f, 1.0f);

            b.Velocity += (b.Force * b.im + (b.IgnoreGravity ? Vector2.Zero : VectorMath.gravity)) * (dt / 2.0f);            
            b.Velocity *= dampening;
            b.AngularVelocity += b.Torque * b.iI * (dt / 2.0f);
            b.AngularVelocity *= dampening;

        }

        private void IntegrateVelocity(Body b, float dt)
        {
            if (b.im == 0.0f)
            {
                return;
            }
           
            b.Position += b.Velocity * dt;
            b.Orientation += b.AngularVelocity * dt;
            b.SetOrientation(b.Orientation);
            IntegrateForces(b, dt);
        }


        public bool IsBodySharingAnyCell(Body b)
        {
            return hash.isBodySharingAnyCell(b);
        }

        public void AddBody(Body body)
        {
            bodies.Add(body);
            hash.addBody(body);
        }

        public void RemoveBody(Body body)
        {
            bodies.Remove(body);
            hash.removeBody(body);
        }

        //deltaTime is seconds
        public void Step(double deltaTime)
        {            
            //use a copy, because we want to delete emitters in the update           
            foreach (PhysicsEmitter e in emitters.ToArray()) e.Update(deltaTime); 

            foreach (Body b in bodies)
            {
                b.calculateAABB();
                hash.updateBody(b);
            }

            contacts.Clear();

            
            foreach (Body A in bodies)
            {                               
                List<Body> collidingWith = hash.getAllBodiesSharingCellsWithBody(A);
               
                foreach (Body B in collidingWith)
                {
                    if (A.im == 0 && B.im == 0)
                        continue;

                    Contact m = new Contact(A, B);
                    m.Solve();
                    if (m.contact_count > 0)
                    {
                        ContactAction action = A.DoCollision(B, m);                        
                        if (action == ContactAction.COLLIDE)
                        {
                            contacts.Add(m);
                        }
                    }
                    
                }
                A.FinishCollisions();

            }
        

            // Integrate forces
            foreach (Body b in bodies) IntegrateForces(b, (float) deltaTime);            

            // Initialize collision
            foreach (Contact c in contacts) c.Initialize();

            // Solve collisions
            for (int j = 0; j < m_iterations; ++j)
            {
                foreach (Contact c in contacts) c.ApplyImpulse();
            }

            // Integrate velocities           
            foreach (Body b in bodies) IntegrateVelocity(b, (float) deltaTime);

            // Correct positions
            foreach (Contact c in contacts) c.PositionalCorrection();

            // Clear all forces
            foreach (Body b in bodies)
            {
                b.Force = Vector2.Zero;
                b.Torque = 0;
            }

}
    }
}
