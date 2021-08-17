using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class Particle : IParticle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Orientation { get; set; }
        public float AngularVelocity { get; set; }
        public bool IsParticle { get; set; }
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
        public float Scale { get; set; } = 1.0f;
        public Texture2D Texture { get; set; }

        public Particle()
        {
            IsParticle = true;
        }
    }
}
