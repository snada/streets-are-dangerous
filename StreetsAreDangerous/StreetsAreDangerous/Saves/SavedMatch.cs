using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace StreetsAreDangerous.Saves
{
    /// <summary>
    /// Rappresenta tutte le variabili che compongono la logica di gioco, pronte per essere salvate.
    /// </summary>
    public class SavedMatch
    {
        //FATTO
        public Vector3 cameraPos { get; set; }
        public Vector3 cameraLook { get; set; }
        public Vector2 cameraShake { get; set; }
        public int shakeMagnitude { get; set; }
        public BoundingBox playerBox { get; set; }
        public int life { get; set; }
        public float[] lifepositions { get; set; }

        public long timeticks { get; set; }

        public GamePageState gameState { get; set; }
        public GamePageState previousState { get; set; }
        public List<Section> sections;
        public float speedTime { get; set; }
        public float flashCounter { get; set; }
        public float fadeIndex { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public TimeSpan matchTime { get; set; }

        public SavedMatch()
        {
        }

        /// <summary>
        /// Da richiamare prima della serializzazione. Perchè quegli imbecilli della Microsoft non sono in grado
        /// di fare una classe per la rappresentazione temporale serializzabile.
        /// </summary>
        public void setTime()
        {
            timeticks = matchTime.Ticks;
        }

        /// <summary>
        /// Da richiamare dopo la deserializzazione. Perchè quegli imbecilli della Microsoft non sono in grado
        /// di fare una classe per la rappresentazione temporale serializzabile.
        /// </summary>
        public void restoreTime()
        {
            matchTime = new TimeSpan(timeticks);
        }
    }
}
