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
    public class cHUD
    {
        public static SpriteFont Font;

        static int score = 0;
        static int lives = 3;

        public static int Score { get { return score; } set { score = value; } }
        public static int Lives { get { return lives; } set { lives = value; } }

        public cHUD()
        {

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, "Score:" + Score.ToString(), new Vector2(120, 0), Color.Red);
            spriteBatch.DrawString(Font, "Lives:" + Lives.ToString(), new Vector2(0, 0), Color.Red);
            
        }

    }
}
