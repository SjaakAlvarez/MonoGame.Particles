using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class Emitter : BaseEmitter
    {
        public event ParticleDeathEventHandler ParticleDeath;
        public delegate void ParticleDeathEventHandler(object sender, ParticleEventArgs e);

        protected virtual void OnParticleDeath(ParticleEventArgs e)
        {
            ParticleDeathEventHandler handler = ParticleDeath;
            handler?.Invoke(this, e);
        }

        public event ParticleBirthEventHandler ParticleBirth;
        public delegate void ParticleBirthEventHandler(object sender, ParticleEventArgs e);

        protected virtual void OnParticleBirth(ParticleEventArgs e)
        {
            ParticleBirthEventHandler handler = ParticleBirth;
            handler?.Invoke(this, e);
        }

        public Emitter(String name, Vector2 position, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge)
        {
            this.Name = name;
            this.Position = position;            
            this.speed = speed;
            this.maxAge = maxAge;
            this.direction = direction;
            this.ParticlesPerSecond = particlesPerSecond;            
            Modifiers = new List<Modifier>(5);
            particles = new List<IParticle>(100);            
        }        

        public override void Update(double seconds)
        {
            if (started)
            {
                releaseTime += seconds;

                double release = ParticlesPerSecond * releaseTime;                
                if (release > 1)
                {
                    int r = (int)Math.Floor(release);
                    releaseTime -= (r / ParticlesPerSecond);

                    for (int i = 0; i < r; i++)
                    {
                        AddParticle();
                    }
                }
            }

            double milliseconds = seconds * 1000;

            foreach (IParticle p in particles.ToArray())
            {
                p.Age += milliseconds;

                if (p.Age > p.MaxAge)
                {
                    OnParticleDeath(new ParticleEventArgs(p));
                }
                
                float dampening = VectorMath.Clamp(1.0f - (float)seconds * LinearDamping, 0.0f, 1.0f);

                p.Position += (p.Velocity * (float)seconds);
                p.Velocity+= (VectorMath.gravity * (float)seconds);
                p.Velocity *= dampening;

                foreach (Modifier m in Modifiers)
                {
                    m.Execute(this, seconds, p);
                }

               
            }            

            particles.RemoveAll(p => p.Age > p.MaxAge);               
            
        }

        public Particle AddParticle()
        {
            Particle particle = new Particle();

            Matrix matrix = Matrix.CreateRotationZ((float)direction.GetValue());

            particle.Velocity = new Vector2((float)speed.GetValue(), 0);
            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
            particle.Position = Position;
            particle.Orientation=(float)rotation.GetValue();
            particle.AngularVelocity = (float)av.GetValue();
           
            //particle.LinearDamping = LinearDamping;
            particle.MaxAge = maxAge.GetValue();
            //particle.IgnoreGravity = IgnoreGravity;
                        
            particles.Add(particle);

            OnParticleBirth(new ParticleEventArgs(particle));

            return particle;
        }
        
       
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (IParticle p in particles)
            {
                spriteBatch.Draw(Texture, new Vector2(p.Position.X, p.Position.Y), new Rectangle(0, 0, Texture.Width, Texture.Height), p.Color * p.Alpha, p.Orientation, new Vector2(Texture.Width, Texture.Height) / 2, 1.0f, SpriteEffects.None, 0);                
            }
        }

    }

}

