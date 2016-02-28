using Microsoft.Xna.Framework;
using StreetsAreDangerous.Utilities;
using StreetsAreDangerous.Enemies;
using StreetsAreDangerous.Elements;
using StreetsAreDangerous.PowerUps;
using System.Collections.Generic;

namespace StreetsAreDangerous
{
    public class Section
    {
        //Lista di nemici

        /// <summary>
        /// Array di elementi scenici.
        /// </summary>
        public ScenicElement[] ScenicElements { get; set; }

        /// <summary>
        /// Array di nemici
        /// </summary>
        public Enemy[] Enemies { get; set; }

        /// <summary>
        /// Array di power up.
        /// </summary>
        public PowerUp[] PowerUps { get; set; }

        /// <summary>
        /// Posizione sull'asse Z di questa section.
        /// </summary>
        public float zPosition { get; set; }

        /// <summary>
        /// Posizioni di chi sta emettendo suoni.
        /// </summary>
        public List<Vector3> emitters { get; set; }

        /// <summary>
        /// Costruttore vuoto per serializzazione.
        /// </summary>
        public Section()
        {
            emitters = new List<Vector3>();
            ScenicElements = new ScenicElement[0];
            Enemies = new Enemy[0];
            PowerUps = new PowerUp[0];
        }

        /// <summary>
        /// Genera una sezione di gioco.
        /// Di default genera i fences (confini visuali) e il plane.
        /// </summary>
        /// <param name="additionalElements">Numero di elementi scenici da aggiungere in modo casuale.</param>
        /// <param name="numberOfEnemies">Numero di nemici da generare in modo casuale</param>
        public Section(int additionalElements, int numberOfEnemies, float position)
        {
            emitters = new List<Vector3>();

            zPosition = position;

            ScenicElements = new ScenicElement[20 + additionalElements];
            //Plane
            ScenicElements[0] = new ScenicElement(0, Vector3.Zero, Vector3.Zero);
            //Fences
            ScenicElements[1] = new ScenicElement(1, new Vector3(9, 0, 12.5f), new Vector3(0, getRandomRotation(), 0));
            ScenicElements[2] = new ScenicElement(1, new Vector3(-9, 0, 12.5f), new Vector3(0, getRandomRotation(), 0));
            ScenicElements[3] = new ScenicElement(1, new Vector3(9, 0, 37.5f), new Vector3(0, getRandomRotation(), 0));
            ScenicElements[4] = new ScenicElement(1, new Vector3(-9, 0, 37.5f), new Vector3(0, getRandomRotation(), 0));
            //Trees
            ScenicElements[5] = new ScenicElement(2, new Vector3(12, 0, 12.5f + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, -getTreeRotation(), 0));
            ScenicElements[6] = new ScenicElement(2, new Vector3(-12, 0, 12.5f + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, getTreeRotation(), 0));
            ScenicElements[7] = new ScenicElement(2, new Vector3(12, 0, 37.5f + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, -getTreeRotation(), 0));
            ScenicElements[8] = new ScenicElement(2, new Vector3(-12, 0, 37.5f + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, getTreeRotation(), 0));
            ScenicElements[9] = new ScenicElement(2, new Vector3(12, 0, 25 + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, -getTreeRotation(), 0));
            ScenicElements[10] = new ScenicElement(2, new Vector3(-12, 0, 25 + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, getTreeRotation(), 0));
            ScenicElements[11] = new ScenicElement(2, new Vector3(12, 0, 50 + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, -getTreeRotation(), 0));
            ScenicElements[12] = new ScenicElement(2, new Vector3(-12, 0, 50 + (UtilityClass.nextInt(-500, 500) / 100.0f)), new Vector3(0, getTreeRotation(), 0));
            //MOAR PLANES
            ScenicElements[13] = new ScenicElement(0, new Vector3(24, 0, 0), Vector3.Zero);
            ScenicElements[14] = new ScenicElement(0, new Vector3(-24, 0, 0), Vector3.Zero);
            ScenicElements[15] = new ScenicElement(0, new Vector3(24, 12, 0), new Vector3(0, 0, MathHelper.ToRadians(90)));
            ScenicElements[16] = new ScenicElement(0, new Vector3(-24, 12, 0), new Vector3(0, 0, MathHelper.ToRadians(270)));
            ScenicElements[17] = new ScenicElement(0, new Vector3(0, 24, 0), new Vector3(0, 0, MathHelper.ToRadians(180)));
            ScenicElements[18] = new ScenicElement(0, new Vector3(12, 24, 0), new Vector3(0, 0, MathHelper.ToRadians(180)));
            ScenicElements[19] = new ScenicElement(0, new Vector3(-12, 24, 0), new Vector3(0, 0, MathHelper.ToRadians(180)));

            Enemies = new Enemy[numberOfEnemies];
            for (int counter = 0; counter < numberOfEnemies; counter++)
                if (UtilityClass.nextBoolean())
                    Enemies[counter] = new Dog();
                else
                    Enemies[counter] = new Boxes();

            //PowerUps
            //DA GENERARE GIA' ORDINATI, PER EVITARE CICLI DI ORDINAMENTO
            //Aumentare il numero col tempo? Forse ne basta uno per section.
            PowerUps = new PowerUp[1];
            if(Utilities.UtilityClass.nextBoolean())
                PowerUps[0] = new Boost();
            else
                PowerUps[0] = new Heart();
        }

        public void Update(float speed)
        {
            zPosition += speed;

            foreach (PowerUp power in PowerUps)
                if (power.isAvailable)
                    power.Update(zPosition);

            foreach (Enemy enemy in Enemies)
                if (enemy.IsAlive)
                {
                    enemy.Update(speed, zPosition);
                    if (enemy.PlaySound())
                        emitters.Add(enemy.Position);
                }

            SortEnemies();

            for (int counter = 0; counter < Enemies.Length - 1; counter++)
            {
                if (Enemies[counter].IsAlive && Enemies[counter + 1].IsAlive)
                    if (Enemies[counter].boundingBox.Intersects(Enemies[counter + 1].boundingBox))
                    {
                        Enemies[counter].IsAlive = false;
                        Enemies[counter + 1].IsAlive = false;
                        counter++;
                    }
            }
        }

        /// <summary>
        /// Effettua un Insertion Sort sull'array dei nemici.
        /// </summary>
        private void SortEnemies()
        {
            for (int counter = 1; counter < Enemies.Length; counter++)
            {
                Enemy x = Enemies[counter];
                int counter2 = counter;
                while (counter2 > 0 && Enemies[counter].Position.Z < Enemies[counter2 - 1].Position.Z)
                {
                    Enemies[counter2] = Enemies[counter2 - 1];
                    counter2--;
                }
                Enemies[counter2] = x;
            }
        }

        private float getTreeRotation()
        {
            return MathHelper.ToRadians(Utilities.UtilityClass.nextInt(0, 45));
        }

        private float getRandomRotation()
        {
            return Utilities.UtilityClass.nextBoolean() ? MathHelper.ToRadians(180) : 0;
        }
    }
}
