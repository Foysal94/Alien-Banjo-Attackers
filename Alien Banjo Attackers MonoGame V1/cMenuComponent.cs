using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Alien_Banjo_Attackers
{
    public class cMenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string[] menuItems; // The array to store the options of the menu
        int selectedIndex;

        Color normal = Color.White; // The colour for menu items that are not selected
        Color highlight = Color.Red; // When a menu item is hovered over, will set the colour to red

        KeyboardState currentKeyBoardState; // Holds the current state of the keyboard
        KeyboardState oldKeyBoardState; // Holds the previous state of the keyboard in the previous frame (Update and Draw are called 60 times a second)

        SpriteBatch spriteBatch1;
        SpriteFont spriteFont1;

        Vector2 position;
        float width = 0f;
        float height = 0f;

        public Vector2 Position { get { return position; } set { position = value; } }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;

                if (selectedIndex < 0) // Always makes sure the selected index or menu item cannot go below zero
                {
                    selectedIndex = 0;
                }

                if (selectedIndex >= menuItems.Length) // Makes sure that the selected index is not greater or equal to the length of the array
                {
                    selectedIndex = menuItems.Length - 1;
                }
            }
        }

        public cMenuComponent(Game game, SpriteBatch spriteBatch2, SpriteFont spriteFont2, string[] menuItems2)
            : base(game)
        {
            spriteBatch1 = spriteBatch2;
            spriteFont1 = spriteFont2;
            menuItems = menuItems2;
            MeasureMenu();
        }

        /// <summary>
        /// Measures the width of the largest string item in the array to center the menu
        /// </summary>
        private void MeasureMenu()
        {
            height = 0;
            width = 0;

            foreach (string item in menuItems)
            {
                //To center an object horizontally you take the width of what you want to center the object in, subtract the width of the object and divide that by two
                // This foreach loop goes through each item of the array, updating the width if the item width is greater then it.
                // The value of width will be used to center the menu, and to do this the width of the largest string item of the array will be needed

                Vector2 size = spriteFont1.MeasureString(item); // X property being the width of the string and the Y property being the height of the string.

                if (size.X > width)
                {
                    width = size.X;
                }

                height = height + spriteFont1.LineSpacing + 5; // Allows a gap between each item of the menu when drawn from top to bottom
            }

            position = new Vector2((Game.Window.ClientBounds.Width - width) / 2, (Game.Window.ClientBounds.Height - height) / 2);
        }
       
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Returns true if the key has been pressed in the last frame and is realeased/let go in the last frame
        /// </summary>
        /// <param name="key"></param>
        /// Excepts a key from the keyboard
        /// <returns></returns>
        public bool CheckKeys(Keys key)
        {
            return currentKeyBoardState.IsKeyUp(key) && oldKeyBoardState.IsKeyDown(key);
        }

        public override void Update(GameTime gameTime)
        {
            currentKeyBoardState = Keyboard.GetState();

            if (CheckKeys(Keys.Down))
            {
                selectedIndex = selectedIndex + 1;
                if (selectedIndex == menuItems.Length)
                {
                    // Resets the index back to 0 or the top of the list if the final item of the array is highlighted and the down key is pressed.
                    selectedIndex = 0;
                }
            }

            if (CheckKeys(Keys.Up))
            {
                selectedIndex = selectedIndex - 1;
                if (selectedIndex < 0)
                {
                    selectedIndex = menuItems.Length - 1;
                }
            }

            base.Update(gameTime);

            oldKeyBoardState = currentKeyBoardState;
            // The oldkeyboard state is now the currentkeyboard state
            // So the next time updated is called, it will have the current state of the keyboard and the old state
        }

        public void Draw(GameTime gameTime,SpriteBatch spritebatch)
        {

            Vector2 location = position;
            Color tint;

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                {
                    tint = highlight; // Sets the colour of the current selected index to the colour of the "highlight" variable
                }
                else
                {
                    tint = normal;
                }
                spriteBatch1.DrawString(spriteFont1, menuItems[i], location, tint);
                location.Y = location.Y + spriteFont1.LineSpacing + 5;
            }
        }
    }
}
