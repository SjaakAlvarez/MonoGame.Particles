using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class ColorRangeModifier : Modifier
    {
        private Color start;
        private Color end;
        public override void Execute(Particle p)
        {
            p.Color = Color.Lerp(start, end, (float)(p.Age / p.MaxAge));
        }

        public ColorRangeModifier(Color c1, Color c2)
        {
            start = c1;
            end = c2;
        }
    }
}
