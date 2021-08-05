using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class AlphaFadeModifier : Modifier
    {
        public override void Execute(Particle p)
        {
            p.Alpha = MathHelper.Lerp(1.0f, 0.0f, (float)(p.Age / p.MaxAge));
        }
    }
}
