using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class OriginData
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public OriginData(Vector2 p, Color c)
        {
            Position = p;
            Color = c;
        }

        public OriginData(Vector2 p)
        {
            Position = p;
            Color = Color.Transparent;
        }
    }
}
