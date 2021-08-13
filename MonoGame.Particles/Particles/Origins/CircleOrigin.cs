using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class CircleOrigin : Origin
    {
        private readonly Interval dist;
        private readonly Interval angle = new Interval(-Math.PI, Math.PI);

        public CircleOrigin(int radius)
        {
            dist = new Interval(0,radius);           
        }

        public override Vector2 GetPosition()
        {
            Vector2 p = new Vector2((int)dist.GetValue(), 0);
            Matrix rotation = Matrix.CreateRotationZ((float)angle.GetValue());
            return Vector2.Transform(p,rotation);
        }
    }
}
