using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Modifiers
{
    public class CustomModifier : Modifier
    {
        public override bool SupportsPhysics { get => false; }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Orientation = (float)Math.Atan2(p.Velocity.Y, p.Velocity.X);
        }
    }
}
