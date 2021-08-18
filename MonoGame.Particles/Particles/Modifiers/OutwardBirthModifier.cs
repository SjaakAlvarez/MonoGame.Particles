using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class OutwardBirthModifier : BirthModifier
    {
        public override bool SupportsPhysics { get => false; }

        public override void Execute(Emitter e, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = p.Position - e.Position;
            temp.Normalize();
            p.Velocity = temp * v;
        }
    }
}
