using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class ColorInterval
    {
        private readonly Color[] _colors;

        public ColorInterval(params Color[] colors)
        {
            _colors = colors;
        }

        public Color GetValue(double amount)
        {
            int count = _colors.Length - 1;

            double pos = amount * count;
            pos = Math.Abs(Math.Min(pos, count - 0.001d));

            int startColor = (int)Math.Floor(pos);
            int endColor = startColor + 1;

            return Color.Lerp(_colors[startColor], _colors[endColor % _colors.Length], (float)pos - startColor);
        }

    }
}
