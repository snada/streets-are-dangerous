using System;
using System.Xml.Serialization;

namespace StreetsAreDangerous.Scores
{
    public class Score : IComparable
    {
        /// <summary>
        /// Il record in formato stringa, per mera comodità di visualizzazione.
        /// </summary>
        public string RecordMatchString { get; set; }

        /// <summary>
        /// Data e ora del conseguimento del record in formato stringa, di nuovo per mera comodità di visualizzazione.
        /// </summary>
        public string ScoreDateTime { get; set; }

        /// <summary>
        /// Le ore del match record.
        /// </summary>
        public int MatchHours { get; set; }

        /// <summary>
        /// I minuti del match record.
        /// </summary>
        public int MatchMinutes { get; set; }

        /// <summary>
        /// I secondi del match record.
        /// </summary>
        public int MatchSeconds { get; set; }

        /// <summary>
        /// I millisecondi del match record.
        /// </summary>
        public int MatchMilliseconds { get; set; }

        /// <summary>
        /// Il nome del giocatore che ha conseguito il record.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Costruisce un punteggio da salvare.
        /// </summary>
        /// <param name="timeSpan">Tempo di gioco record da salvare.</param>
        /// <param name="name">Nick del giocatore che ha effettuato il record.</param>
        public Score(TimeSpan timeSpan, string name)
        {
            this.MatchHours = (int)timeSpan.TotalHours;
            this.MatchMinutes = timeSpan.Minutes;
            this.MatchSeconds = timeSpan.Seconds;
            this.MatchMilliseconds = timeSpan.Milliseconds;
            this.RecordMatchString = this.MatchHours + ":" + this.MatchMinutes + ":" + this.MatchSeconds;
            this.Name = name;
            this.ScoreDateTime = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        }

        public Score()
        {
            this.MatchHours = 0;
            this.MatchMinutes = 0;
            this.MatchSeconds = 0;
            this.MatchMilliseconds = 0;
            this.RecordMatchString = this.MatchHours + ":" + this.MatchMinutes + ":" + this.MatchSeconds;
            this.Name = "NULL";
            this.ScoreDateTime = "NULL";
        }

        /// <summary>
        /// Ritorna un valore numerico indicante la differenza tra due oggetti punteggio.
        /// </summary>
        /// <param name="obj">Oggetto da confrontare.</param>
        /// <returns>Valore intero indicante la differenza tra i due oggetti.</returns>
        public int CompareTo(object obj)
        {
            try
            {
                Score tmp = (Score)obj;
                return -new TimeSpan(0, MatchHours, MatchMinutes, MatchSeconds, MatchMilliseconds).CompareTo(new TimeSpan(0, tmp.MatchHours, tmp.MatchMinutes, tmp.MatchSeconds, tmp.MatchMilliseconds));
            }
            catch (InvalidCastException e)
            {
                e.ToString();
                return int.MinValue;
            }
        }

        public override string ToString()
        {
            return this.Name + " " + RecordMatchString + " " + ScoreDateTime;
        }
    }
}
