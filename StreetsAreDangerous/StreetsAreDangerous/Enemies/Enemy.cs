using Microsoft.Xna.Framework;

namespace StreetsAreDangerous.Enemies
{
    public class Enemy
    {
        /// <summary>
        /// Restituisce la posizione nel nemico relativa alla section.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Restituisce o modifica l'informazione sullo stato di vita del cane.
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// BoundingBox per le collisioni. Va aggiornata nella chiamata di Update di ogni nemico.
        /// </summary>
        public BoundingBox boundingBox { get; set; }

        /// <summary>
        /// Indice del modello nell'array dei modelli 3D. Ogni nuova tipologia di nemico che estende questa classe deve modificare il valore.
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        /// Costruttore di un nemico generico.
        /// </summary>
        /// <param name="Position">Posizione nello spazio, relativa alla section.</param>
        public Enemy(Vector3 Position)
        {
            this.Position = Position;
            this.IsAlive = true;
        }

        /// <summary>
        /// Costruttore generico per serializzazione.
        /// </summary>
        public Enemy()
        {

        }

        /// <summary>
        /// Ogni nemico deve effettuare l'override di questo metodo per l'aggioramento della propria logica (compresa Bounding Box).
        /// <param name="speed">Velocità del player al momento della chiamata del metodo.</param>
        /// <param name="zPosition">Posizione sull'asse Z della sezione che contiene questo enemy.</param>
        /// </summary>
        public virtual void Update(float speed, float zPosition)
        {
            
        }

        /// <summary>
        /// Metodo virtuale per la segnalazione di play di un suono.
        /// Ogni nemico, se è previsto che emetta suoni, dovrebbe effettuare l'override di questo metodo.
        /// </summary>
        /// <returns></returns>
        public virtual bool PlaySound()
        {
            return false;
        }
    }
}
