using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class PhysicsParticle : Body,IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
      

        public PhysicsParticle(Shape shape, Vector2 pos):base(shape, pos)
        {
            IsParticle = true;            
        }

       
    }
}
