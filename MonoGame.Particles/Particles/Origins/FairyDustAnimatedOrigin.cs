using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class FairyDustAnimatedOrigin : Origin
    {
        private readonly Rectangle _screen;
        private readonly List<Vector2> _positions = new List<Vector2>();
        private static Random rand = new Random();
        private double _lastMove;
        private double _moveDuration;
        private readonly float _minDist;
        private readonly double _speed;

        public override bool UseColorData => false;

        public FairyDustAnimatedOrigin(Rectangle screen, float minDist=250.0f, double speed=1.0d)
        {
            _screen = screen;
            _minDist = minDist;
            _speed = speed;
            Vector2 start = Vector2.Zero;
            _positions.Add(start);
            _positions.Add(start);
            _positions.Add(getNextPoint(start));
            _positions.Add(getNextPoint(_positions[2]));
        }

        public Vector2 getNextPoint(Vector2 point)
        {
            Vector2 n = Vector2.Zero;
            float dist = 0;
            while (dist < _minDist)
            {
                n = new Vector2(rand.Next(_screen.Left, _screen.Right), rand.Next(_screen.Top,_screen.Bottom));
                dist = (n - point).Length();
            }
            return n;
        }

        public override OriginData GetPosition(Emitter e)
        {
            if (e.TotalSeconds > _lastMove+_moveDuration)
            {
                _positions.RemoveAt(0);
                _positions.Add(getNextPoint(_positions[2]));
                _moveDuration = (_positions[2] - _positions[1]).Length() /_speed /1000.0f;
                _lastMove=e.TotalSeconds;
            }

            return new OriginData(Vector2.CatmullRom(_positions[0], _positions[1], _positions[2], _positions[3], (float)((e.TotalSeconds-_lastMove) / _moveDuration)));
            
        }
    }
}
