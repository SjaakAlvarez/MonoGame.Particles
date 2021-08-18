using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class InwardBirthModifier : BirthModifier
    {
        public override bool SupportsPhysics { get => false; }

        public override void Execute(Emitter e, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = e.Position - p.Position;
            temp.Normalize();
            p.Velocity = temp * v;
        }
    }
}
