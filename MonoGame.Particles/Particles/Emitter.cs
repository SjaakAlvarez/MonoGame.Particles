﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Particles.Origins;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class Emitter
    {
        public List<IParticle> particles;
        public Texture2D Texture { get; set; }
        protected bool started = false;
        protected double releaseTime = 0;        
        public Vector2 Position { get; set; }
        public string Name { get; set; }
        protected Interval maxAge;
        public float ParticlesPerSecond;
        public float LinearDamping { get; set; }
        protected Interval speed;
        public List<Modifier> Modifiers { get; set; }
        public bool IgnoreGravity { get; set; }
        public Origin Origin { get; set; } = new PointOrigin();
        
        protected Interval direction;
        protected Interval rotation = new Interval(-Math.PI, Math.PI);
        protected Interval av = new Interval(-0.1f, 0.1f);

        public void Start()
        {
            releaseTime = 0;
            started = true;
        }

        public void Stop()
        {
            started = false;
        }

        public bool CanDestroy()
        {
            return particles.Count == 0 && !started;
        }

        public virtual void Update(double seconds) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (IParticle p in particles)
            {
                spriteBatch.Draw(Texture, new Vector2(p.Position.X, p.Position.Y), new Rectangle(0, 0, Texture.Width, Texture.Height), p.Color * p.Alpha, p.Orientation, new Vector2(Texture.Width, Texture.Height) / 2, 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
