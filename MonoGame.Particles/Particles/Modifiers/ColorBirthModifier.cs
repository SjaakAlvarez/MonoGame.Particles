using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class ColorBirthModifier : BirthModifier
    {
        private ColorInterval _colorInterval;

        public override bool SupportsPhysics { get => true; }

        public ColorBirthModifier(params Color[] colors)
        {
            _colorInterval = new ColorInterval(colors);
        }

        public override void Execute(Emitter e, IParticle p)
        {
            p.Color = _colorInterval.GetValue(e.TotalSeconds - Math.Truncate(e.TotalSeconds));
        }
    }
}
