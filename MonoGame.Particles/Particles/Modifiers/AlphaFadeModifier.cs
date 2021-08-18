using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class AlphaFadeModifier : Modifier
    {
        public override bool SupportsPhysics { get => true; }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Alpha = MathHelper.Lerp(1.0f, 0.0f, (float)(p.Age / p.MaxAge));
        }
    }
}
