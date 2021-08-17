using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class ColorRangeModifier : Modifier
    {        
        private ColorInterval _colorInterval;

        public override bool SupportsPhysics { get => true; }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {            
            p.Color = _colorInterval.GetValue(p.Age / p.MaxAge);
        }

        public ColorRangeModifier(params Color[] colors)
        {
            _colorInterval = new ColorInterval(colors);           
        }
    }
}
