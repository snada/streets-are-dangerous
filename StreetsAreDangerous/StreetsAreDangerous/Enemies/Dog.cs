using Microsoft.Xna.Framework;
using StreetsAreDangerous.Utilities;

namespace StreetsAreDangerous.Enemies
{
    public class Dog : Enemy
    {
        /// <summary>
        /// Curva per il movimento del cane
        /// </summary>
        public QuadraticBezier3D curve;

        /// <summary>
        /// Iteratore che indica a che punto ci si trova della curva.
        /// </summary>
        public float curveIterator;

        /// <summary>
        /// Velocità di percorrimento della curva. Random.
        /// </summary>
        public float curveStep;

        /// <summary>
        /// Contatore per la generazione del suono.
        /// </summary>
        public int soundIterator;

        /// <summary>
        /// Valore da raggiungere per effettuare il suono.
        /// </summary>
        public int soundTarget;

        /// <summary>
        /// Incremento di soundIterator ad ogni step.
        /// </summary>
        public int soundStep;

        /// <summary>
        /// Crea una nuova istanza di Dog. Posizione random.
        /// </summary>
        public Dog() 
            : this(new Vector3((float)UtilityClass.nextInt(-850, 850) / 100, 0, ((float)UtilityClass.nextInt(0, 500)) / 10.0f))
        {
        }

        /// <summary>
        /// Crea una nuova istanza di Dog.
        /// </summary>
        /// <param name="Position">Posizione in cui deve comparire il Dog.</param>
        public Dog(Vector3 Position) 
            : base(Position)
        {
            this.Model = 3;

            curve = new QuadraticBezier3D();
            regenerateCurve();
            
            curveIterator = 0;
            curveStep = (float)UtilityClass.nextInt(2, 4) / 100;

            soundSetup();
        }

        /// <summary>
        /// Aggiornamento del cane.
        /// </summary>
        /// <param name="speed">Velocità di percorrimento.</param>
        /// <param name="zPosition">Posizione sull'asse Z relativa alla Section su cui si trova il Dog.</param>
        public override void Update(float speed, float zPosition)
        {
            soundIterator += soundStep;

            if (curveIterator < 1)
            {
                this.Position = curve.Evaluate(curveIterator);
                curveIterator += curveStep;
            }
            else
            {
                regenerateCurve();
                curveIterator = 0;
            }
            boundingBox = new BoundingBox(new Vector3(Position.X - 1.7f, Position.Y, zPosition - Position.Z - 0.1f), new Vector3(Position.X + 1.7f, Position.Y + 2.4f, zPosition - Position.Z + 0.1f));
        }

        public override bool PlaySound()
        {
            if (soundIterator >= soundTarget)
            {
                soundSetup();
                return true;
            }
            else
                return false;
        }

        private void soundSetup()
        {
            soundIterator = 0;
            soundStep = 1;
            soundTarget = UtilityClass.nextInt(20, 100);
        }

        /// <summary>
        /// Finita la curva, se ne genera una nuova.
        /// </summary>
        private void regenerateCurve()
        {
            curve.StartPoint = Position;
            
            float zAdder = UtilityClass.nextInt(-5, 5);
            curve.EndPoint = new Vector3((float)UtilityClass.nextInt(-850, 850) / 100, 0, MathHelper.Clamp(curve.StartPoint.Z + zAdder, 0.0f, 50.0f));

            Vector3 puntoMedio = Vector3.Lerp(curve.StartPoint, curve.EndPoint, 0.5f);
            curve.ControlPoint = new Vector3(puntoMedio.X, (float)UtilityClass.nextInt(400, 700) / 100, puntoMedio.Z);
        }

    }
}
