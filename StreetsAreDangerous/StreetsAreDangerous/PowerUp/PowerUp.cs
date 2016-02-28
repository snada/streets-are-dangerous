using Microsoft.Xna.Framework;
using StreetsAreDangerous.Utilities;

namespace StreetsAreDangerous.PowerUps
{
    public class PowerUp
    {
        public bool isAvailable { get; set; }

        /// <summary>
        /// Posizione del PowerUp relativa alla Section.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotazione del PowerUp relativa ai propri assi.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// BoundingBox per le collisioni. Va aggiornata nella chiamata di Update di ogni nemico.
        /// </summary>
        public BoundingBox boundingBox { get; set; }

        /// <summary>
        /// Indice del modello nell'array dei modelli 3D. Ogni nuova tipologia di nemico che estende questa classe deve modificare il valore.
        /// </summary>
        public int Model { get; set; }
        
        /// <summary>
        /// Variabile di supporto per la rotazione del modello.
        /// </summary>
        public float angle;

        public PowerUp()
            :this(new Vector3((float)UtilityClass.nextInt(-850, 850) / 100, 0, (float)UtilityClass.nextInt(0, 500) / 100))
        {

        }

        public PowerUp(Vector3 position)
        {
            isAvailable = true;
            this.Position = position;
        }

        public virtual void Update(float zPosition)
        {

        }
    }
}
