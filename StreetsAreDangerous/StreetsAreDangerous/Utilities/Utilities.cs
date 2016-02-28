using System;

namespace StreetsAreDangerous.Utilities
{
    public class Constants
    {
        /// <summary>
        /// Indica la lunghezza di una section.
        /// </summary>
        public const float SECTION_LENGTH = 50;

        /// <summary>
        /// Indica la metà della larghezza dell'area giocabile di una section.
        /// </summary>
        public const float SECTION_HALF_WIDTH = 8.50f;

        /// <summary>
        /// Durata della fase di accelerazione
        /// </summary>
        public const int SPRINT_DURATION = 3;
        
        /// <summary>
        /// Durata della velocità massima
        /// </summary>
        public const int MAX_SPEED_DURATION = 3;
        
        /// <summary>
        /// Durata della decelerazione
        /// </summary>
        public const int SLOWDOWN_DURATION = 15;
        
        /// <summary>
        /// Velocità massima raggiungibile
        /// </summary>
        public const float MAX_SPEED = 1.5f;

        /// <summary>
        /// Duranta del flash del pickup di un boost.
        /// </summary>
        public const float FLASH_DURATION = 0.7f;

        /// <summary>
        /// Peso del flash del pickup di un boost.
        /// </summary>
        public const int FLASH_STRENGTH = 300;

        /// <summary>
        /// Numero massime di vite in difficoltà difficile.
        /// </summary>
        public const short MAX_EASY_LIFE = 10;

        /// <summary>
        /// Valore da aggiungere alla coordinata Y del draw dei cuori indicanti la vita del giocatore durante la fase di recupero/perdita della vita.
        /// </summary>
        public const float LIFE_TRANSITION_STEP = 3.0f;

        /// <summary>
        /// Valore da aggiungere ad ogni step per il fade nero dopo le condizioni di sconfitta.
        /// </summary>
        public const float BLACK_FADE_STEP = 0.05f;

        /// <summary>
        /// Numero di tempi record da salvare.
        /// </summary>
        public const int NUMBER_OF_RECORDS_SAVED = 10;
    }

    /// <summary>
    /// Classe di Utilità per il gioco, principalmente per la generazione di numeri casuali
    /// </summary>
    public static class UtilityClass
    {
        private static Random randomizer = new Random();

        /// <summary>
        /// Restituisce un valore intero casuale, compreso tra minValue e maxValue
        /// </summary>
        /// <param name="minValue">Valore minimo</param>
        /// <param name="maxValue">Valore massimo</param>
        /// <returns>Valore intero compreso tra minValue e maxValue</returns>
        public static int nextInt(int minValue, int maxValue)
        {
            return randomizer.Next(minValue, maxValue);
        }

        /// <summary>
        /// Ritorna il numero di nemici da posizionare nella section a seconda del secondo di gioco raggiunto.
        /// </summary>
        /// <param name="seconds">Secondi di gioco effettivi trascorsi</param>
        /// <returns>Il numero di nemici da posizionare nella section</returns>
        public static int numberOfEnemies(double seconds)
        {
            return (int)(Math.Sqrt(seconds) / 4) + 1;
        }


        /// <summary>
        /// Restituisce un valore booleano casuale.
        /// </summary>
        /// <returns>Valore booleano true o false in modo casuale.</returns>
        public static bool nextBoolean()
        {
            return randomizer.Next(-100, 100) <= 0 ? true : false;
        }
    }
}
