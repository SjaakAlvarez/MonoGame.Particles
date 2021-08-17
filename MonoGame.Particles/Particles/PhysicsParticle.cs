using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class PhysicsParticle : IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
        public Texture2D Texture { get; set; }

        public Body Body { get; set; }
        public bool IsParticle { get => Body.IsParticle; set => throw new NotImplementedException(); }
        public Vector2 Position { get => Body.Position; set => Body.Position = value; }
        public Vector2 Velocity { get => Body.Velocity; set => Body.Velocity=value; }
        public float Orientation { get => Body.Orientation; set => Body.SetOrientation(value); }
        public float AngularVelocity { get => Body.AngularVelocity; set => Body.AngularVelocity=value; }
        public float LinearDamping { get => Body.LinearDamping; set => Body.LinearDamping = value; }
        public bool IgnoreGravity { get => Body.IgnoreGravity; set => Body.IgnoreGravity = value; }

        public delegate ContactAction OnParticleCollisionEventHandler(PhysicsParticle sender, Body other, Contact m);

        internal OnParticleCollisionEventHandler onParticleCollisionEventHandler;
        public event OnParticleCollisionEventHandler OnCollision
        {
            add { onParticleCollisionEventHandler += value; }
            remove { onParticleCollisionEventHandler -= value; }
        }

        public PhysicsParticle(Shape shape, Vector2 pos)
        {
            Body = new Body(shape, pos);
            Body.IsParticle = true;
            Body.OnCollision += Body_OnCollision;
        }

        private ContactAction Body_OnCollision(Body sender, Body other, Contact m)
        {
            ContactAction action = ContactAction.COLLIDE;
            if (onParticleCollisionEventHandler != null)
            {
                action=onParticleCollisionEventHandler(this, other, m);
            }
            return action;
        }
    }
}
