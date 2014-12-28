using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Alien_Banjo_Attackers 
{
    public class cExplosion 
    {
        public Texture2D explosionTexture;

        float timer;
        float interval;
        bool isAlive;
        Vector2 position;

        int currentFrame;
        int width;
        int height;

        Rectangle sourceRectangle; // Will store the width and/or height of the current frame of the spritesheet

        public Vector2 Position { get { return position; } set { position = value; } }
        public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

        public cExplosion(Texture2D newtexture, Vector2 newPosition)
        {
            explosionTexture = newtexture;
            position = newPosition;
            isAlive = false;
            width = 128; // The width of the whole spritesheet
            height = 128; // The height of the whole spritesheet
            currentFrame = 1;
            timer = 0f;
            interval = 20f; // The time needed before the frame updates
        }

        public void Update(GameTime gameTime)
        {
            timer = timer + (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer >= interval)
            {
                timer = 0;
                currentFrame = currentFrame + 1;
            }

            sourceRectangle = new Rectangle(currentFrame * width, 0, width, height);

            if (currentFrame == 17)
            {
                // When the last frame has been animated, will set the instance of this class boolean "isAlive" to false and move it off screen
                isAlive = false;
                position = new Vector2(-1000, -1000);
                currentFrame = 1;
            }

        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(explosionTexture, position, sourceRectangle, Color.White);
        }
    }
}
