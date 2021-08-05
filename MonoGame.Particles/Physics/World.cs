using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class World
    {        
        private readonly int m_iterations;
        public List<Body> bodies=new List<Body>();
        public List<Contact> contacts = new List<Contact>();        
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

            if (b.Force.Length() < 1) b.Force = Vector2.Zero;

            float dampening = VectorMath.Clamp(1.0f - dt * b.LinearDamping, 0.0f, 1.0f);

            b.velocity += (b.Force * b.im + (b.IgnoreGravity?Vector2.Zero:VectorMath.gravity)) * (dt / 2.0f);
            b.velocity *= dampening;
            b.angularVelocity += b.torque * b.iI * (dt / 2.0f);
        }

        private void IntegrateVelocity(Body b, float dt)
        {
            if (b.im == 0.0f)
            {
                return;
            }            

            b.position += b.velocity * dt;
            b.orientation += b.angularVelocity * dt;
            b.SetOrientation(b.orientation);
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

        public void Step(double deltaTtime)
        {            
            foreach(Body b in bodies)
            {
                b.calculateAABB();
                hash.updateBody(b);
            }
            
            contacts.Clear();
            
            for (int i = 0; i < bodies.Count; ++i)
            {
                Body A = bodies[i];
                List<Body> collidingWith = hash.getAllBodiesSharingCellsWithBody(A);
                foreach (Body B in collidingWith)
                {
                    if (A.im == 0 && B.im == 0)
                    {
                        continue;
                    }

                    Contact m = new Contact(A, B);
                    m.Solve();
                    if (m.contact_count > 0)
                    {                                                
                        bool handle=A.DoCollision(B, m);

                        if (handle) {
                            contacts.Add(m);
                        }
                    }
                   
                }
                A.FinishCollisions();
            }

           

            // Integrate forces
            for (int i = 0; i < bodies.Count; ++i)
            {
                IntegrateForces(bodies[i], (float)deltaTtime);
            }

            // Initialize collision
            for (int i = 0; i < contacts.Count; ++i)
            {
                contacts[i].Initialize();
            }

            // Solve collisions
            for (int j = 0; j < m_iterations; ++j)
            {
                for (int i = 0; i < contacts.Count; ++i)
                {
                    contacts[i].ApplyImpulse();
                }
            }

            // Integrate velocities
            for (int i = 0; i < bodies.Count; ++i)
            {
                IntegrateVelocity(bodies[i], (float)deltaTtime);
            }

            // Correct positions
            for (int i = 0; i < contacts.Count; ++i)
            {
                contacts[i].PositionalCorrection();
            }

            // Clear all forces
            for (int i = 0; i < bodies.Count; ++i)
            {
                Body b = bodies[i];                
                b.Force=Vector2.Zero;
                b.torque = 0;
            }
        }
    }
}
