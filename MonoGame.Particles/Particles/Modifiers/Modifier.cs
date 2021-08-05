using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public abstract class Modifier
    {
        public abstract void Execute(Particle p);
    }
}
