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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
       
        // GamesState
        enum GameState
        {
            StartMenu,
            HighScore,
            Loading,
            Playing,
            GameOver,
            PauseMenu,
        }
        GameState currentGameState = new GameState();
        
        // Game Data
        Texture2D backgroundTexture;
        Rectangle backgroundRectangle;

        SpriteFont mainFont;
        cExplosion explosion;

        KeyboardState currentKeyBoardState; // Holds the current state of the keyboard
        KeyboardState oldKeyBoardState; // Holds the previous state of the keyboard in the previous frame (Update and Draw are called 60 times a second)
        // These fields will be used to detect the single key presses

        // Menus
        cMenuComponent startMenu;
        cMenuComponent pauseMenu;
        cMenuComponent endMenu;
        cMenuComponent highScoreMenu;

        // An array for each menu items/options
        string[] startMenuItems = { "Start Game", "HighScore", "Load Game", "Exit" };
        string[] pauseMenuItems = { "Resume", "Exit to menu", "Exit" };
        string[] endMenuItems = { "Restart","Exit to menu", "Exit" };
        string[] highScoreMenuItems = { "Reset", "Back", "Exit" };

        // Files
        cSaveLoad saveLoad;
        string gameSave = "GameSave.txt";
        string highScore = "HighScore.txt";
        
        // Player Components 
        public cPlayer accordion;

        // Bullet Components
        cBullet noteBullet;
        cBullet deadlyBanjoBullet;
        bool drawBullet = false;
        int runningBulletTotal = 0;

        // Enemy/Hunter Components
        cEnemy plainBanjo;
        cEnemy hunterBanjo;
        cEnemy deadlyBanjo;
        int enemySpawnCounter = 2;

        // Lists
        public List<cBullet> bullets = new List<cBullet>();
        public List<cBullet> dBanjoBullets = new List<cBullet>();
        public List<cEnemy> totalEnemies = new List<cEnemy>();
        public List<cEnemy> pBanjo = new List<cEnemy>();
        public List<cEnemy> hBanjo = new List<cEnemy>();
        public List<cEnemy> dBanjo = new List<cEnemy>();
        public List<cExplosion> explosions = new List<cExplosion>();

        // Random objects
        Random ranX = new Random(screenWidth); // To pick a random X coordinate across the whole screen
        Random ranY = new Random(screenHeight); // To pick a random Y coordinate down the screen
        Random explosionPicker = new Random(); // Will be used later to pick a random index of the explosions list
        Random enemyPicker = new Random();
        Random bulletPicker = new Random();
        Random pBanjoPicker = new Random();
        Random hBanjoPicker = new Random();
        Random dBanjoPicker = new Random();

        // Screen size
        const int screenWidth = 1200;
        const int screenHeight = 700;

        public int ScreenHeight { get { return screenHeight; } }
        public int ScreenWidth { get { return screenWidth; } }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            currentGameState = GameState.StartMenu; // So the game is started in the startmenu gamestate and will draw it
           
            // Create the files to save the game objects to, if they do not exist yet
            if (!File.Exists(gameSave))
            {
                File.Create(gameSave).Close();
            }
            if (!File.Exists(highScore))
            {
                File.Create(highScore).Close();
            }

            // Checks to see if the highscore file has zero lines in it, since the user might have deleted the line within the file.
            // If it has no lines in the file, adds a line with a zero to it
            int lineCount = File.ReadLines(highScore).Count();
            if (lineCount == 0)
            {
                StreamWriter textIn = new StreamWriter(highScore);
                textIn.WriteLine("0");
                textIn.Close();
            }

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Game Data Loading
            mainFont = Content.Load<SpriteFont>("Font1");

            // File data loading
            saveLoad = new cSaveLoad();
            
            // Loads all the menu and adds it to the game components list to be updated
            startMenu = new cMenuComponent(this,spriteBatch,mainFont,startMenuItems);
            pauseMenu = new cMenuComponent(this, spriteBatch, mainFont, pauseMenuItems);
            endMenu = new cMenuComponent(this, spriteBatch, mainFont, endMenuItems);
            highScoreMenu = new cMenuComponent(this, spriteBatch, mainFont, highScoreMenuItems);
            Components.Add(startMenu);
            Components.Add(pauseMenu);
            Components.Add(endMenu);
            Components.Add(highScoreMenu);

            // Onscreen text/score
            cHUD.Font = mainFont;
            
            // Background Loading
            backgroundTexture = Content.Load<Texture2D>("Space Background");
            backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Player Loading
            accordion = new cPlayer(Content.Load<Texture2D>("accordian"),new Rectangle(screenWidth / 2,screenHeight / 2 + 350 , 60, 40));

            // Hunter/Enemy Loading
            // Creates a certain number of instances of the plainBanjos and adds each instance to their respective lists
            for (int i = 0; i < 100; i = i + 1)
            {
                plainBanjo = new cEnemy(Content.Load<Texture2D>("PlainBanjo"), new Rectangle(0, 0, screenWidth / 20, screenHeight / 20), 10, 1);
                pBanjo.Add(plainBanjo);
                totalEnemies.Add(plainBanjo);
            }

            for (int i = 0; i < 50; i = i + 1)
            {
                hunterBanjo = new cEnemy(Content.Load<Texture2D>("HunterBanjo"), new Rectangle(0, 0, screenWidth / 20, screenHeight / 20), 20, 1);
                hBanjo.Add(hunterBanjo);
                totalEnemies.Add(hunterBanjo);
            }

            for (int i = 0; i < 8; i = i + 1)
            {
                deadlyBanjo = new cEnemy(Content.Load<Texture2D>("DeadlyBanjo"), new Rectangle(0, 0, screenWidth / 20, screenHeight / 20), 50, 2);
                dBanjo.Add(deadlyBanjo);
                totalEnemies.Add(deadlyBanjo);
            }

            // Bullets Loading
            for (int i = 0; i < 60; i = i + 1)
            {
                noteBullet = new cBullet(Content.Load<Texture2D>("notebullet"), new Rectangle(400, 450, screenWidth / 20, screenHeight / 20));
                bullets.Add(noteBullet);
            }

            for (int i = 0; i < 30; i = i + 1)
            {
                deadlyBanjoBullet = new cBullet(Content.Load<Texture2D>("noteBullet"), new Rectangle(400, 450, screenWidth / 20, screenHeight / 20));
                dBanjoBullets.Add(deadlyBanjoBullet);
            }

            // Explosion loading
            for (int i = 0; i < 12; i = i + 1)
            {
                explosion = new cExplosion(Content.Load<Texture2D>("explosion"),new Vector2(-1000,-1000));
                explosions.Add(explosion);
            }
        }

       
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState playerKeyboard = Keyboard.GetState(); // This keyboard is used for player actions
            currentKeyBoardState = Keyboard.GetState(); // This is used for player actions in the menustates
            int loopAgain = 0;
            
            // GameState
            switch (currentGameState)
            {
                // The game is initialized in this state
                case GameState.StartMenu:
                    // Checks if the each option of the menu is selected and the enter key is pressed and let go before continuing inside the if statement
                    if (startMenu.SelectedIndex == 0 && CheckKeys(Keys.Enter))
                    {
                        accordion.PlayerRectangle = new Rectangle(screenWidth / 2, screenHeight / 2 + 300, 60, 40);
                        currentGameState = GameState.Playing;
                    }

                    if (startMenu.SelectedIndex == 1 && CheckKeys(Keys.Enter))
                    {
                        accordion.PlayerRectangle = new Rectangle(screenWidth / 2, screenHeight / 2 + 300, 60, 40);
                        currentGameState = GameState.HighScore;
                        highScoreMenu.SelectedIndex = 0;
                    }

                    if (startMenu.SelectedIndex == 2 && CheckKeys(Keys.Enter))
                    {
                        currentGameState = GameState.Loading;
                    }

                    if (startMenu.SelectedIndex == 3 && CheckKeys(Keys.Enter))
                    {
                        Exit();
                    }
                    break;

                case GameState.HighScore:

                    if (!File.Exists(highScore))
                    {
                        File.Create(highScore).Close();
                        StreamWriter textIn = new StreamWriter(highScore);
                        textIn.WriteLine("0");
                        textIn.Close();
                    }

                    accordion.AutoUpdate(this); // Automatically moves the player's accordion
                    AutoShoot(); // Automatically shoots bullets from the player
                    Collisions();

                    // Spwans a maximum of two enemies if the counter does not equal 0, the type of banjo spawned are random
                    if (enemySpawnCounter != 0)
                    {
                        LoadEnemies();
                        enemySpawnCounter = enemySpawnCounter - 1;
                    }

                    foreach (cBullet b in bullets)
                    {
                        if (b.IsAlive == true)
                        {
                            b.Update(); // Moving the bullets up the screen
                        }
                    }

                    foreach (cEnemy plainBanjo in pBanjo)
                    {
                        if (plainBanjo.IsAlive == true)
                        {
                            plainBanjo.plainBanjo(this); // Movement for each plainBanjo 
                        }
                    }

                    foreach (cExplosion e in explosions)
                    {
                        if (e.IsAlive == true)
                        {
                            e.Update(gameTime);
                        }
                    }

                    if (highScoreMenu.SelectedIndex == 0 && CheckKeys(Keys.Enter))
                    {
                        // Resets the highscore to 0 if the reset option is selected
                        StreamReader textIn = new StreamReader(highScore);
                        // Reads in the current highscore off the file
                        string text = textIn.ReadLine();
                        textIn.Close();
                        int score = int.Parse(text);

                            StreamWriter textOut = new StreamWriter(highScore);
                            // Creates a new string that contains "0", which is a replaced string of the current highscore
                            // Then writes this string to current file replacing the old one
                            string newText = text.Replace(text, "0");
                            textOut.WriteLine(newText);
                            textOut.Close();
                       
                    }

                    if (highScoreMenu.SelectedIndex == 1 && CheckKeys(Keys.Enter))
                    {
                        RestartGame();
                        currentGameState = GameState.StartMenu;
                        startMenu.SelectedIndex = 0;
                    }

                    if (highScoreMenu.SelectedIndex == 2 && CheckKeys(Keys.Enter))
                    {
                        Exit();
                    }

                    break;

                case GameState.Loading:
                    int banjoTruthCounter = 0;
                    RestartGame();
                    saveLoad.GameDataLoad(this); // If the load game option was picked, it will load the last saved data from the file

                    // Checks if atleast one banjo is alive after loading, so the enemyspawncounter equals zero since there is no need for more banjos to spawn
                    // If all the banjos are dead, the enemyspawncounter value remains 2 so it spawns banjos
                    // This is good if the player selected the loadgame option, but there is no data inside the gameSave file
                    foreach (cEnemy banjo in totalEnemies)
                    {
                        if (banjo.IsAlive == true)
                        {
                            banjoTruthCounter = banjoTruthCounter + 1;
                            break;
                        }
                    }

                    if (banjoTruthCounter == 1)
                    {
                        enemySpawnCounter = 0;
                    }

                    currentGameState = GameState.Playing;
                    break;

                case GameState.Playing:
                    // Player Updating/Data
                    // Gets the player X and Y postion 60 times a second for the hunter and deadly banjos
                    int accordianX = accordion.PlayerRectangle.X;
                    int accordianY = accordion.PlayerRectangle.Y;
                    accordion.Update(this);
                    saveLoad.HighScoreSave(this); // Constantly tracking and updating the highscore

                    // Bullets Updating/Data for the bullets that come out of the player
                    Shoot();
                    Collisions();

                    // Spwans a maximum of two enemies if the counter does not equal 0, the type of hunters spwaned are random
                    if (enemySpawnCounter != 0)
                    {
                        LoadEnemies();
                        enemySpawnCounter = enemySpawnCounter - 1;
                    }

                    foreach (cBullet b in bullets)
                    {
                        if (b.IsAlive == true)
                        {
                            b.Update(); // Moving the bullets up the screen
                        }
                    }

                    // Bullets Updating/Data for the bullets that come out of the player deadly banjos
                    foreach (cBullet b in dBanjoBullets)
                    {
                        if (b.IsAlive == true)
                        {
                            b.deadlyBanjoShoot(accordion);

                            if (b.BulletRectangle.Bottom == screenHeight)
                            {
                                b.IsAlive = false;
                                b.BulletRectangle = new Rectangle(-800, -800, screenWidth / 20, screenHeight / 20);
                            }
                            if (b.BulletRectangle.Bottom > accordion.PlayerRectangle.Center.Y) // Moves the bullet off the screen when it touches the bottom of the screen
                            {
                                b.IsAlive = false;
                                b.BulletRectangle = new Rectangle(-800, -800, screenWidth / 20, screenHeight / 20);
                            }

                            if (b.BulletRectangle.Intersects(accordion.PlayerRectangle)) 
                            {
                                // If the bullet collides with the player, moves it off the screen and removes a life
                                b.IsAlive = false;
                                b.BulletRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                                if (cHUD.Lives != 0)
                                {
                                    cHUD.Lives = cHUD.Lives - 1;
                                }
                            }
                        }
                    }

                    // Updates for each plainbanjo that is alive
                    foreach (cEnemy plainBanjo in pBanjo)
                    {
                        if (plainBanjo.IsAlive == true)
                        {
                            plainBanjo.plainBanjo(this); // Movement for each plainBanjo 

                            if (plainBanjo.EnemyRectangle.Bottom > screenHeight) // if the banjo has fallen off the screen
                            {
                                while (loopAgain == 0)
                                {
                                    // Picks an explosion from the list and sets its boolean "isAlive" to true, so it can be drawn and updated
                                    // Also moves the explosion position to the banjo's position
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(plainBanjo.EnemyRectangle.X, plainBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                // Sets it to false and moves it off the screen, also sets the enemy spwan counter to 2 to spwan 2 more enemies
                                plainBanjo.IsAlive = false;
                                plainBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20);
                                enemySpawnCounter = 2;
                                cHUD.Lives = 0;
                            }

                            if (plainBanjo.EnemyRectangle.Intersects(accordion.PlayerRectangle)) // if the banjo instersects the player
                            {
                                while (loopAgain == 0)
                                {
                                    // Picks an explosion from the list and sets its boolean "isAlive" to true, so it can be drawn and updated
                                    // Also moves the explosion position to the banjo's position
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(plainBanjo.EnemyRectangle.X, plainBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                // Sets it to false and moves it off the screen
                                plainBanjo.IsAlive = false;
                                plainBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                                enemySpawnCounter = 2;
                                if (cHUD.Lives != 0)
                                {
                                    cHUD.Lives = cHUD.Lives - 1;
                                }
                            }
                        }
                    }

                    // HunterBanjo
                    foreach (cEnemy hunterBanjo in hBanjo)
                    {
                        if (hunterBanjo.IsAlive == true)
                        {
                            hunterBanjo.hunterBanjo(accordianX, accordianY,this);

                            if (hunterBanjo.EnemyRectangle.Bottom > screenHeight)
                            {
                                while (loopAgain == 0)
                                {
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(hunterBanjo.EnemyRectangle.X, hunterBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                hunterBanjo.IsAlive = false;
                                hunterBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20);
                                hunterBanjo.RunningHunterTotal = 0;
                                cHUD.Lives = 0;
                            }

                            if (hunterBanjo.EnemyRectangle.Intersects(accordion.PlayerRectangle))
                            {
                                while (loopAgain == 0)
                                {
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(hunterBanjo.EnemyRectangle.X, hunterBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                hunterBanjo.IsAlive = false;
                                hunterBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                                hunterBanjo.RunningHunterTotal = 0;
                                enemySpawnCounter = 2;
                                if (cHUD.Lives != 0)
                                {
                                    cHUD.Lives = cHUD.Lives - 1;
                                }
                            }
                        }
                    }
                    
                     //DeadlyBanjo
                    foreach (cEnemy deadlyBanjo in dBanjo)
                    {
                        if (deadlyBanjo.IsAlive == true) 
                        {
                            deadlyBanjo.deadlyBanjo(accordianX, accordianY);

                            if (deadlyBanjo.EnemyRectangle.Bottom > screenHeight)
                            {
                                while (loopAgain == 0)
                                {
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(deadlyBanjo.EnemyRectangle.X, deadlyBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                deadlyBanjo.IsAlive = false;
                                deadlyBanjo.EnemyRectangle = new Rectangle(-500, -500, screenWidth / 20, screenHeight / 20);
                                cHUD.Lives = 0;
                            }

                            if (deadlyBanjo.EnemyRectangle.Intersects(accordion.PlayerRectangle))
                            {
                                while (loopAgain == 0)
                                {
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(deadlyBanjo.EnemyRectangle.X, deadlyBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                deadlyBanjo.IsAlive = false;
                                deadlyBanjo.EnemyRectangle = new Rectangle(-200, -100, screenWidth / 20, screenHeight / 20);
                                enemySpawnCounter = 2;

                                if (cHUD.Lives != 0)
                                {
                                    cHUD.Lives = cHUD.Lives - 1;
                                }
                            } 
                        }
                        
                    }

                    if(CheckKeys(Keys.Escape))
                    { 
                        // Enters the pause menu gamestate, so will draw a menu with the items if the player presses escape.
                        currentGameState = GameState.PauseMenu;
                        pauseMenu.SelectedIndex = 0; // Resets the index to the top of the menu
                    }

                    if (CheckKeys(Keys.S))
                    {
                        saveLoad.GameDataSave(this); // Saves the game if the S key is pressed and let go of
                    }

                     //Checks to see if the lives ever equal zero, to stop the game and move to a gameover state
                    if (cHUD.Lives == 0)
                    {
                        currentGameState = GameState.GameOver;
                        endMenu.SelectedIndex = 0;
                    }
                    
                    // Explosions
                    foreach (cExplosion e in explosions)
                    {
                        if (e.IsAlive == true)
                        {
                           e.Update(gameTime);
                        }
                    }
                    break;

                case GameState.PauseMenu:

                    if (pauseMenu.SelectedIndex == 0 && CheckKeys(Keys.Enter))
                    {
                        currentGameState = GameState.Playing;
                    }

                    if (pauseMenu.SelectedIndex == 1 && CheckKeys(Keys.Enter))
                    {
                        RestartGame();
                        currentGameState = GameState.StartMenu;
                        startMenu.SelectedIndex = 0;
                    }

                    if (pauseMenu.SelectedIndex == 2 && CheckKeys(Keys.Enter))
                    {
                        Exit();
                    }

                    break;

                case GameState.GameOver:
                    
                    if (endMenu.SelectedIndex == 0 && CheckKeys(Keys.Enter))
                    {
                        RestartGame();
                        currentGameState = GameState.Playing;
                    }

                    if (endMenu.SelectedIndex == 1 && CheckKeys(Keys.Enter))
                    {
                        RestartGame();
                        currentGameState = GameState.StartMenu;
                        startMenu.SelectedIndex = 0;
                    }

                    if (endMenu.SelectedIndex == 2 && CheckKeys(Keys.Enter))
                    {
                        Exit();
                    }

                    break;
            }

            oldKeyBoardState = currentKeyBoardState;
            // The oldkeyboard state is now the currentkeyboard state
            // So the next time updated is called, it will have the current state of the keyboard and the old state
            base.Update(gameTime);
        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            KeyboardState playerKeyboard = new KeyboardState();

            spriteBatch.Begin();

            // Background Drawing
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);

            // Menu Drawing or GameState
            switch (currentGameState)
            {
                case GameState.StartMenu:
                    startMenu.Draw(gameTime, spriteBatch); // Draws the startmenu if the game is in the StartMenu state
                    break;

                case GameState.HighScore: // In this state the objects are drawn and moved automatically
                    accordion.Draw(spriteBatch);
                    foreach (cBullet b in bullets)
                    {
                        if (b.IsAlive == true) // Only draws if it the boolean "isAlive" for each cBullet instance is set to true
                        {
                            b.Draw(spriteBatch);
                        }
                    }

                    foreach (cEnemy plainBanjo in pBanjo)
                    {
                        if (plainBanjo.IsAlive == true) // Only draws if it the boolean "isAlive" for each cEnemy instance is set to true
                        {
                            plainBanjo.Draw(spriteBatch);
                        }
                    }

                    foreach (cExplosion e in explosions)
                    {
                        if (e.IsAlive == true)
                        {
                            e.Draw(spriteBatch);
                        }
                    }

                    // While playing the user might have deleated the highscore file.
                    // This creates a new file and writes a line to it, so the program does not break

                    // Reads the current highscore from a file, and drawing it at a certain positiion along with the menu
                    StreamReader textOut = new StreamReader(highScore);
                    string text = textOut.ReadLine();
                    textOut.Close();
                    Vector2 position = new Vector2(screenWidth/2 - 20,screenHeight/2 - 50); // Position to draw the score
                    spriteBatch.DrawString(mainFont, "The current highScore is", new Vector2(440, 250), Color.White);
                    spriteBatch.DrawString(mainFont, text, position, Color.White);
                    highScoreMenu.Position = new Vector2(highScoreMenu.Position.X, 350);
                    highScoreMenu.Draw(gameTime, spriteBatch); // highscore menu drawn
                 
                    break;

                case GameState.Playing:

                    // Bullet Drawing
                    foreach (cBullet b in bullets)
                    {
                        if (b.IsAlive == true) // Only draws if it the boolean "isAlive" for each cBullet instance is set to true
                       {
                          b.Draw(spriteBatch);
                       }
                    }

                    foreach (cBullet b in dBanjoBullets)
                    {
                        if (b.IsAlive == true)
                        {
                            b.Draw(spriteBatch);
                        }
                    }

                    // Player Drawing
                    accordion.Draw(spriteBatch);

                    // Onscreen text Drawing
                    cHUD.Draw(spriteBatch);

                    // Banjo Drawing
                    foreach (cEnemy plainBanjo in pBanjo)
                    {
                        if (plainBanjo.IsAlive == true) // Only draws if it the boolean "isAlive" for each cEnemy instance is set to true
                        {
                            plainBanjo.Draw(spriteBatch);
                        }
                    }

                    foreach (cEnemy hunterBanjo in hBanjo)
                    {
                        if (hunterBanjo.IsAlive == true)
                        {
                            hunterBanjo.Draw(spriteBatch);
                        }
                    }

                    foreach (cEnemy deadlyBanjo in dBanjo)
                    {
                        if (deadlyBanjo.IsAlive == true)
                        {
                            deadlyBanjo.Draw(spriteBatch);
                        }
                    }

                    if(playerKeyboard.IsKeyDown(Keys.Escape))
                    {
                        currentGameState = GameState.PauseMenu;
                    }

                    // Explosion Drawing
                    foreach (cExplosion e in explosions)
                    {
                        if (e.IsAlive == true)
                        {
                            e.Draw(spriteBatch);
                        }
                    }

                    break;

                case GameState.PauseMenu:
                    pauseMenu.Draw(gameTime, spriteBatch);
                    break;

                case GameState.GameOver:
                    spriteBatch.DrawString(mainFont, "You have ran out of lives", new Vector2(420, 200), Color.White);
                    spriteBatch.DrawString(mainFont, "Your score was" + " " + cHUD.Score, new Vector2(470, 240), Color.White);
                    endMenu.Draw(gameTime, spriteBatch); // Draws this menu when the game is over
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Returns the state of the key last frame and the current frame, update and draw are called 60 times/frames per second.
        /// </summary>
        /// <param name="key"></param>
        /// Excepts a key from the keyboard
        public bool CheckKeys(Keys key)
        {
            return currentKeyBoardState.IsKeyUp(key) && oldKeyBoardState.IsKeyDown(key);
        }

        /// <summary>
        /// Move's all game objects off the screen and set's their booleans "isAlive" to flase so they cannot be updated/drawn
        /// </summary>
        public void RestartGame()
        {
            // Moves all the objects off the screen, and their respective booleans "isAlive" to false
            foreach (cEnemy plainBanjo in pBanjo)
            {
                plainBanjo.IsAlive = false;
                plainBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20);
            }

            foreach (cEnemy hunterBanjo in hBanjo)
            {
                hunterBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                hunterBanjo.RunningHunterTotal = 0; // Resets the runningHunterTotal for the hunterBanjo
                hunterBanjo.IsAlive = false;
            }

            foreach (cEnemy deadlyBanjo in dBanjo)
            {
                deadlyBanjo.EnemyRectangle = new Rectangle(-100, -200, screenWidth / 20, screenHeight / 20); // - 500, - 400
                deadlyBanjo.EnemyHealth = 2; // Resets the health for the deadly Banjo
                deadlyBanjo.IsAlive = false;
            }

            foreach (cBullet b in bullets)
            {
                b.BulletRectangle = new Rectangle(-800, -800, screenWidth / 20, screenHeight / 20);
                b.IsAlive = false;
            }

            foreach (cBullet b in dBanjoBullets)
            {
                b.BulletRectangle = new Rectangle(-800, -800, screenWidth / 20, screenHeight / 20);
                b.IsAlive = false;
            }

            foreach (cExplosion e in explosions)
            {
                e.Position = new Vector2(-1000, -1000);
                e.IsAlive = false;
            }

            // Moves the player back to middle of the screen and resets all game data
            accordion.PlayerRectangle = new Rectangle(screenWidth / 2,screenHeight / 2 + 300 , 60, 40); 
            cHUD.Lives = 3;
            cHUD.Score = 0;
            enemySpawnCounter = 2;
        }

        /// <summary>
        ///  This method allows the player to shoot bullets while the game is in the playing state
        ///  When the counter equals 10, picks a random bullet from the list and set's their boolean "isAlive" to true to be able to be drawn and updated
        /// </summary>
        public void Shoot()
        {
            KeyboardState playerKeyboard = Keyboard.GetState();

            // Runs the counter if space is not being pressed
            if (drawBullet == false && playerKeyboard.IsKeyUp(Keys.Space)) 
            {
                if (runningBulletTotal != 10)
                {
                    runningBulletTotal = runningBulletTotal + 1;
                }
            } 

            if (playerKeyboard.IsKeyDown(Keys.Space))
            {
                // Only fires if the counter equals 10
                if (runningBulletTotal == 10)
                {
                    drawBullet = true;
                    runningBulletTotal = 0; 
                }

                while (drawBullet == true)
                {
                    //Picks a random bullet from the list and sets it's boolean "isAlive" to true to be able to be drawn and updated
                    int bulletNum = bulletPicker.Next(bullets.Count);
                    if (bullets[bulletNum].IsAlive == false)
                    {
                        bullets[bulletNum].BulletRectangle = new Rectangle(accordion.PlayerRectangle.X, accordion.PlayerRectangle.Y, 50, 30);
                        bullets[bulletNum].IsAlive = true;
                        drawBullet = false; //Sets it to false so anoter bullet cant be picked/drawn
                    }
                }

                while (drawBullet == false)
                {
                    // Even if space is held down still updates the counter from 0 after resetting
                    runningBulletTotal = runningBulletTotal + 1;
                    break;
                }
            }
        }

        /// <summary>
        /// This method makes the accordian automatically shoot bullets everytime the counter equals zero, and will be used for automatic gameplay in the highscore state
        /// </summary>
        public void AutoShoot() 
        {
            if (drawBullet == false)
            {
                if (runningBulletTotal != 10)
                {
                    runningBulletTotal = runningBulletTotal + 1;
                }
            }


            if (runningBulletTotal == 10)
            {
                drawBullet = true;
                runningBulletTotal = 0;
            }

            while (drawBullet == true) // This while loop makes sure that a bullet is always picked
            {
                //Picks a random bullet from the list and sets it's boolean "isAlive" to true to be able to be drawn and updated
                int bulletNum = bulletPicker.Next(bullets.Count);
                if (bullets[bulletNum].IsAlive == false)
                {
                    bullets[bulletNum].BulletRectangle = new Rectangle(accordion.PlayerRectangle.X, accordion.PlayerRectangle.Y, 50, 30);
                    bullets[bulletNum].IsAlive = true;
                    drawBullet = false; // Sets it to false so anoter bullet cant be picked
                }
            }
        }

        /// <summary>
        /// Constantly checking if each bullet in the list has collided with any of the banjos in their respective list's
        /// If collided, moves bullet and banjo off screen and sets their boolean "isAlive" to false;
        /// Picks a random explosion from their list, sets it's "isAlive" to true to be drawn and updated
        /// </summary>
        public void Collisions()
        {
            int loopAgain = 0;

            foreach (cBullet b in bullets)
            {
                if (b.IsAlive == true) // Only checks if it's boolean "isAlive" is set to true 
                {
                    foreach (cEnemy plainBanjo in pBanjo)
                    {
                        if (b.BulletRectangle.Intersects(plainBanjo.EnemyRectangle))
                        {
                            while (loopAgain == 0)
                            {
                                // Picks an explosion from the list and sets its boolean "isAlive" to true, so it can be drawn and updated
                                // Also moves the explosion position to the banjo's position
                                int explosionNum = explosionPicker.Next(explosions.Count);
                                if (explosions[explosionNum].IsAlive == false)
                                {
                                    explosions[explosionNum].IsAlive = true;
                                    explosions[explosionNum].Position = new Vector2(plainBanjo.EnemyRectangle.X, plainBanjo.EnemyRectangle.Y);
                                    loopAgain = 1;
                                }
                            }

                            // Moves the bullet and banjo offscreen and sets their bollean "isAlive" to false to be able to be picked again to spwan.
                            plainBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                            plainBanjo.IsAlive = false;
                            b.BulletRectangle = new Rectangle(-200, -100, screenWidth / 20, screenHeight / 20);
                            b.IsAlive = false;
                            cHUD.Score = cHUD.Score + plainBanjo.EnemyScoreValue; // Updates the score
                            enemySpawnCounter = 2; //Resets the spwan counter to 2 so more enemies can be drawn onto the screen
                        }

                    }

                    foreach (cEnemy hunterBanjo in hBanjo)
                    {
                        if (b.BulletRectangle.Intersects(hunterBanjo.EnemyRectangle))
                        {
                            while (loopAgain == 0)
                            {
                                int explosionNum = explosionPicker.Next(explosions.Count);
                                if (explosions[explosionNum].IsAlive == false)
                                {
                                    explosions[explosionNum].IsAlive = true;
                                    explosions[explosionNum].Position = new Vector2(hunterBanjo.EnemyRectangle.X, hunterBanjo.EnemyRectangle.Y);
                                    loopAgain = 1;
                                }
                            }

                            hunterBanjo.EnemyRectangle = new Rectangle(-1000, -1000, screenWidth / 20, screenHeight / 20); // - 500, - 400
                            hunterBanjo.IsAlive = false;
                            hunterBanjo.RunningHunterTotal = 0; //Resets the counter for how long the hunter acts like a banjo, since its not alive it will not add to the counter
                            b.BulletRectangle = new Rectangle(-200, -100, screenWidth / 20, screenHeight / 20);
                            b.IsAlive = false;
                            cHUD.Score = cHUD.Score + hunterBanjo.EnemyScoreValue;
                            enemySpawnCounter = 2;

                        }

                    }

                    foreach (cEnemy deadlyBanjo in dBanjo)
                    {
                        // Deadly banjo has a health of 2, so will need 2 collisions before it can be moved off the screen like the plain and hunter banjos
                        if (b.BulletRectangle.Intersects(deadlyBanjo.EnemyRectangle))
                        {
                            deadlyBanjo.EnemyHealth = deadlyBanjo.EnemyHealth - 1;
                            b.BulletRectangle = new Rectangle(-200, -100, screenWidth / 20, screenHeight / 20);
                            b.IsAlive = false;

                            if (deadlyBanjo.EnemyHealth == 0)
                            {
                                while (loopAgain == 0)
                                {
                                    int explosionNum = explosionPicker.Next(explosions.Count);
                                    if (explosions[explosionNum].IsAlive == false)
                                    {
                                        explosions[explosionNum].IsAlive = true;
                                        explosions[explosionNum].Position = new Vector2(deadlyBanjo.EnemyRectangle.X, deadlyBanjo.EnemyRectangle.Y);
                                        loopAgain = 1;
                                    }
                                }

                                deadlyBanjo.EnemyRectangle = new Rectangle(-100, -200, screenWidth / 20, screenHeight / 20); // - 500, - 400
                                deadlyBanjo.IsAlive = false;
                                deadlyBanjo.EnemyHealth = 2;
                                cHUD.Score = cHUD.Score + deadlyBanjo.EnemyScoreValue;
                                enemySpawnCounter = 2;
                            }
                        }
                    }
                }
            }     
        }

        /// <summary>
        /// Picks a random type of banjo to set their boolean "isAlive" to true so it can be drawn and updated
        /// </summary>
        public void LoadEnemies()
        {
            // Picks a random X and Y coordinate at the top of the screen for the banjos to spwan at
            int pickerX = ranX.Next(0, 750);
            int pickerY = ranY.Next(0, 5);
            int loopAgain = 0;
            int pickerEnemy;

            while (loopAgain == 0) // This loop makes sure that a banjo is picked before leaving the method
            {
                if (currentGameState == GameState.HighScore)
                {
                    pickerEnemy = 1; // Only spawns plain banjos
                }
                else
                {
                    pickerEnemy = enemyPicker.Next(1, 8); // Picks a random number from and including 1 to 7
                }
                if (pickerEnemy == 1 || pickerEnemy == 2 || pickerEnemy == 3 || pickerEnemy == 4)
                {

                    int pBanjoTruthCount = 0;
                    int banjoNum = pBanjoPicker.Next(pBanjo.Count); // Picks a random banjo from the plain banjo list
                    if (pBanjo[banjoNum].IsAlive == false) // Checks if the boolean isAlive is set to false
                    {
                        // Moves the banjo rectangle to a position of the random X and Y coordinates that were picked above
                        pBanjo[banjoNum].EnemyRectangle = new Rectangle(pickerX, pickerY, 40, 80);
                        pBanjo[banjoNum].IsAlive = true; // Sets the bool to true to be able to update and draw it on the screen
                        loopAgain = 1;
                    }
                    else
                    {
                        // Checks if all the plainBanjos booleans "isAive" are all set to true and are alive on the screen
                        foreach (cEnemy plainBanjo in pBanjo)
                        {
                            if (plainBanjo.IsAlive == true)
                            {
                                pBanjoTruthCount = pBanjoTruthCount + 1;
                            }
                        }

                        if (pBanjoTruthCount == pBanjo.Count)
                        {
                            loopAgain = 1;
                        }
                    }
                }

                if (pickerEnemy == 5 || pickerEnemy == 6)
                {
                    int hBanjoTruthCount = 0;
                    int banjoNum = hBanjoPicker.Next(hBanjo.Count);
                    if (hBanjo[banjoNum].IsAlive == false)
                    {
                        hBanjo[banjoNum].EnemyRectangle = new Rectangle(pickerX, pickerY, 40, 80);
                        hBanjo[banjoNum].IsAlive = true;
                        loopAgain = 1;
                    }
                    else
                    {
                        foreach (cEnemy hunterBanjo in hBanjo)
                        {
                            if (hunterBanjo.IsAlive == true)
                            {
                                hBanjoTruthCount = hBanjoTruthCount + 1;
                            }
                        }

                        if (hBanjoTruthCount == hBanjo.Count)
                        {
                            // All plain banjos are alive
                            loopAgain = 1;
                        }
                    }
                }

                if (pickerEnemy == 7)
                {
                    int dBanjoTruthCount = 0;
                    int banjoNum = dBanjoPicker.Next(dBanjo.Count);
                    if (dBanjo[banjoNum].IsAlive == false)
                    {
                        dBanjo[banjoNum].EnemyRectangle = new Rectangle(pickerX, pickerY, 40, 80);
                        dBanjo[banjoNum].IsAlive = true;
                        //Picks a random bullet from the deadly banjo bullet list and draws it at the position of the deadlybanjo when it was spawned 
                        int bulletNum = bulletPicker.Next(dBanjoBullets.Count);
                        if (dBanjoBullets[bulletNum].IsAlive == false)
                        {
                            dBanjoBullets[bulletNum].BulletRectangle = new Rectangle(dBanjo[banjoNum].EnemyRectangle.X, dBanjo[banjoNum].EnemyRectangle.Y + 70, 50, 30);
                            dBanjoBullets[bulletNum].IsAlive = true;
                            loopAgain = 1;
                        }
                    }
                    else
                    {
                        foreach (cEnemy deadlyBanjo in dBanjo)
                        {
                            if (deadlyBanjo.IsAlive == true)
                            {
                                dBanjoTruthCount = dBanjoTruthCount + 1;
                            }
                        }

                        if (dBanjoTruthCount == dBanjo.Count)
                        {
                            loopAgain = 1;
                        }
                    }
                }
            }

        }
            
    }
}
