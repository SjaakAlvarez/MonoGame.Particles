using Microsoft.Xna.Framework;
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
        public enum EmitterState { INIT, STARTED, STOPPING, STOPPED}

        public List<IParticle> Particles { get; set; }
        public Texture2D Texture { get; set; }        
        public Vector2 Position { get; set; }
        public string Name { get; set; }        
        public float ParticlesPerSecond;
        public float LinearDamping { get; set; }        
        public bool IgnoreGravity { get; set; }
        public Origin Origin { get; set; } = new PointOrigin();
        public double TotalSeconds { get; set; }

        protected Interval speed;
        protected List<Modifier> Modifiers { get; set; }
        protected List<BirthModifier> BirthModifiers { get; set; }
        protected Interval maxAge;
        protected EmitterState state = EmitterState.INIT;
        protected double releaseTime = 0;
        protected Interval direction;
        protected Interval rotation = new Interval(-Math.PI, Math.PI);
        protected Interval av = new Interval(-0.1f, 0.1f);

        private double _stopTime;
        private float _stopCount;

        public virtual void AddModifier(Modifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public virtual void AddBirthModifier(BirthModifier modifier)
        {
            BirthModifiers.Add(modifier);
        }

        public void Start()
        {
            releaseTime = 0;
            state = EmitterState.STARTED;
        }

        public void Stop()
        {
            if (state == EmitterState.STARTED)
            {
                state = EmitterState.STOPPING;
                _stopCount = ParticlesPerSecond;
            }
        }

        public bool CanDestroy()
        {
            return Particles.Count == 0 && state==EmitterState.STOPPED;
        }

        public virtual void Update(double seconds)
        {
            if (state == EmitterState.STOPPING)
            {
                _stopTime += seconds;
                ParticlesPerSecond = MathHelper.SmoothStep(_stopCount, 0, (float)_stopTime);
                if (ParticlesPerSecond <= 0)
                {
                    state = EmitterState.STOPPED;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (IParticle p in Particles)
            {
                spriteBatch.Draw(p.Texture, new Vector2(p.Position.X, p.Position.Y), new Rectangle(0, 0, Texture.Width, Texture.Height), p.Color * p.Alpha, p.Orientation, new Vector2(Texture.Width, Texture.Height) / 2, 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
