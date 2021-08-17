using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Modifiers
{
    public class CustomBirthModifier : BirthModifier
    {
        public override bool SupportsPhysics { get => true; }

        public override void Execute(Emitter e, IParticle p)
        {
            p.AngularVelocity = 0;            
            p.Orientation = (float)Math.Atan2(p.Velocity.Y , p.Velocity.X);
        }
    }
}
