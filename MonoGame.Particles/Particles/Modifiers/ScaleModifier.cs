using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class ScaleModifier : Modifier
    {
        private readonly float _start;
        private readonly float _end;

        public override bool SupportsPhysics { get => false; }

        public ScaleModifier(float a, float b)
        {
            _start = a;
            _end = b;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {      
            if(p is Particle particle)
            {
                particle.Scale = MathHelper.Lerp(_start, _end,(float)(p.Age / p.MaxAge));
            }
            
        }
    }
}
