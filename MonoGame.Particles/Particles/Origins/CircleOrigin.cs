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
        private readonly bool _edge;
        private readonly int _radius;

        public CircleOrigin(int radius, bool edge = false)
        {
            dist = new Interval(0, radius);
            _edge = edge;
            _radius = radius;
        }

        public override bool UseColorData => false;

        public override OriginData GetPosition(Emitter e)
        {
            Matrix rotation = Matrix.CreateRotationZ((float)angle.GetValue());
            if (_edge)
            {
                Vector2 p = new Vector2(_radius, 0);
                return new OriginData(Vector2.Transform(p, rotation));
            }
            else
            {
                Vector2 p = new Vector2((int)dist.GetValue(), 0);
                return new OriginData(Vector2.Transform(p, rotation));
            }
        }
    }
}
