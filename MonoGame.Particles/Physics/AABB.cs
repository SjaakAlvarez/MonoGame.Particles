using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    #pragma warning disable S101
    public class AABB
    {
        public Vector2 min;
        public Vector2 max;

        public AABB(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
