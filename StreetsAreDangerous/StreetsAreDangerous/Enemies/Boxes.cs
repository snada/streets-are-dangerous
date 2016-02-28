using StreetsAreDangerous.Enemies;
using StreetsAreDangerous.Utilities;
using Microsoft.Xna.Framework;

namespace StreetsAreDangerous.Enemies
{
    public class Boxes : Enemy
    {
        public Boxes()
            : this(new Vector3((float)UtilityClass.nextInt(-850 + 25, 850 - 25) / 100, 0, ((float)UtilityClass.nextInt(2, 498)) / 10.0f))
        {

        }

        public Boxes(Vector3 Position)
            :base(Position)
        {
            this.Model = 7;
        }

        public override void Update(float speed, float zPosition)
        {
            this.boundingBox = new BoundingBox(new Vector3(Position.X - 2.5f, Position.Y, zPosition - Position.Z - 1.5f), new Vector3(Position.X + 2.5f, 4, zPosition - Position.Z + 1 + 1.5f));
            base.Update(speed, zPosition);
        }
    }
}
