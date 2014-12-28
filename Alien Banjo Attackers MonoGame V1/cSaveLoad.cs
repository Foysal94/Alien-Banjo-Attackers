using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alien_Banjo_Attackers
{
    public class cSaveLoad
    {
        static string gameSave = "GameSave.txt";
        static string highScore = "HighScore.txt";

        int hunterBanjoNum;
        int plainBanjoNum;
        int deadlyBanjoNum;
        int dBulletNum;
        Random bulletPicker = new Random();
        Random dBulletPicker = new Random();
        Random pBanjoPicker = new Random();
        Random hBanjoPicker = new Random();
        Random dBanjoPicker = new Random();

        public cSaveLoad()
        {

        }

        /// <summary>
        /// Constantly checks the hud/game score if it is higher or equal to the current highscore saved on a file
        /// If true, it will overwrite the highscore written on that line.
        /// </summary>
        /// <param name="game"></param>
        /// A reference object to the main game class
        public void HighScoreSave(Game1 game)
        {
            StreamReader textIn = new StreamReader(highScore);
            string text = textIn.ReadLine(); // Reads whatever the current saved score is in the file
            textIn.Close();
            int score = int.Parse(text);
            if (cHUD.Score >= score) // Checks to see if ever the ingame score is higher then the one stored in the file
            {
                // Write's a new string that replaces the old score with the current gamescore since it is the new highscore and writes it to the file
                StreamWriter textOut = new StreamWriter(highScore);
                string newText = text.Replace(text, cHUD.Score.ToString()); 
                textOut.WriteLine(newText);
                textOut.Close();
            }
        }

        /// <summary>
        ///  Goes through each list of the game, and saving the relevant data of each object if their boolean
        ///  "isAlive" is set to true
        /// </summary>
        /// <param name="game"></param>
        /// A reference object to the main game class
        public void GameDataSave(Game1 game)
        {
            int pBanjo = 0;
            int hBanjo = 0;
            int dBanjo = 0;
            int bulletNumber = 0;
            int deadlyBulletNumber = 0;

            StreamWriter textOut = new StreamWriter(gameSave);

            textOut.WriteLine(cHUD.Lives);
            textOut.WriteLine(cHUD.Score);
            textOut.WriteLine("Player position" + " " + "(X:" + game.accordion.PlayerRectangle.X + " " + "Y:" + game.accordion.PlayerRectangle.Y + ")");

            foreach (cBullet b in game.bullets)
            {
                if (b.IsAlive == true)
                {
                    bulletNumber = bulletNumber + 1;
                    textOut.WriteLine("Bullet number" + " " + bulletNumber + " " + "(X:" + b.BulletRectangle.X + " " + "Y:" + b.BulletRectangle.Y + ")");
                }
            }

            foreach (cBullet b in game.dBanjoBullets)
            {
                if (b.IsAlive == true)
                {
                    deadlyBulletNumber = deadlyBulletNumber + 1;
                    textOut.WriteLine("DBullet number" + " " + deadlyBulletNumber + " " + "(X:" + b.BulletRectangle.X + " " + "Y:" + b.BulletRectangle.Y + ")");
                    textOut.WriteLine("MoveTo" + "(X:" + b.CurrentXPosition + " " + "Y:" + b.CurrentYPosition + ")");
                }

            }
            foreach (cEnemy plainBanjo in game.pBanjo)
            {
                if (plainBanjo.IsAlive == true)
                {
                    pBanjo = pBanjo + 1;
                    textOut.WriteLine("Plain banjo Number" + " " + pBanjo + " " + "(X:" + plainBanjo.EnemyRectangle.X + " " + "Y:" + plainBanjo.EnemyRectangle.Y + ")");
                    textOut.WriteLine("Pspeed:" + plainBanjo.PlainBanjoSpeeds);
                }
            }

            foreach (cEnemy hunterBanjo in game.hBanjo)
            {
                if (hunterBanjo.IsAlive == true)
                {
                    hBanjo = hBanjo + 1;
                    textOut.WriteLine("Hunter banjo number" + " " + hBanjo + " " + "(X:" + hunterBanjo.EnemyRectangle.X + " " + "Y:" + hunterBanjo.EnemyRectangle.Y + ")");
                    textOut.WriteLine("runninghunterTotal:" + hunterBanjo.RunningHunterTotal);
                    textOut.WriteLine("Hspeed:" + hunterBanjo.PlainBanjoSpeeds);
                }
            }

            foreach (cEnemy deadlyBanjo in game.dBanjo)
            {
                if (deadlyBanjo.IsAlive == true)
                {
                    dBanjo = dBanjo + 1;
                    textOut.WriteLine("Deadly banjo number" + " " + dBanjo + " " + "(X:" + deadlyBanjo.EnemyRectangle.X + " " + "Y:" + deadlyBanjo.EnemyRectangle.Y + ")");
                    textOut.WriteLine("Dhealth:" + deadlyBanjo.EnemyHealth);
                }
            }

            textOut.Close();
        }

        /// <summary>
        /// Creates an array of string, that contains all the lines of the file, each line holding data
        /// Goes through each line of the array checking if it starting with certain characters or words
        /// Picks objects from the game's list to assign the saved data to
        /// </summary>
        /// <param name="game"></param>
        /// A reference object to the main game class
        public void GameDataLoad(Game1 game)
        {
            // While the game is playing, the user might have delated the gameSave file, this creates a new gamesave file to stop the program breaking
            if (!File.Exists(gameSave))
            {
                File.Create(gameSave).Close();
            }

            string[] lines = File.ReadAllLines(gameSave); // Reads all lines of the file into an array

            foreach (string line in lines)
            {
                int loopAgain = 0;
                while (loopAgain == 0) // This loop is to make sure an object of the game list, bullets or banjos are always picked/loaded onto the screen
                {
                    // The lives and score are always the first two lines
                    if ( line == lines[0])
                    {
                        int lives = int.Parse(line);
                        cHUD.Lives = lives;
                        loopAgain = 1;
                    }
                    if ( line == lines[1])
                    {
                        int score = int.Parse(line);
                        cHUD.Score = score;
                        loopAgain = 1;
                    }
                
                    if (line.StartsWith("Player"))
                    {
                        // Reads the numbers after the "X:" plus two characters and upto " Y", the same again for numbers after "Y:" but plus two characters
                        // This will be the player X and Y coordinate
                        int startIndex = line.IndexOf("X:") + 2; 
                        float positionX = float.Parse(line.Substring(startIndex, line.IndexOf(" Y") - startIndex));
                        startIndex = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startIndex, line.IndexOf(")") - startIndex));

                        game.accordion.PlayerRectangle = new Rectangle((int)positionX, (int)positionY, 60, 40);
                        loopAgain = 1;
                    }
                
                    if (line.StartsWith("Bullet"))
                    {
                        // Reads off two coordinates from the line and makes a new rectangle with these coordinates
                        int startIndex = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startIndex, line.IndexOf(" Y") - startIndex));
                        startIndex = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startIndex, line.IndexOf(")") - startIndex));
                        Rectangle rectangle = new Rectangle((int)positionX, (int)positionY, 50, 30);

                        // Picks a random bullet from the list, and if its boolean "isAlive" is set to false, it will draw the bullet at the loaded rectangle position 
                        // Sets it's boolean "isAlive" to true to be able to be drawn and updated after the game has entered the playing state
                        int bulletNum = bulletPicker.Next(game.bullets.Count);
                        if (game.bullets[bulletNum].IsAlive == false)
                        {
                            game.bullets[bulletNum].BulletRectangle = rectangle;
                            game.bullets[bulletNum].IsAlive = true;
                            loopAgain = 1;
                        }
                    }

                    if (line.StartsWith("DBullet"))
                    {
                        int startInded = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startInded, line.IndexOf(" Y") - startInded));
                        startInded = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startInded, line.IndexOf(")") - startInded));
                        Rectangle rectangle = new Rectangle((int)positionX, (int)positionY, 50, 30);

                        dBulletNum = dBulletPicker.Next(game.dBanjoBullets.Count);
                        if (game.dBanjoBullets[dBulletNum].IsAlive == false)
                        {
                            game.dBanjoBullets[dBulletNum].BulletRectangle = rectangle;
                            game.dBanjoBullets[dBulletNum].IsAlive = true;
                            loopAgain = 1;
                        }
                    }

                    if(line.StartsWith("Move"))
                    {
                        int startInded = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startInded, line.IndexOf(" Y") - startInded));
                        startInded = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startInded, line.IndexOf(")") - startInded));

                        game.dBanjoBullets[dBulletNum].CurrentXPosition = (int)positionX;
                        game.dBanjoBullets[dBulletNum].CurrentYPosition = (int)positionY;
                        game.dBanjoBullets[dBulletNum].LoopAgain = 1;
                        loopAgain = 1;

                    }

                    if (line.StartsWith("Plain"))
                    {
                        int startIndex = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startIndex, line.IndexOf(" Y") - startIndex));
                        startIndex = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startIndex, line.IndexOf(")") - startIndex));
                        Rectangle rectangle = new Rectangle((int)positionX, (int)positionY, 40, 80);

                        // Picks a random plainBanjo from the list, and if its boolean "isAlive" is set to false, it will draw the plainBanjo at the loaded rectangle position 
                        // Sets it's boolean "isAlive" to true to be able to be drawn and updated after the game has entered the playing state
                        plainBanjoNum = pBanjoPicker.Next(game.pBanjo.Count);
                        if (game.pBanjo[plainBanjoNum].IsAlive == false)
                        {
                            game.pBanjo[plainBanjoNum].EnemyRectangle = rectangle;
                            game.pBanjo[plainBanjoNum].IsAlive = true;
                            loopAgain = 1;
                        }
                    }

                    if (line.StartsWith("Ps"))
                    {
                        int startIndex = line.IndexOf("d:") + 2;
                        int speed = int.Parse(line.Substring(startIndex));
                        game.pBanjo[plainBanjoNum].PlainBanjoSpeeds = speed; // The random plainbanjo  that was picked from the previous line will be the same index here
                        loopAgain = 1;
                    }

                    if (line.StartsWith("Hunter"))
                    {
                        int startIndex = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startIndex, line.IndexOf(" Y") - startIndex));
                        startIndex = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startIndex, line.IndexOf(")") - startIndex));
                        Rectangle rectangle = new Rectangle((int)positionX, (int)positionY, 40, 80);

                        hunterBanjoNum = hBanjoPicker.Next(game.hBanjo.Count);
                        if (game.hBanjo[hunterBanjoNum].IsAlive == false)
                        {
                            game.hBanjo[hunterBanjoNum].EnemyRectangle = rectangle;
                            game.hBanjo[hunterBanjoNum].IsAlive = true;
                            loopAgain = 1;

                        }
                    }

                    if (line.StartsWith("running"))
                    {
                        int startIndex = line.IndexOf("al:") + 3;
                        int counter = int.Parse(line.Substring(startIndex));
                        game.hBanjo[hunterBanjoNum].RunningHunterTotal = counter; 
                        loopAgain = 1;

                    }

                    if (line.StartsWith("Hs"))
                    {
                        int startIndex = line.IndexOf("d:") + 2;
                        int speed = int.Parse(line.Substring(startIndex));
                        game.hBanjo[hunterBanjoNum].PlainBanjoSpeeds = speed;
                        loopAgain = 1;
                    }

                    if (line.StartsWith("Deadly"))
                    {
                        int startIndex = line.IndexOf("X:") + 2;
                        float positionX = float.Parse(line.Substring(startIndex, line.IndexOf(" Y") - startIndex));
                        startIndex = line.IndexOf("Y:") + 2;
                        float positionY = float.Parse(line.Substring(startIndex, line.IndexOf(")") - startIndex));

                        Rectangle rectangle = new Rectangle((int)positionX, (int)positionY, 40, 80);

                        deadlyBanjoNum = dBanjoPicker.Next(game.dBanjo.Count);
                        if (game.dBanjo[deadlyBanjoNum].IsAlive == false)
                        {
                            game.dBanjo[deadlyBanjoNum].EnemyRectangle = rectangle;
                            game.dBanjo[deadlyBanjoNum].IsAlive = true;
                            loopAgain = 1;
                        }        
                    }

                    if (line.StartsWith("Dhe"))
                    {
                        int startIndex = line.IndexOf("h:") + 2;
                        int health = int.Parse(line.Substring(startIndex));
                        game.dBanjo[deadlyBanjoNum].EnemyHealth = health;
                        loopAgain = 1;
                    }
                }
            }

        }
    }
}
