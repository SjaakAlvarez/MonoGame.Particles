using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public abstract class Origin
    {
        public abstract OriginData GetPosition(Emitter e);

        public abstract bool UseColorData { get; }
    }
}
