using Microsoft.Xna.Framework;

namespace MonoGame.Particles.Particles.Origins
{
    public class PointOrigin : Origin
    {
        public override bool UseColorData => false;

        public override OriginData GetPosition(Emitter e)
        {
            return new OriginData(Vector2.Zero);
        }
    }
}
