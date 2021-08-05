using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Particles.Particles
{
    public class Emitter
    {
        public List<Particle> particles=new List<Particle>();
        public Vector2 position;
        private static Random rand = new Random();
        private World scene;
        private Interval maxAge;
        public float ParticlesPerSecond { get; set; }
        public float LinearDamping { get; set; }
        private double releaseTime=0;
        private Shape shape;
        private Interval speed;
        public bool started=false;
        public List<Modifier> Modifiers { get; set; }
        public Texture2D Texture { get; set; }
        public bool IgnoreGravity { get; set; }
        private bool canDestroy;

        public delegate bool OnCollisionEventHandler(Body sender, Body other, Contact m);

        internal OnCollisionEventHandler onCollisionEventHandler;
        public event OnCollisionEventHandler OnCollision
        {
            add { onCollisionEventHandler += value; }
            remove { onCollisionEventHandler -= value; }
        }

        public Emitter(World scene, Shape shape, Vector2 position, Interval speed, float particlesPerSecond, Interval maxAge)
        {
            this.position = position;
            this.scene = scene;
            this.speed = speed;
            this.maxAge = maxAge;
            this.ParticlesPerSecond = particlesPerSecond;
            this.shape = shape;
            Modifiers = new List<Modifier>();
        }

        public void Start()
        {
            releaseTime = 0;
            started = true;
        }

        public void Stop()
        {
            started = false;
            canDestroy = true;
        }

        public bool CanDestroy()
        {
            return canDestroy && particles.Count == 0;
        }

        public void Update(double seconds)
        {
            if (started)
            {
                releaseTime += seconds;

                double release = ParticlesPerSecond * releaseTime;
                if (release > 1)
                {
                    int r = (int)Math.Floor(release);
                    releaseTime -= (r / ParticlesPerSecond);

                    Interval rotation = new Interval(-Math.PI, Math.PI);
                    Interval av = new Interval(-0.1f,0.1f);

                    for (int i = 0; i < r; i++)
                    {
                        Particle particle = new Particle((Shape)shape.Clone(), position);

                        Matrix matrix = Matrix.CreateRotationZ((float)(rand.NextDouble() * Math.PI * 2));

                        particle.velocity = new Vector2((float)speed.GetValue(), 0);
                        particle.velocity = Vector2.Transform(particle.velocity, matrix);                        
                        particle.SetOrientation((float)rotation.GetValue());
                        particle.angularVelocity = (float)av.GetValue();
                        particle.LinearDamping = LinearDamping;
                        particle.MaxAge = maxAge.GetValue();
                        particle.IgnoreGravity = IgnoreGravity;
                        particle.OnCollision += Particle_OnCollision;
                        scene.AddBody(particle);
                        particles.Add(particle);
                    }
                }
            }

            foreach(Particle p in particles)
            {
                p.Age += seconds*1000;
                foreach(Modifier m in Modifiers)
                {
                    m.Execute(p);
                }
            }
            
            List<Particle> remove = particles.FindAll(p => p.Age > p.MaxAge);
            particles.RemoveAll(p => p.Age > p.MaxAge);
            foreach (Particle b in remove) scene.RemoveBody(b);
        }

        private bool Particle_OnCollision(Body sender, Body other, Contact m)
        {
            
            bool keep = true;
            if (onCollisionEventHandler != null)
            {
                keep = onCollisionEventHandler(sender, other, m);
            }
            if (!keep)
            {
                ((Particle)sender).Age = ((Particle)sender).MaxAge;
            }
            //Let the other object determine if it wants to collide
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (Particle p in particles)
            {
                //new Rectangle(new Point((int)b.position.X, (int)b.position.Y), new Point(40, 40)), new Rectangle(0, 0, 64, 64), b.Color* b.Alpha, b.orientation, new Vector2(32, 32), SpriteEffects.None, 0);
                spriteBatch.Draw(Texture,new Vector2(p.position.X, p.position.Y), new Rectangle(0,0,Texture.Width,Texture.Height), p.Color * p.Alpha, p.orientation, new Vector2(Texture.Width,Texture.Height)/2,1.0f, SpriteEffects.None, 0); 
            }
        }
    }
}
