using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class RectangleOrigin : Origin
    {
        private readonly Interval x;
        private readonly Interval y;

        public RectangleOrigin(int width, int height)
        {
            x = new Interval(-width / 2, width / 2);
            y = new Interval(-height / 2, height / 2);
        }

        public override Vector2 GetPosition()
        {
            return new Vector2((int)x.GetValue(), (int)y.GetValue());
        }
    }
}
