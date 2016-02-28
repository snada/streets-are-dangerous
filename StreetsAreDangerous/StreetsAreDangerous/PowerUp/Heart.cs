using Microsoft.Xna.Framework;
using System;

namespace StreetsAreDangerous.PowerUps
{
    public class Heart : PowerUp
    {
        public Heart()
            : base()
        {
            this.Model = 6;
            this.angle = 0;
        }

        public override void Update(float zPosition)
        {
            base.Update(zPosition);
            this.Rotation = new Vector3(0, this.Rotation.Y + 0.1f, 0);
            this.Position = new Vector3(this.Position.X, this.Position.Y + (float)Math.Sin(angle) / 12, this.Position.Z);
            this.boundingBox = new BoundingBox(new Vector3(Position.X - 0.7f, 0, zPosition - Position.Z - 0.7f), new Vector3(Position.X + 0.7f, 2.5f, zPosition - Position.Z + 0.7f));

            angle += 0.1f;
        }
    }
}
