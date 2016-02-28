using Microsoft.Xna.Framework;

namespace StreetsAreDangerous.Utilities
{
    public class QuadraticBezier3D
    {
        /// <summary>
        /// Punto di inizio della curva.
        /// </summary>
        public Vector3 StartPoint { get; set; }

        /// <summary>
        /// Punto di controllo della curva.
        /// </summary>
        public Vector3 ControlPoint { get; set; }

        /// <summary>
        /// Punto di fine della curva.
        /// </summary>
        public Vector3 EndPoint { get; set; }

        /// <summary>
        /// Crea una nuova istanza di Curva di Bezier.
        /// </summary>
        public QuadraticBezier3D()
        {
            StartPoint = Vector3.Zero;
            ControlPoint = Vector3.Zero;
            EndPoint = Vector3.Zero;
        }

        /// <summary>
        /// Crea una nuova istanza di Curva di Bezier.
        /// </summary>
        /// <param name="startPoint">Punto di inizio della curva.</param>
        /// <param name="controlPoint">Punto di controllo della curva.</param>
        /// <param name="endPoint">Punto di fine della curva.</param>
        public QuadraticBezier3D(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            this.StartPoint = startPoint;
            this.ControlPoint = controlPoint;
            this.EndPoint = endPoint;
        }

        /// <summary>
        /// Trova un punto della curva per un valore i specificato, compreso tra 0 e 1. Per 0 sarà restituito StartPoint, per 1 sarà restituito EndPoint.
        /// </summary>
        /// <param name="i">Valore del punto nella curva.</param>
        /// <returns></returns>
        public Vector3 Evaluate(float i)
        {
            return Vector3.Lerp(Vector3.Lerp(StartPoint, ControlPoint, i), Vector3.Lerp(ControlPoint, EndPoint, i) , i);
        }
    }
}