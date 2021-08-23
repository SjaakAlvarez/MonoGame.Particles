using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Particles.Particles.Origins
{

    public enum TextureAnimatedDirections { LEFT, RIGHT, UP, DOWN }

    public class TextureAnimatedOrigin : Origin
    {
        private static readonly Random random = new Random();
        private readonly double _speed;
        private readonly Texture2D _texture;
        private readonly TextureAnimatedDirections _direction;
        private Color[] _data;

        public override bool UseColorData => true;

        public TextureAnimatedOrigin(Texture2D texture, TextureAnimatedDirections direction, double speed)
        {
            _speed = speed;
            _texture = texture;
            _direction = direction;

            _data = new Color[_texture.Width * _texture.Height];
            _texture.GetData<Color>(_data);
        }

        private Color getPixel(int x, int y)
        {
            return _data[y * _texture.Width + x];
        }

        public override OriginData GetPosition(Emitter e)
        {
            List<int> mask;
            int value;
            float pos;

            switch (_direction)
            {
                case TextureAnimatedDirections.RIGHT:
                    pos = MathHelper.Lerp(0, _texture.Width - 1, (float)(e.TotalSeconds / _speed));
                    if (pos >= _texture.Width - 1)
                    {
                        e.Stop();
                        return null;
                    }
                    mask = GetVerticalMask((int)pos);
                    if (mask.Count == 0) return null;
                    value = random.Next(mask.Count);
                    return new OriginData(new Vector2(pos, mask[value]), getPixel((int)pos, mask[value]));

                case TextureAnimatedDirections.LEFT:
                    pos = MathHelper.Lerp(_texture.Width - 1, 0, (float)(e.TotalSeconds / _speed));
                    if (pos <= 0)
                    {
                        e.Stop();
                        return null;
                    }
                    mask = GetVerticalMask((int)pos);
                    if (mask.Count == 0) return null;
                    value = random.Next(mask.Count);
                    return new OriginData(new Vector2(pos, mask[value]), getPixel((int)pos, mask[value]));

                case TextureAnimatedDirections.DOWN:
                    pos = MathHelper.Lerp(0, _texture.Height - 1, (float)(e.TotalSeconds / _speed));
                    if (pos >= _texture.Height - 1)
                    {
                        e.Stop();
                        return null;
                    }
                    mask = GetHorizontalMask((int)pos);
                    if (mask.Count == 0) return null;
                    value = random.Next(mask.Count);
                    return new OriginData(new Vector2(mask[value], pos), getPixel(mask[value], (int)pos));

                case TextureAnimatedDirections.UP:
                    pos = MathHelper.Lerp(_texture.Height - 1, 0, (float)(e.TotalSeconds / _speed));
                    if (pos <= 0)
                    {
                        e.Stop();
                        return null;
                    }
                    mask = GetHorizontalMask((int)pos);
                    if (mask.Count == 0) return null;
                    value = random.Next(mask.Count);
                    return new OriginData(new Vector2(mask[value], pos), getPixel(mask[value], (int)pos));
            }


            return null;
        }

        private List<int> GetVerticalMask(int pos)
        {
            List<int> possible = new List<int>();
            for (int n = 0; n < _texture.Height; n++)
            {
                if (getPixel(pos, n) != Color.Transparent)
                {
                    possible.Add(n);
                }
            }
            return possible;
        }

        private List<int> GetHorizontalMask(int pos)
        {
            List<int> possible = new List<int>();
            for (int n = 0; n < _texture.Width; n++)
            {
                if (getPixel(n, pos) != Color.Transparent)
                {
                    possible.Add(n);
                }
            }
            return possible;
        }
    }
}
