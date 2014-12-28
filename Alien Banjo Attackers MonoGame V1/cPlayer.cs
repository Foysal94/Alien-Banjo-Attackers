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
    public class cPlayer
    {
        public Texture2D playerTexture;
        Rectangle playerRectangle;
        int playerSpeed;
        int autoPlayerSpeed;
        
        public cPlayer(Texture2D newTexture, Rectangle newRectangle)
        {
            playerTexture = newTexture;
            playerRectangle = newRectangle;
            playerSpeed = 3;
            autoPlayerSpeed = 3;
        }

        public Rectangle PlayerRectangle { get { return playerRectangle; } set { playerRectangle = value; } }

        public void Update(Game1 game)
        {
            KeyboardState playerKeyboard = Keyboard.GetState();

            if (playerKeyboard.IsKeyDown(Keys.Left))
            {
                playerRectangle.X = playerRectangle.X - playerSpeed; // Moves the player left
            }

            if (playerKeyboard.IsKeyDown(Keys.Right))
            {
                playerRectangle.X = playerRectangle.X + playerSpeed; // Moves the player right
            }
    
            if (playerRectangle.Left < 0)
            {
                // This is to stop the player moving off the left side of the screen by setting its speed to X, the speed will only reset if the right key is pressed
                playerSpeed = 0;
                if (playerKeyboard.IsKeyDown(Keys.Right))
                {
                    playerSpeed = 3; 
                    playerRectangle.X = playerRectangle.X + playerSpeed;
                }
            }

            if (playerRectangle.Right > game.ScreenWidth)
            {
                playerSpeed = 0;
                if (playerKeyboard.IsKeyDown(Keys.Left))
                {
                    playerSpeed = 3;
                    playerRectangle.X = playerRectangle.X - playerSpeed;
                }
            }
        }

        /// <summary>
        /// Automatically moves the player from left to right
        /// </summary>
        /// <param name="game"></param>
        /// A reference object to the main game class
        public void AutoUpdate(Game1 game)
        {
            if (autoPlayerSpeed == -3)
            {
                playerRectangle.X = playerRectangle.X + autoPlayerSpeed; // Moves the accordion left
            }

            if (autoPlayerSpeed == 3)
            {
                playerRectangle.X = playerRectangle.X + autoPlayerSpeed; // Moves the accordion right
            }

            if (playerRectangle.Left < 0)
            {
                // This is to stop the player moving off the left side of the screen by setting its speed to X, the speed will only reset if the right key is pressed
                autoPlayerSpeed = 3;
            }

            if (playerRectangle.Right > game.ScreenWidth)
            {
                autoPlayerSpeed = -3;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, playerRectangle, Color.White);
        }

    }
}
