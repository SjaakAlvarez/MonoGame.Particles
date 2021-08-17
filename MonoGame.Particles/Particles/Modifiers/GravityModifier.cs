using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class GravityModifier : Modifier
    {
        private readonly Vector2 _gravity;

        public override bool SupportsPhysics { get => false; }

        public GravityModifier(Vector2 g)
        {
            _gravity = g;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Velocity += _gravity * (float)seconds;
        }
    }
}
