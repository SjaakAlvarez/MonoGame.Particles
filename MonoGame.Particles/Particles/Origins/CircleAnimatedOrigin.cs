using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class CircleAnimatedOrigin : Origin
    {                    
        private readonly int _radius;
        private readonly double _speed;
        private readonly int _cycles;

        public CircleAnimatedOrigin(int radius, double speed=1.0d, int cycles=0)
        {                      
             _radius = radius;
            _speed = speed;
            _cycles = cycles;
        }

        public override bool UseColorData { get => false; }

        public override OriginData GetPosition(Emitter e)
        {
            double angle = e.TotalSeconds;

            if (_cycles > 0 && angle*_speed > MathHelper.TwoPi * _cycles) e.Stop();

            Matrix rotation = Matrix.CreateRotationZ((float)(angle*_speed));
            
            Vector2 p = new Vector2(_radius, 0);
            return new OriginData(Vector2.Transform(p, rotation));
                      
        }
    }
}
