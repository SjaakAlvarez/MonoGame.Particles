using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MonoGame.Particles.Particles.Origins;
using Microsoft.Extensions.ObjectPool;
using MonoGame.Particles.Particles.Data;
using System.Threading.Tasks;

namespace MonoGame.Particles.Particles
{
    public class ParticleEmitter : Emitter
    {
        private readonly World world;

        public DefaultObjectPool<IParticle> ParticlePool { get; set; }

        public event ParticleDeathEventHandler ParticleDeath;
        public delegate void ParticleDeathEventHandler(object sender, ParticleEventArgs e);

        protected virtual void OnParticleDeath(ParticleEventArgs e)
        {
            ParticleDeathEventHandler handler = ParticleDeath;
            handler?.Invoke(this, e);
        }        

        public ParticleEmitter(String name, World world, Vector2 position, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge)
        {
            Name = name;
            Position = position;
            this.speed = speed;
            this.maxAge = maxAge;
            this.world = world;
            this.direction = direction;
            ParticlesPerSecond = particlesPerSecond;
            Modifiers = new List<Modifier>();
            BirthModifiers = new List<BirthModifier>();
            Particles = new List<IParticle>(100);
            world.emitters.Add(this);            

            ParticlePool = new DefaultObjectPool<IParticle>(new ParticlePooledObjectPolicy());
        }

        public new bool CanDestroy()
        {
            return Particles.Count == 0 && state == EmitterState.STOPPED;
        }

        public override void Update(double seconds)
        {
            base.Update(seconds);

            if (state==EmitterState.STARTED || state==EmitterState.STOPPING)
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

            TotalSeconds += seconds;
            double milliseconds = seconds * 1000;
            float dampening = VectorMath.Clamp(1.0f - (float)seconds * LinearDamping, 0.0f, 1.0f);

            
            //Parallel.ForEach(Particles.ToArray(), p =>//
            foreach (IParticle p in Particles)
            {
                p.Age += milliseconds;

                if (p.Age > p.MaxAge)
                {
                    OnParticleDeath(new ParticleEventArgs(p));
                    ParticlePool.Return(p);
                }
                else
                {
                    p.Position += (p.Velocity * (float)seconds);
                    p.Velocity += (VectorMath.gravity * (float)seconds);
                    p.Velocity *= dampening;
                    p.Orientation += p.AngularVelocity;

                    foreach (Modifier m in Modifiers)
                    {
                        m.Execute(this, seconds, p);
                    }
                }

            }
            //);
                                 
            Particles.RemoveAll(p => p.Age > p.MaxAge);        
            if (CanDestroy()) world.emitters.Remove(this);
        }

        public void AddParticle()
        {
            OriginData data = Origin.GetPosition(this);

            if (data != null)
            {
                IParticle particle = ParticlePool.Get();               

                Matrix matrix = Matrix.CreateRotationZ((float)direction.GetValue());

                particle.Velocity = new Vector2((float)speed.GetValue(), 0);
                particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
                particle.Position = Position + data.Position;
                if (Origin.UseColorData) particle.Color = data.Color;
                particle.AngularVelocity = (float)av.GetValue();
                particle.Orientation = (float)rotation.GetValue();
                particle.AngularVelocity = (float)av.GetValue();
                particle.MaxAge = maxAge.GetValue();
                particle.Age = 0;
                particle.Texture = Texture;

                foreach (BirthModifier m in BirthModifiers) m.Execute(this, particle);

                Particles.Add(particle);                            
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {       
            foreach (Particle p in Particles.OfType<Particle>())
            {
                spriteBatch.Draw(p.Texture, new Vector2(p.Position.X, p.Position.Y), new Rectangle(0, 0, p.Texture.Width, p.Texture.Height), p.Color * p.Alpha, p.Orientation, new Vector2(p.Texture.Width, p.Texture.Height) / 2, p.Scale, SpriteEffects.None, 0);
            }
        }
    }
}

