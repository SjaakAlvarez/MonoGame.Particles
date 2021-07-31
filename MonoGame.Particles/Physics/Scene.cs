using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class Scene
    {
        private float m_dt;
        private readonly int m_iterations;
        private List<Body> bodies=new List<Body>();
        private List<Manifold> contacts = new List<Manifold>();

        private SpatialHash hash;

        public Scene(float dt, int iterations, int cellSize)
        {
            this.m_dt = dt;
            this.m_iterations = iterations;
            hash = new SpatialHash(1920, 1080, cellSize);
        }

        private void IntegrateForces(Body b, float dt)
        {
            if (b.im == 0.0f)
            {
                return;
            }

            b.velocity += (b.Force * b.im + VectorMath.gravity) * (dt / 2.0f);
            b.angularVelocity += b.torque * b.iI * (dt / 2.0f);
        }

        private void IntegrateVelocity(Body b, float dt)
        {
            if (b.im == 0.0f)
            {
                return;
            }

            b.position += b.velocity * dt;
            b.orient += b.angularVelocity * dt;
            b.SetOrientation(b.orient);
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
            /*for (int i = 0; i < bodies.Count; ++i)
            {
                Body A = bodies[i];

                for (int j = i + 1; j < bodies.Count; ++j)
                {
                    Body B = bodies[j];
                    if (A.im == 0 && B.im == 0)
                        continue;
                    Manifold m=new Manifold(A, B);
                    m.Solve();
                    if (m.contact_count>0)
                        contacts.Add(m);
                }
            }*/
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

                    Manifold m = new Manifold(A, B);
                    m.Solve();
                    if (m.contact_count > 0)
                    {
                        contacts.Add(m);
                    }
                }
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
