using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Particles.Particles
{
    public class Emitter
    {
        public List<Body> particles=new List<Body>();
        public Vector2 position;
        private static Random rand = new Random();
        private Scene scene;
        private TimeSpan timeSpan;
        private float particlesPerSecond;
        private double releaseTime;
        private Shape shape;

        public Emitter(Scene scene, Shape shape, Vector2 position, float particlesPerSecond, TimeSpan timeSpan)
        {
            this.position = position;
            this.scene = scene;
            this.timeSpan = timeSpan;
            this.particlesPerSecond = particlesPerSecond;
            this.shape = shape;
        }

        public void Trigger()
        {
            
        }

        public void Update(double seconds)
        {
            releaseTime += seconds;

            double release = particlesPerSecond * releaseTime;
            if (release > 1) {
                int r =(int)Math.Floor(release);
                releaseTime -= (r / particlesPerSecond);

                for (int i = 0; i < r; i++)
                {
                    //Circle circle2 = new Circle(2);
                    Body body2=null;
                    
                    if(shape is Circle c)
                    {
                        Circle s = new Circle(shape.radius);
                        body2 = new Body(s, position);
                    }
                    else if(shape is PolygonShape p)
                    {
                        PolygonShape s = new PolygonShape();
                        p.m_normals.CopyTo(s.m_normals,0);
                        p.m_vertices.CopyTo(s.m_vertices,0);
                        s.m_vertexCount=p.m_vertexCount;
                        body2 = new Body(s, position);
                    }
                    
                    body2.velocity = new Vector2((float)rand.NextDouble() * 8 - 4.0f, (float)rand.NextDouble() * 4);
                    body2.IsParticle = true;
                    body2.SetOrientation(0);
                    scene.AddBody(body2);
                    particles.Add(body2);
                }
            }

            foreach(Body p in particles)
            {
                p.Age += (float)seconds;
            }
            
            List<Body> remove = particles.FindAll(p => p.Age > timeSpan.TotalSeconds);
            particles.RemoveAll(p => p.Age > timeSpan.TotalSeconds);
            foreach (Body b in remove) scene.RemoveBody(b);
        }
    }
}
