using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class Particle : Body
    {
        public double Age = 0;
        public double MaxAge = 0;
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;

        public Particle(Shape shape, Vector2 pos):base(shape, pos)
        {
            IsParticle = true;            
        }
    }
}
