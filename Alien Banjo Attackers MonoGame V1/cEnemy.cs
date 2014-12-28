using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Alien_Banjo_Attackers
{
    public class cEnemy
    {
        public Texture2D enemyTexture;
        Rectangle enemyRectangle;
        int enemyHealth;
        int enemyScoreValue;
        bool isAlive;
        
        // Plain Banjo Data
        int plainXSpeed = 3;

        // Hunter Banjo Data
        int runningHunterTotal = 0;
        int hunterXSpeed = 1;
        int hunterYSpeed = 1;

        // Deadly Banjo Data
        int deadlyXSpeed = 2;
        int deadlyYSpeed = 2;

        public Rectangle EnemyRectangle { get { return enemyRectangle; } set { enemyRectangle = value; } }
        public int EnemyScoreValue { get { return enemyScoreValue; } }
        public int EnemyHealth { get { return enemyHealth; } set { enemyHealth = value; } }
        public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

        public int PlainBanjoSpeeds { get { return plainXSpeed; } set { plainXSpeed = value; } }
        public int HunterBanjoSpeeds { set { hunterXSpeed = value; hunterYSpeed = value; } }
        public int RunningHunterTotal { get { return runningHunterTotal; } set { runningHunterTotal = value; } }
        public int DeadlyBanjoSpeeds { get { return deadlyXSpeed & deadlyYSpeed; } set { deadlyXSpeed = value; deadlyYSpeed = value; } }

        public cEnemy(Texture2D newTexture, Rectangle newRectangle, int newScoreValue,int newHealth)
        {
            enemyTexture = newTexture;
            enemyRectangle = newRectangle;
            enemyScoreValue = newScoreValue;
            enemyHealth = newHealth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(enemyTexture, enemyRectangle, Color.White);
        }

        /// <summary>
        ///  Method for updating the plainBanjo's, moving it from left to right and back and forth 
        ///  and moving it down if it touches one of the screen edge's
        /// </summary>
        /// <param name="game"></param>
        public void plainBanjo(Game1 game) 
        {
            if (enemyRectangle.Right > game.ScreenWidth) // If the banjo touches the right side of the screen, sets the speed to a negative number
            {
                enemyRectangle.Y = enemyRectangle.Y + 45;
                plainXSpeed = -3;
            }
            else if (enemyRectangle.Left < 0)
            {
                enemyRectangle.Y = enemyRectangle.Y + 25;
                plainXSpeed = 3;
            }

            enemyRectangle.X = enemyRectangle.X + plainXSpeed; // It will move left or right depending on wheater or not the speed is negative or positive
        }
        /// <summary>
        ///  The hunter banjo acts like a plain banjo for 5 seconds and then moves towards the player
        /// </summary>
        /// <param name="playerX"></param>
        /// The constantly updated player's X coordinate to move to
        /// <param name="playerY"></param>
        /// The constantly updated player's Y coordinate to move to
        /// <param name="game"></param>
        /// A reference object to the main game class
        public void hunterBanjo(int playerX,int playerY, Game1 game)
        {     
            while( runningHunterTotal != 300)
            {
                // This is to make the hunter banjo behave like a plain banjo for 5 seconds
                plainBanjo(game);
                runningHunterTotal = runningHunterTotal + 1;
                break;
            }   
            
            // When 5 seconds has passed, the hunter banjo moves towards the constantly updated player position
            if (runningHunterTotal == 300)
            {
                if (playerX < enemyRectangle.X)
                {
                    enemyRectangle.X = enemyRectangle.X - hunterXSpeed;
                }
                else if (playerX > enemyRectangle.X)
                {
                    enemyRectangle.X = enemyRectangle.X + hunterXSpeed;
                }

                if(playerY > enemyRectangle.Y)
                {
                    enemyRectangle.Y = enemyRectangle.Y + hunterYSpeed;
                }
            }
           
        }

        /// <summary>
        /// Method for updating the deadly banjos, that moves constantly to the updated player's position
        /// </summary>
        /// <param name="playerX"></param>
        /// The constantly updated player's X coordinate to move to
        /// <param name="playerY"></param>
        /// The constantly updated player's Y coordinate to move to
        public void deadlyBanjo(int playerX, int playerY) // Method for updating the deadly Banjo's
        {
            if (playerX < enemyRectangle.X)
            {
                enemyRectangle.X = enemyRectangle.X - deadlyXSpeed;
            }
            else if (playerX > enemyRectangle.X)
            {
                enemyRectangle.X = enemyRectangle.X + deadlyXSpeed;
            }
            
            if (playerY > enemyRectangle.Y)
            {
                enemyRectangle.Y = enemyRectangle.Y + deadlyYSpeed;
            }
        }

    }
}
