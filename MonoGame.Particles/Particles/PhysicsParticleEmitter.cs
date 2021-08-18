using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MonoGame.Particles.Particles
{
    public class PhysicsParticleEmitter :Emitter
    {                   
        private readonly World world;            
        private readonly Shape shape;
        
        public delegate ContactAction OnCollisionEventHandler(PhysicsParticle sender, Body other, Contact m);

        internal OnCollisionEventHandler onCollisionEventHandler;
        public event OnCollisionEventHandler OnCollision
        {
            add { onCollisionEventHandler += value; }
            remove { onCollisionEventHandler -= value; }
        }

        public PhysicsParticleEmitter(String name, World world, Shape shape, Vector2 position, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge)
        {
            this.Name = name;
            this.Position = position;
            this.world = world;
            this.speed = speed;
            this.maxAge = maxAge;
            this.direction = direction;
            this.ParticlesPerSecond = particlesPerSecond;
            this.shape = shape;
            Modifiers = new List<Modifier>();
            BirthModifiers = new List<BirthModifier>();
            Particles = new List<IParticle>(100);
            world.physicsEmitters.Add(this);
        }

        public override void AddBirthModifier(BirthModifier modifier)
        {
            if (!modifier.SupportsPhysics) throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
            base.AddBirthModifier(modifier);
        }

        public override void AddModifier(Modifier modifier)
        {
            if (!modifier.SupportsPhysics) throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
            base.AddModifier(modifier);
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
            TotalSeconds += seconds;

            foreach (IParticle p in Particles.ToArray())
            {
                p.Age += milliseconds;
                foreach (Modifier m in Modifiers)
                {
                    m.Execute(this, seconds, p);
                }
            }

            List<IParticle> remove = Particles.FindAll(p => p.Age > p.MaxAge);

            Parallel.Invoke(
                () =>
                {
                    Particles.RemoveAll(p => p.Age > p.MaxAge);
                },
                () =>
                {
                    foreach (PhysicsParticle p in remove.OfType<PhysicsParticle>()) world.RemoveBody(p.Body);
                });


            if (CanDestroy()) world.physicsEmitters.Remove(this);
        }

        public PhysicsParticle AddParticle()
        {
            PhysicsParticle particle = new PhysicsParticle((Shape)shape.Clone(), Position + Origin.GetPosition());

            Matrix matrix = Matrix.CreateRotationZ((float)direction.GetValue());

            particle.Velocity = new Vector2((float)speed.GetValue(), 0);
            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
            particle.Orientation=(float)rotation.GetValue();
            particle.AngularVelocity = (float)av.GetValue();
            particle.LinearDamping = LinearDamping;
            particle.MaxAge = maxAge.GetValue();
            particle.IgnoreGravity = IgnoreGravity;
            particle.OnCollision += Particle_OnCollision;
            particle.Texture = Texture;
            foreach (BirthModifier m in BirthModifiers) m.Execute(this, particle);
            world.AddBody(particle.Body);
            Particles.Add(particle);
            return particle;
        }

        private ContactAction Particle_OnCollision(PhysicsParticle sender, Body other, Contact m)
        {
            ContactAction action = ContactAction.COLLIDE;
            if (onCollisionEventHandler != null)
            {
                action = onCollisionEventHandler(sender, other, m);
            }
            if (action == ContactAction.DESTROY)
            {
                (sender).Age = (sender).MaxAge;
                return ContactAction.IGNORE;
            }
            //Let the other object determine if it wants to collide
            return action;
        }

        
    }
}
