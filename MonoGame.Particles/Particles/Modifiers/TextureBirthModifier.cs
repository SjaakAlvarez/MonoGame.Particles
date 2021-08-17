using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class TextureBirthModifier : BirthModifier
    {
        private Texture2D[] _textures;
        private static Random random = new Random();

        public override bool SupportsPhysics { get => true; }

        public TextureBirthModifier(params Texture2D[] textures)
        {
            _textures = textures;
        }

        public override void Execute(Emitter e, IParticle p)
        {
            p.Texture = _textures[random.Next(_textures.Length)];
        }
    }
}
