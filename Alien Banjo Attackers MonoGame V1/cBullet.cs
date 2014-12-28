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
    public class cBullet
    {
        public Texture2D bulletTexture;
        Rectangle bulletRectangle;
        int bulletYSpeed = 4;
        int deadlyBulletSpeedX = 2;
        int deadlyBulletSpeedY = 2;
        bool isAlive = false;
        int loopAgain;
        int currentXPosition; // This is used to get the player's position for the bullets that come out of the deadly banjo
        int currentYPosition;

        public Rectangle BulletRectangle { get{ return bulletRectangle;} set { bulletRectangle = value; } }
        public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

        public int CurrentXPosition { get { return currentXPosition; } set { currentXPosition = value; } }
        public int CurrentYPosition { get { return currentYPosition; } set { currentYPosition = value; } }
        public int LoopAgain { set { loopAgain = value; } }

        public cBullet(Texture2D newTexture, Rectangle newRectangle)
        {
            bulletTexture = newTexture;
            bulletRectangle = newRectangle;
            loopAgain = 0;
        }
        public void Update()
        {
            bulletRectangle.Y = bulletRectangle.Y - bulletYSpeed; // Moves the bullets that come out of the player up the screen only
            if (bulletRectangle.Y < 0) // If the bullet passes the top of the screen
            {
                bulletRectangle = new Rectangle(-800, -800, 1000 / 20, 1000 / 20);
                isAlive = false;
                // Moves the bullet off the screen and sets it to false to be able to be picked again
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(bulletTexture, bulletRectangle, Color.White);
        }

        /// <summary>
        /// Method for bullets that the deadly strummer banjos shoot after spawning
        /// Locks in player's position only once and moves to that location
        /// Does not follow the player
        /// </summary>
        /// <param name="player"></param>
        /// A reference object for the player class
        public void deadlyBanjoShoot(cPlayer player)
        {
            while (loopAgain == 0) // This is true for each bullet because in the constructor I set it to 0
            {
                // Makes sure to only get the position of the player as it spwans and only moves to that spot even if the player moves
                currentXPosition = player.PlayerRectangle.X;
                currentYPosition = player.PlayerRectangle.Y;
                loopAgain = 1; // Allows it to exit the loop and not search/update's the coordinates it should move to
            }

            if (currentXPosition < bulletRectangle.X) // if the player position is less then the bullets position, will move the bullet left
            {
                bulletRectangle.X = bulletRectangle.X - deadlyBulletSpeedX;
            }
            else if (currentXPosition > bulletRectangle.X)
            {
                bulletRectangle.X = bulletRectangle.X + deadlyBulletSpeedX;
            }
            
            if (currentYPosition > bulletRectangle.Y)
            {
                bulletRectangle.Y = bulletRectangle.Y + deadlyBulletSpeedY;
            }

        }

    }
}
