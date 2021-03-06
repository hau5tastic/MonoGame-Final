﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame_Dynamics_Final_Project.Sprites;
using MonoGame_Dynamics_Final_Project.Weapons;
#endregion

namespace MonoGame_Dynamics_Final_Project
{
    public enum GameState
    {
        LoadWave,
        StartMenu,
        Play,
        Pause,
        GameOver,
        Exit
    }

    public class Game1 : Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Independent Resolution and Camera
        private ResolutionRenderer _irr;
        private const int VIRTUAL_RESOLUTION_WIDTH = 2160;
        private const int VIRTUAL_RESOLUTION_HEIGHT = 1440;
        Rectangle VirtualSize = new Rectangle(0, 0, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT);
        private Camera2D _camera;


        GameState gameState = GameState.StartMenu;
        PowerUps Powerups = PowerUps.Null;

        public static Random random;

        //AUDIO 
        AudioManager audioManager;
        bool isThrusting = false;
        bool isThrustingDown = false;
        bool ifFiring = false;

        bool songSwap = false;

        // background
        int windowWidth, windowHeight;
        Texture2D[] background;
        Texture2D[] background2;
        ScrollingBackground myBackground;
        ScrollingBackground myBGtwo;


        //POWER UP IMGs
        Texture2D AtkSpdUp;
        Texture2D MoveSpdUp;
        Texture2D HPUp;
        Texture2D AtkSpdDown;
        Texture2D MoveSpdDown;
        Texture2D HPDown;
        Texture2D GravAmmo;
        Texture2D RocketAmmo;
        Texture2D HomingAmmo;


        // menu
        Texture2D startMenuScreen;
        Menu menuScreen;
        Menu gameOver;
        SpriteFont menuFont;
        Color customColor;
        float score = 0;
        float timer = 0.0f;
        bool playGame;
        bool swapScreen = false;
        //Buttons;

        // player
        Texture2D playerTexture;
        Texture2D playerMove;
        Texture2D playerRight;
        Texture2D playerLeft;
        Texture2D playerRightTurn;
        Texture2D playerLeftTurn;
        Texture2D health;
        Rectangle healthRect;
        Texture2D xp;
        Rectangle xpRect;
        Player playerShip;
        //Player follower;
        // rail turret
        Texture2D turretImage;

        // enemies
        int currentWave;
        List<Enemy> Enemywave = new List<Enemy>();
        List<Enemy> tempWave = new List<Enemy>();
        List<PowerUp> powerUpList = new List<PowerUp>();
        List<Score> DisplayScorePos = new List<Score>();
        List<ProgressUI> DisplayProgress = new List<ProgressUI>();

        //PowerUps
        double spawnChance = 0.2;

        // Vector2 gravityForce = new Vector2(0.0f, 150.0f);
        Vector2 offset = new Vector2(500, 500);

        // input
        KeyboardState oldState;
        int animationResetSwitchU;
        int animationResetSwitchL;
        int animationResetSwitchR;

        //Particle Effects
        ParticleEngine Thruster1;
        ParticleEngine Thruster2;
        List<Texture2D> Thrustertextures;

        List<ParticleEngine> StingrayParticles = new List<ParticleEngine>();
        List<Texture2D> StingrayTextures;
        List<ParticleEngine> Stingray2Particles = new List<ParticleEngine>();
        List<Texture2D> Stingray2Textures;

        int EnemyParticleCounter = 0;
        int EnemyParticleCounter2 = 0;

        List<ParticleEngine> DestructionParticles = new List<ParticleEngine>();
        List<Texture2D> DestructionTextures = new List<Texture2D>();
        List<int> DestructionRadiusCounters = new List<int>();
        List<int> DestructionAngleCounters = new List<int>();
        List<Vector2> DestructionEmmision = new List<Vector2>();

        List<ParticleEngine> AftershockParticles = new List<ParticleEngine>();
        List<Texture2D> AftershockTextures = new List<Texture2D>();
        List<int> AftershockRadiusCounters = new List<int>();
        List<int> AftershockAngleCounters = new List<int>();
        List<Vector2> AftershockEmmision = new List<Vector2>();

        List<ParticleEngine> PowerupParticles = new List<ParticleEngine>();
        List<Texture2D> PowerupTextures = new List<Texture2D>();
        List<int> PowerupRadiusCounters = new List<int>();
        List<int> PowerupAngleCounters = new List<int>();
        List<Vector2> PowerupEmmision = new List<Vector2>();
        int powFlip = 2;

        Random randomnumber = new Random();

        public int shakeCounter = 0;
        public bool shakeSwitch = false;
        public bool shakeReset = false;
        Vector2 originalCameraPosition;

        float elapsedMS;
        #endregion

        #region graphicsDevice/Resolution Stuff
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.Window.IsBorderless = true;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            //set virtual screen resolution
            _irr = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _camera = new Camera2D(_irr) { MaxZoom = 10f, MinZoom = .4f, Zoom = 1.0f };
            _camera.SetPosition(new Vector2(VIRTUAL_RESOLUTION_WIDTH / 2, VIRTUAL_RESOLUTION_HEIGHT / 2));
            _camera.RecalculateTransformationMatrices();
            originalCameraPosition = _camera.Position;

            base.Initialize();
        }
        #endregion

        #region LoadContent
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            random = new Random();
            playGame = false;

            // Menu / UI Items
            string[] menuItems = { "Launch Ship", "How to Play", "Exit Cockpit" };
            string[] gameOverItems = { "", "Main Menu", "Quit" };
            menuFont = Content.Load<SpriteFont>("Fonts/titleFont");
            menuScreen = new Menu(GraphicsDevice, Content, menuFont, menuItems);
            gameOver = new Menu(GraphicsDevice, Content, menuFont, gameOverItems);
            startMenuScreen = Content.Load<Texture2D>("Images/Backgrounds/MenuTwo");
            customColor.A = 1;
            customColor.R = 200;
            customColor.G = 0;
            customColor.B = 255;
            health = Content.Load<Texture2D>("Images/playerhealth");
            xp = Content.Load<Texture2D>("Images/expereince bar");

            //PARTICLES
            // Thruster Particles
            Thrustertextures = new List<Texture2D>();
            Thrustertextures.Add(Content.Load<Texture2D>("Images/Particles/smokepoof"));
            Thrustertextures.Add(Content.Load<Texture2D>("Images/Particles/poofparticle"));
            Thruster1 = new ParticleEngine(Thrustertextures, new Vector2(400, 240));
            Thruster2 = new ParticleEngine(Thrustertextures, new Vector2(400, 240));

            // StingRay Particles
            StingrayTextures = new List<Texture2D>();
            StingrayTextures.Add(Content.Load<Texture2D>("Images/Particles/starpoof1"));
            StingrayTextures.Add(Content.Load<Texture2D>("Images/Particles/starpoof2"));
            Stingray2Textures = new List<Texture2D>();
            Stingray2Textures.Add(Content.Load<Texture2D>("Images/Particles/xdiamond"));
            Stingray2Textures.Add(Content.Load<Texture2D>("Images/Particles/poweruparticle2"));

            //Destruction Particles
            DestructionTextures.Add(Content.Load<Texture2D>("Images/Particles/meow"));
            DestructionTextures.Add(Content.Load<Texture2D>("Images/Particles/meow1"));
            DestructionTextures.Add(Content.Load<Texture2D>("Images/Particles/meow2"));
            DestructionTextures.Add(Content.Load<Texture2D>("Images/Particles/meow3"));
            DestructionTextures.Add(Content.Load<Texture2D>("Images/Particles/meow4"));

            //Aftershock Particles
            AftershockTextures.Add(Content.Load<Texture2D>("Images/Particles/xdiamond"));
            AftershockTextures.Add(Content.Load<Texture2D>("Images/Particles/exhaust"));
            AftershockTextures.Add(Content.Load<Texture2D>("Images/Particles/starpoo"));
            AftershockTextures.Add(Content.Load<Texture2D>("Images/Particles/pulse"));
            
            //PowerUp Particles
            PowerupTextures.Add(Content.Load<Texture2D>("Images/Particles/poweruparticle1"));
            PowerupTextures.Add(Content.Load<Texture2D>("Images/Particles/poweruparticle2"));
            PowerupTextures.Add(Content.Load<Texture2D>("Images/Particles/poweruparticle3"));
            PowerupTextures.Add(Content.Load<Texture2D>("Images/Particles/poweruparticle4"));

            //Loading audio
            audioManager = new AudioManager();
            audioManager.Initialize(Content);
            audioManager.Play("menu song");

            //PwrUpTextures
            AtkSpdUp = Content.Load<Texture2D>("Images/PowerUps/AtkSpdUp");
            MoveSpdUp = Content.Load<Texture2D>("Images/PowerUps/MoveSpdUp");
            HPUp = Content.Load<Texture2D>("Images/PowerUps/HPUp");
            AtkSpdDown = Content.Load<Texture2D>("Images/PowerUps/AtkSpdDown");
            MoveSpdDown = Content.Load<Texture2D>("Images/PowerUps/MoveSpdDown");
            HPDown = Content.Load<Texture2D>("Images/PowerUps/HPDown");
            GravAmmo = Content.Load<Texture2D>("Images/PowerUps/GravWell");
            RocketAmmo = Content.Load<Texture2D>("Images/PowerUps/Rockets");
            HomingAmmo = Content.Load<Texture2D>("Images/PowerUps/Homing");

            // background
            myBackground = new ScrollingBackground();
            myBGtwo = new ScrollingBackground();
            background = new Texture2D[3];
            background2 = new Texture2D[1];
            for (int i = 0; i < background.Length; i++)
            {
                background[i] = Content.Load<Texture2D>("Images/Backgrounds/universe" + (i).ToString());
            }
            background2[0] = Content.Load<Texture2D>("Images/Backgrounds/universe-background");

            myBackground.Load(VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, background, background.Length, 0.5f); // change float to change animation speed           
            myBGtwo.Load(VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, background2, background2.Length, 0.5f); // change float to change animation speed 

            // player sprites
            playerTexture = Content.Load<Texture2D>("Images/Animations/Commandunit-idle");
            playerMove = Content.Load<Texture2D>("Images/Animations/Commandunit-move");
            playerRight = Content.Load<Texture2D>("Images/Animations/Commandunit-right");
            playerLeft = Content.Load<Texture2D>("Images/Animations/Commandunit-left");
            playerRightTurn = Content.Load<Texture2D>("Images/Animations/Commandunit-Turn");
            playerLeftTurn = Content.Load<Texture2D>("Images/Animations/Commandunit-Turn-left");
            turretImage = Content.Load<Texture2D>("Images/Animations/Plasma-Repeater");

            playerShip = new Player(64, 70, playerTexture, turretImage,
                new Vector2(VirtualSize.Width / 2, VirtualSize.Height - 70),
                new Vector2(10, 10),
                true,
                1.0f
                );

            //follower = new Follower(32, 32, Content, playerShip, new Vector2(0, playerShip.frameHeight + 20), 1.0f, true);

            LoadWave();
            currentWave = 1;
        }

        protected override void UnloadContent()
        {
        }
        #endregion

        #region Upate Methods
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Backgrounds
            float BGelapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            myBackground.Update(gameTime, elapsedMS + BGelapsed * 400);
            myBGtwo.Update(gameTime, elapsedMS + BGelapsed * 200);
            UpdateInput(gameTime);

            // Load waves, increase difficulty, levels, score
            if (gameState == GameState.LoadWave)
            {
                LoadWave();
                currentWave++;
                foreach (Enemy enemy in Enemywave)
                {
                    enemy.Level = currentWave;
                    enemy.Health = (enemy.Health * (enemy.Level / 2));
                    enemy.Damage = (enemy.Damage * (enemy.Level / 2));
                    enemy.score = enemy.score * enemy.Level;
                }
                gameState = GameState.Play;
                DisplayProgress.Add(new ProgressUI(new Vector2(VirtualSize.Width / 2, VirtualSize.Height / 2), currentWave, menuFont, "Wave:"));
            }

            // Main Update Method
            if (gameState == GameState.Play)
            {
                elapsedMS += (float)gameTime.ElapsedGameTime.Milliseconds / 1000000000.0f;

                if (Enemywave.Count == 0)
                {
                    playerShip.Secondary.Clear(); // get rid of existing gravity wells
                    gameState = GameState.LoadWave;
                }

                if (songSwap)
                {
                    audioManager.Play("Catalysm Song");
                    songSwap = false;
                }

                // UI
                playGame = true;
                healthRect = new Rectangle(100, GraphicsDevice.Viewport.Height - 50, (int)playerShip.Health * (GraphicsDevice.Viewport.Width - 200) / (int)playerShip.MaxHealth, health.Height);
                xpRect = new Rectangle(100, GraphicsDevice.Viewport.Height - 28, (int)playerShip.Experience * (GraphicsDevice.Viewport.Width - 200) / (int)playerShip.ExperienceToNextLevel, xp.Height / 16);
                for (int i = 0; i < DisplayProgress.Count; i++)
                {
                    if (DisplayProgress[i].progressType == "Wave:")
                    {
                        DisplayProgress[i].Update(gameTime);
                    }
                    else
                    {
                        DisplayProgress[i].Update(new Vector2(playerShip.Position.X, playerShip.Position.Y + (playerShip.SpriteOrigin.Y * 2)), gameTime);
                    }
                    if (!DisplayProgress[i].alive)
                    {
                        DisplayProgress.RemoveAt(i);
                    }

                }

                // updating scroll speed
                float elapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // player update
                if (playerShip.Alive && playGame == true)
                {
                    playerShip.Update(gameTime, VirtualSize, Enemywave);
                    //follower.Update(playerShip, gameTime);

                    UpdateEnemyShots();

                    Thruster1.EmitterLocation = playerShip.Position + new Vector2(15, playerShip.frameHeight - 30);
                    Thruster2.EmitterLocation = playerShip.Position + new Vector2(-15, playerShip.frameHeight - 30);
                }

                Thruster1.Update(playerShip.Alive, playerShip.Velocity, 100f, Color.SlateGray, 1);
                Thruster2.Update(playerShip.Alive, playerShip.Velocity, 100f, Color.SlateGray, 1);

                // enemy updates
                foreach (Enemy enemy in Enemywave)
                {
                    enemy.Update(gameTime, VirtualSize);

                    if (enemy.enemyType == "stingRay" && playGame == true)
                    {
                        enemy.Update(Content, gameTime, playerShip);

                        if (EnemyParticleCounter < StingrayParticles.Count)
                        {
                            StingrayParticles[EnemyParticleCounter].EmitterLocation = enemy.Position + new Vector2(Convert.ToSingle(Math.Cos(enemy.rotation) * enemy.frameWidth / 4), Convert.ToSingle(Math.Sin(enemy.rotation) * enemy.frameWidth / 4));
                            StingrayParticles[EnemyParticleCounter].Update(enemy.Alive, enemy.Velocity, 9f, Color.Plum, 1);
                            EnemyParticleCounter++;
                        }
                        else
                        {
                            EnemyParticleCounter = 0;
                        }
                    }
                    if (enemy.enemyType == "stingRay2" && playGame == true)
                    {
                        enemy.Update(Content, gameTime, playerShip);

                        if (EnemyParticleCounter2 < Stingray2Particles.Count)
                        {
                            Stingray2Particles[EnemyParticleCounter2].EmitterLocation = enemy.Position + new Vector2(Convert.ToSingle(Math.Cos(enemy.rotation) * enemy.frameWidth / 4), Convert.ToSingle(Math.Sin(enemy.rotation) * enemy.frameWidth / 4));
                            Stingray2Particles[EnemyParticleCounter2].Update(enemy.Alive, enemy.Velocity, 9f, Color.Blue, 1);
                            EnemyParticleCounter2++;
                        }
                        else
                        {
                            EnemyParticleCounter2 = 0;
                        }
                    }
                    if (enemy.enemyType == "voidVulture" && playGame == true)
                    {
                        enemy.Update(Content, gameTime, playerShip);
                    }
                    if (enemy.enemyType == "voidAngel" && playGame == true)
                    {
                        enemy.Update(Content, gameTime, playerShip);
                    }
                }

                // tests for collision of primary shots against enemy (one for each turret)
                checkPrimaryCollisions(1);
                checkPrimaryCollisions(2);

                // tests for collision of secondary shots against enemy
                if (Enemywave.Count > 0)
                {                    
                    for (int i = 0; i < Enemywave.Count; i++)
                    {
                        int collide = Enemywave[i].CollisionShot(playerShip.Secondary);
                        if (collide != -1)
                        {
                            if (playerShip.SecondaryType != "gravityWell")
                            {
                                audioManager.PlaySoundEffect("hit");
                                playerShip.CurrentSecondaryAmmo++;
                                playerShip.Secondary.RemoveAt(collide);
                                if (random.NextDouble() <= spawnChance)
                                {
                                    SpawnPowerUp(Enemywave[i].Position);
                                    PowerupParticles.Add(new ParticleEngine(PowerupTextures, new Vector2(400, 240)));
                                    PowerupRadiusCounters.Add(10);
                                    PowerupAngleCounters.Add(0);
                                    PowerupEmmision.Add(Enemywave[i].Position);
                                }
                                Enemywave[i].Health = 0;
                                playerShip.Experience += Enemywave[i].score;
                                DisplayScorePos.Add(new Score(Enemywave[i].Position, Enemywave[i].score, menuFont));
                                
                            }
                            
                            if (Enemywave[i].Health <= 0f)
                            {
                                score += Enemywave[i].score;
                                playerShip.Experience += Enemywave[i].score;

                                if (Enemywave[i].enemyType == "stingRay")
                                {
                                    audioManager.PlaySoundEffect("enemy dead2");
                                    StingrayParticles.RemoveAt(StingrayParticles.Count - 1);
                                }
                                if (Enemywave[i].enemyType == "stingRay2")
                                {
                                    audioManager.PlaySoundEffect("enemy dead2");
                                    Stingray2Particles.RemoveAt(Stingray2Particles.Count - 1);
                                }
                                if (Enemywave[i].enemyType == "voidVulture")
                                {
                                    audioManager.PlaySoundEffect("enemy dead");
                                    shakeSwitch = true;

                                    DestructionParticles.Add(new ParticleEngine(DestructionTextures, new Vector2(400, 240)));
                                    DestructionRadiusCounters.Add(10);
                                    DestructionAngleCounters.Add(0);
                                    DestructionEmmision.Add(Enemywave[i].Position);

                                    AftershockParticles.Add(new ParticleEngine(AftershockTextures, new Vector2(400, 240)));
                                    AftershockRadiusCounters.Add(10);
                                    AftershockAngleCounters.Add(0);
                                    AftershockEmmision.Add(Enemywave[i].Position);
                                }
                                if (Enemywave[i].enemyType == "voidAngel")
                                {
                                    audioManager.PlaySoundEffect("enemy dead2");
                                }
                            }
                            if (playerShip.SecondaryType != "gravityWell")
                            {
                               Enemywave.RemoveAt(i);
                            }
                        }
                    }
                }

                // Player Level Update
                if (playerShip.Experience >= playerShip.ExperienceToNextLevel)
                {
                    playerShip.Level++;
                    playerShip.MaxHealth += (int)playerShip.Level * 10;
                    audioManager.PlaySoundEffect("Spawn pUp");
                    playerShip.ExperienceToNextLevel *= 1.25f;
                    playerShip.Experience = 0;
                    DisplayProgress.Add(new ProgressUI(playerShip.Position, playerShip.Level, menuFont, "Level:"));
                }

                //Destrcution Update
                for (int i = 0; i < DestructionParticles.Count; i++)
                {
                    DestructionParticles[i].EmitterLocation = DestructionEmmision[i];
                    DestructionEmmision[i] += new Vector2(Convert.ToSingle(Math.Cos(DestructionAngleCounters[i]) * DestructionRadiusCounters[i]), Convert.ToSingle(Math.Sin(DestructionAngleCounters[i]) * DestructionRadiusCounters[i]));
                    DestructionRadiusCounters[i] += 10;
                    DestructionAngleCounters[i] += 10;

                    DestructionParticles[i].Update((DestructionRadiusCounters[i] < 200), new Vector2(10, 10), 0f, Color.White, 20);
                    DestructionParticles[i].Update((DestructionRadiusCounters[i] < 400 && DestructionRadiusCounters[i] >= 200), new Vector2(10, 10), 0f, Color.Yellow, 40);
                    DestructionParticles[i].Update((DestructionRadiusCounters[i] < 600 && DestructionRadiusCounters[i] >= 400), new Vector2(10, 10), 0f, Color.Orange, 60);
                    DestructionParticles[i].Update((DestructionRadiusCounters[i] < 800 && DestructionRadiusCounters[i] >= 600), new Vector2(10, 10), 0f, Color.Red, 80);
                    DestructionParticles[i].Update((DestructionRadiusCounters[i] < 1000 && DestructionRadiusCounters[i] >= 800), new Vector2(10, 10), 0f, Color.DarkRed, 100);

                    if (DestructionRadiusCounters[i] >= 200)
                    {
                        for (int f = 0; f < AftershockParticles.Count; f++)
                        {
                            AftershockParticles[f].EmitterLocation = AftershockEmmision[f];
                            AftershockEmmision[f] += new Vector2(Convert.ToSingle(Math.Cos(AftershockAngleCounters[f]) * AftershockRadiusCounters[f]), Convert.ToSingle(Math.Sin(AftershockAngleCounters[f]) * AftershockRadiusCounters[f]));
                            AftershockRadiusCounters[f] += 10;
                            AftershockAngleCounters[f] -= 10;

                            AftershockParticles[f].Update((AftershockRadiusCounters[f] <= 200), new Vector2(10, 10), 0f, Color.Purple, 4);
                            AftershockParticles[f].Update((AftershockRadiusCounters[f] <= 400 && AftershockRadiusCounters[f] > 200), new Vector2(10, 10), 0f, Color.Blue, 6);
                            AftershockParticles[f].Update((AftershockRadiusCounters[f] <= 600 && AftershockRadiusCounters[f] > 400), new Vector2(10, 10), 0f, Color.Turquoise, 8);
                            AftershockParticles[f].Update((AftershockRadiusCounters[f] <= 800 && AftershockRadiusCounters[f] > 600), new Vector2(10, 10), 0f, Color.Goldenrod, 10);
                        }
                    }                    
                }

                // Player-Enemy Collision Update
                playerShip.collisionDetected = false;
                float damage = 0.0f;
                for (int i = 0; i < Enemywave.Count; i++)
                {
                    if (playerShip.CollisionSprite(Enemywave[i]))
                    {
                        playerShip.collisionDetected = true;
                        damage += Enemywave[i].Damage;
                        Console.WriteLine("Damage:" + Enemywave[i].Damage);
                    }
                }
                playerShip.Health -= (int)damage;

                // Game Over Man, Game Over
                if (playerShip.Health <= 0.0f)
                {
                    if (!swapScreen)
                    {
                        gameState = GameState.GameOver;
                        swapScreen = true;
                    }
                    playerShip.Alive = false;
                    //follower.Alive = false;
                }
                
                // Gravity Well Special Update
                if (playerShip.ForcePull && playerShip.Secondary.Count > 0)
                {
                    playerShip.Secondary[0].forcePull(gameTime, Enemywave);
                    playerShip.Secondary[0].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

                    if (playerShip.Secondary[0].ElapsedTime > 5.0f)
                    {
                        audioManager.PlaySoundEffect("gravity well");
                        playerShip.ForcePull = false;
                        playerShip.Secondary.RemoveAt(0);
                    }
                }

                // Powerup updates
                if (powerUpList.Count > 0)
                {
                    for (int i = 0; i < powerUpList.Count; i++)
                    {
                        powerUpList[i].Update(gameTime, VirtualSize);

                        PowerupEmmision[i] = powerUpList[i].Position;
                        PowerupEmmision[i] += new Vector2(Convert.ToSingle(Math.Cos(PowerupAngleCounters[i]) * PowerupRadiusCounters[i]), Convert.ToSingle(Math.Sin(PowerupAngleCounters[i]) * PowerupRadiusCounters[i]));
                        PowerupParticles[i].EmitterLocation = PowerupEmmision[i];
                        PowerupRadiusCounters[i] += powFlip;
                        PowerupAngleCounters[i] += 1;

                        if (PowerupRadiusCounters[i] >= 20)
                        {
                            powFlip *= -1;
                        }
                        if (PowerupRadiusCounters[i] <= 0)
                        {
                            powFlip *= -1;
                        }

                        PowerupParticles[i].Update(powerUpList[i].Alive, new Vector2(10, 10), 0f, new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)), 1);

                        if (powerUpList[i].removeFromScreen)
                        {
                            powerUpList[i].Alive = false;
                            PowerupParticles.RemoveAt(i);
                            PowerupRadiusCounters.RemoveAt(i);
                            PowerupEmmision.RemoveAt(i);
                            PowerupAngleCounters.RemoveAt(i);
                        }
                    }
                }

                for (int i = powerUpList.Count - 1; i >= 0; i--)
                {
                    if (powerUpList[i].removeFromScreen)
                    {
                        powerUpList.RemoveAt(i);
                    }
                    else if (CheckForPowerUps(playerShip.CollisionRectangle, powerUpList[i].CollisionRectangle))
                    {
                        audioManager.PlaySoundEffect("Get pUp");
                        powerUpList[i].ActivatePowerUp(Powerups, playerShip);
                        powerUpList.RemoveAt(i);
                    }
                }

                // Score Display Updates
                for (int i = 0; i < DisplayScorePos.Count; i++)
                {
                    DisplayScorePos[i].Update(gameTime);

                    if (!DisplayScorePos[i].alive)
                    {
                        DisplayScorePos.RemoveAt(i);
                    }
                }

                // Viewport Camera Update
                if(shakeSwitch == false) 
                { 
                    _camera.Position = originalCameraPosition;  
                }
                if (shakeSwitch == true)
                {
                    if (shakeReset == false)
                    {
                        _camera.Move(new Vector2(random.Next(-50, 50), random.Next(-50, 50)));
                        shakeReset = true;
                    }
                    else
                    {
                        _camera.Position = originalCameraPosition;
                        shakeReset = false;
                    }
                shakeCounter++;
                    if (shakeCounter == 50) { 
                        shakeSwitch = false; shakeCounter = 0; 
                    }
                }
                _camera.Update(gameTime);

                base.Update(gameTime);
            }
        }

        //Update Input Method **************************************************************************************************************
        public void UpdateInput(GameTime gameTime)
        {
            bool keyPressed = false;
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gameState == GameState.StartMenu)
            {
                playGame = false;
                Rectangle exit = new Rectangle(572, 384, 100, 50);
                Rectangle start = new Rectangle(572, 274, 200, 50);
                Rectangle instr = new Rectangle(572, 344, 150, 50);
                menuScreen.Update();

                if (menuScreen.ItemSelected == 3)
                {
                    this.Exit();
                }
                else if (menuScreen.ItemSelected == 1)
                {
                    gameState = GameState.Play;
                    songSwap = true;
                }
            }
            if(gameState == GameState.GameOver)
            {
                playGame = false;
                gameOver.Update();
                if (gameOver.ItemSelected == 2)
                {
                    gameState = GameState.StartMenu;
                    RefreshGameInfo();
                }
                if (gameOver.ItemSelected == 3)
                {
                    this.Exit();
                }
            }
            if (gameState == GameState.Play)
            {
                playGame = true;
                if (keyState.IsKeyDown(Keys.Up)
                   || keyState.IsKeyDown(Keys.W)
                   || gamePadState.DPad.Up == ButtonState.Pressed
                   || gamePadState.ThumbSticks.Left.Y > 0)
                {
                    playerShip.Up();
                    if (!isThrusting)
                    {
                        audioManager.setThrustLooping(true);
                        audioManager.PlaySoundEffect("thrust");
                        isThrusting = true;
                    }
                    keyPressed = true;

                    if (animationResetSwitchU == 0 && animationResetSwitchR == 0 && animationResetSwitchL == 0)
                    {
                        playerShip.resetAnimation();
                        playerShip.TextureImage = playerMove;
                        animationResetSwitchU++;
                        keyPressed = true;
                    }
                }
                else if (keyState.IsKeyUp(Keys.Up)
                        || keyState.IsKeyUp(Keys.W)
                        || gamePadState.DPad.Up == ButtonState.Released
                        || gamePadState.ThumbSticks.Left.Y == 0)
                {
                    if (isThrusting && audioManager.isThrustLooping())
                    {
                        audioManager.setThrustLooping(false);
                        audioManager.StopThrust();
                        isThrusting = false;
                    }
                    if (animationResetSwitchU > 0)
                    {
                        playerShip.resetAnimation();
                        animationResetSwitchU = 0;
                        playerShip.TextureImage = playerTexture;
                    }
                }
                if (keyState.IsKeyDown(Keys.Down)
                   || keyState.IsKeyDown(Keys.S)
                   || gamePadState.DPad.Down == ButtonState.Pressed
                   || gamePadState.ThumbSticks.Left.Y < -0.5f)
                {
                    playerShip.Down();
                    if (!isThrustingDown)
                    {
                        audioManager.setThrustLooping(true);
                        audioManager.PlaySoundEffect("thrust");
                        isThrustingDown = true;
                    }
                    keyPressed = true;
                }
                else if (keyState.IsKeyUp(Keys.Down)
                        || keyState.IsKeyUp(Keys.S)
                        || gamePadState.DPad.Down == ButtonState.Released
                        || gamePadState.ThumbSticks.Left.Y == 0)
                {
                    if (isThrustingDown && audioManager.isThrustLooping())
                    {
                        audioManager.setThrustLooping(false);
                        audioManager.StopThrust();
                        isThrustingDown = false;
                    }
                }

                if (keyState.IsKeyDown(Keys.Left)
                   || gamePadState.DPad.Left == ButtonState.Pressed
                   || gamePadState.ThumbSticks.Left.X < -0.5f)
                {
                    playerShip.Left();
                    playerShip.isTurning = true;
                    playerShip.turnOrientation = -1;
                    keyPressed = true;

                    if (animationResetSwitchL == 0)
                    {
                        playerShip.resetAnimation();
                        animationResetSwitchL++;
                        playerShip.framesOverride = 6;
                        playerShip.TextureImage = playerLeftTurn;
                    }
                    if (animationResetSwitchL == 1)
                    {

                        if (playerShip.frameIndex > 6) { animationResetSwitchL++; }
                    }
                    if (animationResetSwitchL == 2)
                    {
                        playerShip.resetAnimation();
                        playerShip.framesOverride = 0;
                        playerShip.TextureImage = playerLeft;
                        animationResetSwitchL++;
                    }
                }
                else if (keyState.IsKeyUp(Keys.Left)
                        || gamePadState.DPad.Left == ButtonState.Released
                        || gamePadState.ThumbSticks.Left.X == 0)
                {
                    playerShip.isTurning = false;
                    if (animationResetSwitchL > 0)
                    {
                        playerShip.resetAnimation();
                        playerShip.framesOverride = 0;
                        animationResetSwitchL = 0;
                        animationResetSwitchR = 0;
                        playerShip.TextureImage = playerTexture;
                    }
                }
                if (keyState.IsKeyDown(Keys.Right)
                   || gamePadState.DPad.Right == ButtonState.Pressed
                   || gamePadState.ThumbSticks.Left.X > 0.5f)
                {
                    playerShip.Right();
                    keyPressed = true;
                    playerShip.isTurning = true;
                    playerShip.turnOrientation = 1;

                    if (animationResetSwitchR == 0)
                    {
                        playerShip.resetAnimation();
                        animationResetSwitchR++;
                        playerShip.framesOverride = 6;
                        playerShip.TextureImage = playerRightTurn;
                    }
                    if (animationResetSwitchR == 1)
                    {
                        if (playerShip.frameIndex > 6) { animationResetSwitchR++; }
                    }
                    if (animationResetSwitchR == 2)
                    {
                        playerShip.resetAnimation();
                        playerShip.framesOverride = 0;
                        playerShip.TextureImage = playerRight;
                        animationResetSwitchR++;
                    }
                }
                else if (keyState.IsKeyUp(Keys.Right)
                        || gamePadState.DPad.Right == ButtonState.Released
                        || gamePadState.ThumbSticks.Left.X == 0)
                {
                    playerShip.isTurning = false;
                    if (animationResetSwitchR > 0)
                    {
                        playerShip.resetAnimation();
                        playerShip.framesOverride = 0;
                        animationResetSwitchR = 0;
                        animationResetSwitchL = 0;
                        playerShip.TextureImage = playerTexture;
                    }

                }

                // turrets
                if ((keyState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Right.X > 0.5) && playerShip.PrimaryType == "rail")
                {
                    playerShip.RailLeft.rotateTurret(-1 * playerShip.RailLeft.orientation);
                    playerShip.RailRight.rotateTurret(-1 * playerShip.RailRight.orientation);
                }
                else if ((keyState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Right.X < -0.5) && playerShip.PrimaryType == "rail")
                {
                    playerShip.RailLeft.rotateTurret(1 * playerShip.RailLeft.orientation);
                    playerShip.RailRight.rotateTurret(1 * playerShip.RailRight.orientation);
                }

                // Primary Weapon
                if (((oldState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space))|| gamePadState.Triggers.Right > 0.1) && playerShip.PrimaryType == "rail")
                {
                    playerShip.HasShotPrim = true;
                    if (playerShip.HasShotPrim)
                    {
                        audioManager.PlaySoundEffect("shot");
                        playerShip.shootPrimary(Content, gameTime);
                    }
                    else
                    {
                        playerShip.HasShotPrim = false;
                    }
                }
                else if ((keyState.IsKeyDown(Keys.Space) || gamePadState.Triggers.Right > 0.1) && playerShip.PrimaryType == "laser")
                {
                    playerShip.HasShotPrim = true;
                    if (playerShip.HasShotPrim)
                    {
                        if (!ifFiring)
                        {
                            audioManager.PlaySoundEffect("shot");
                            audioManager.setLaserLooping(true);
                            ifFiring = true;
                        }
                        playerShip.shootPrimary(Content, gameTime);
                    }
                    else
                    {
                        playerShip.HasShotPrim = false;
                    }               
                }
                else if (keyState.IsKeyUp(Keys.Space) && playerShip.PrimaryType == "laser")
                {
                    if (ifFiring && audioManager.isLaserLooping())
                    {
                        ifFiring = false;
                        audioManager.setLaserLooping(false);
                        audioManager.StopLaser();
                    }
                }
                // Secondary Weapon
                if ((oldState.IsKeyUp(Keys.B) && keyState.IsKeyDown(Keys.B)) || gamePadState.IsButtonDown(Buttons.A))
                {
                    if (playerShip.HasShot && playerShip.Secondary.Count > 0 && playerShip.SecondaryType == "gravityWell")
                    {
                        audioManager.PlaySoundEffect("rocket");
                        playerShip.HasShot = false;
                        playerShip.ForcePull = true;
                        playerShip.Secondary[0].ElapsedTime = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                    }
                    else if (!playerShip.HasShot)
                    {
                        audioManager.PlaySoundEffect("shot");
                        playerShip.shootSecondary(Content);
                    }
                }
                if (keyState.IsKeyDown(Keys.D1))
                {
                    playerShip.setWeapon("rail", 10);
                }
                if (keyState.IsKeyDown(Keys.K) || gamePadState.IsButtonDown(Buttons.Start)) // kill switch
                {
                    playerShip.Health = 0;
                }
                if (keyState.IsKeyDown(Keys.D2))
                {
                    playerShip.setWeapon("laser", 10);
                    playerShip.RailLeft.rotation = 0;
                    playerShip.RailRight.rotation = 0;
                }
                if(gamePadState.IsButtonDown(Buttons.DPadUp))
                {
                    if(playerShip.PrimaryType == "laser")
                    {
                        playerShip.setWeapon("rail", 10);
                    }
                    if(playerShip.PrimaryType == "rail")
                    {
                        playerShip.setWeapon("laser", 10);
                        playerShip.RailLeft.rotation = 0;
                        playerShip.RailRight.rotation = 0;
                    }
                }
                if (keyState.IsKeyDown(Keys.D3) || gamePadState.IsButtonDown(Buttons.DPadDown))
                {
                    //playerShip.SecondaryType = "gravityWell";
                    playerShip.setWeapon("gravityWell", 1);
                }
                if (keyState.IsKeyDown(Keys.D4) || gamePadState.IsButtonDown(Buttons.DPadLeft))
                {
                    //playerShip.SecondaryType = "helixMissile";
                    playerShip.setWeapon("helixMissile", 10);
                }
                if (keyState.IsKeyDown(Keys.D5) || gamePadState.IsButtonDown(Buttons.DPadRight))
                {
                    //playerShip.SecondaryType = "homingMissile";
                    playerShip.setWeapon("homingMissile", 10);
                }
                if (keyState.IsKeyDown(Keys.P))
                {
                    gameState = GameState.Pause;
                    playGame = false;
                }

                if (!keyPressed)
                {
                    playerShip.Idle();
                }

                oldState = keyState;
            }
        }
        #endregion

        #region Draw Methods
        private void drawRect(Rectangle coords, Color color)
        {
            var rect = new Texture2D(GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                //Menu***********************************************************************************************************************
                case GameState.StartMenu:

                    _irr.Draw();
                    spriteBatch.BeginCamera(_camera, BlendState.AlphaBlend);
                    myBGtwo.Draw(spriteBatch);
                    myBackground.Draw(spriteBatch);
                    spriteBatch.End();

                    _irr.SetupFullViewport();
                    spriteBatch.Begin();
                    spriteBatch.DrawString(menuFont, "FINAL CATACLYSM", new Vector2(GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 15), customColor, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(startMenuScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    menuScreen.Draw(spriteBatch);
                    spriteBatch.End();
                    _irr.SetupVirtualScreenViewport();
                    break;

                //Play***********************************************************************************************************************
                case GameState.Play:
                    //Clear screen
                    GraphicsDevice.Clear(Color.Black);

                    //IRR
                    _irr.Draw();

                    //Begin Drawing Gameplay Stuff!!
                    spriteBatch.BeginCamera(_camera, BlendState.AlphaBlend);

                    //Draw Background
                    myBGtwo.Draw(spriteBatch);
                    myBackground.Draw(spriteBatch);

                    //Draw Stingray Particles
                    foreach (ParticleEngine particle in StingrayParticles)
                    {
                        particle.Draw(spriteBatch);
                    }
                    foreach (ParticleEngine particle in Stingray2Particles)
                    {
                        particle.Draw(spriteBatch);
                    }

                    //Draw Enemies
                    foreach (Enemy enemy in Enemywave)
                    {
                        enemy.Draw(spriteBatch, gameTime, enemy.hurtFlash);
                    }

                    //Draw Powerups
                    foreach (ParticleEngine powerParticle in PowerupParticles)
                    {
                        powerParticle.Draw(spriteBatch);
                    }
                    foreach (PowerUp pUp in powerUpList)
                    {
                        pUp.Draw(spriteBatch, gameTime);
                    }

                    // Draw Score/Progress
                    foreach (Score scoreDisplay in DisplayScorePos)
                    {
                        scoreDisplay.Draw(spriteBatch, menuFont);
                    }
                    foreach (ProgressUI progress in DisplayProgress)
                    {
                        progress.Draw(spriteBatch, menuFont);
                    }

                    //Draw Follower
                    //follower.Draw(spriteBatch, gameTime, Color.White);

                    //Draw Explosion
                    for (int i = 0; i < DestructionParticles.Count; i++)
                    {
                        if (DestructionRadiusCounters[i] >= 200)
                        {
                            for (int f = 0; f < AftershockParticles.Count; f++)
                            {
                                AftershockParticles[f].Draw(spriteBatch);
                                if (AftershockParticles[f].particles.Count == 0)
                                {
                                    AftershockRadiusCounters.RemoveAt(f);
                                    AftershockAngleCounters.RemoveAt(f);
                                    AftershockEmmision.RemoveAt(f);
                                    AftershockParticles.RemoveAt(f);
                                }
                            }
                        }
                        DestructionParticles[i].Draw(spriteBatch);
                        if (DestructionParticles[i].particles.Count == 0)
                        {
                            DestructionRadiusCounters.RemoveAt(i);
                            DestructionAngleCounters.RemoveAt(i);
                            DestructionEmmision.RemoveAt(i);
                            DestructionParticles.RemoveAt(i);
                        }
                    }

                    //Draw Ship Thruster Particles
                    Thruster1.Draw(spriteBatch);
                    Thruster2.Draw(spriteBatch);

                    //Draw Ship
                    playerShip.Draw(spriteBatch, gameTime, Color.White);

                    //End Drawing Gameplay Stuff!!
                    spriteBatch.End();
                    _irr.SetupFullViewport();

                    //Begin Drawing UI
                    spriteBatch.Begin();
                    spriteBatch.Draw(health, healthRect, Color.White);
                    spriteBatch.DrawString(menuFont, "hp:" + (int)playerShip.Health + " / " + playerShip.MaxHealth, new Vector2(110.0f, GraphicsDevice.Viewport.Height - 47.0f), Color.White, 0.08f);
                    spriteBatch.Draw(xp, xpRect, Color.White);
                    spriteBatch.DrawString(menuFont, "xp:" + (int)playerShip.Experience + "/" + playerShip.ExperienceToNextLevel, new Vector2(110.0f, GraphicsDevice.Viewport.Height - 27.0f), Color.White, 0.08f);
                    spriteBatch.DrawString(menuFont, "Lvl:" + playerShip.Level, new Vector2(GraphicsDevice.Viewport.Width - 200.0f, 50.0f), Color.White, 0.25f);
                    spriteBatch.DrawString(menuFont, "Score:" + (int)score, new Vector2(0.0f, 50.0f), Color.White, 0.0f,Vector2.Zero,0.25f,SpriteEffects.None,0.0f);
                    spriteBatch.DrawString(menuFont, "Primary:" + playerShip.PrimaryType, new Vector2(100.0f, GraphicsDevice.Viewport.Height - 80.0f), Color.White, 0.0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0.0f);
                    spriteBatch.DrawString(menuFont, "Secondary:" + playerShip.SecondaryType, new Vector2(GraphicsDevice.Viewport.Width - 550.0f, GraphicsDevice.Viewport.Height - 80.0f), Color.White, 0.0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0.0f);

                    //End Drawing UI
                    spriteBatch.End();
                    _irr.SetupVirtualScreenViewport();
                    //base.Draw(gameTime);
                    break;

                //Pause******************************************************************************************************************
                case GameState.Pause:
                    _irr.DrawPause();
                    _irr.SetupFullViewport();
                    spriteBatch.Begin();
                    spriteBatch.DrawString(menuFont, "Paused", new Vector2(GraphicsDevice.Viewport.Width / 4, 100.0f), customColor, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    spriteBatch.End();
                    _irr.SetupVirtualScreenViewport();
                    break;

                //Game Over**************************************************************************************************************
                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.Black);
                    _irr.Draw();
                    _irr.SetupFullViewport();
                    spriteBatch.Begin();
                    gameOver.Draw(spriteBatch);
                    drawRect(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black);
                    spriteBatch.DrawString(menuFont, "Game Over\n", new Vector2(VirtualSize.Width / 6, VirtualSize.Height / 8), customColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(menuFont, "You Scored:" + score, new Vector2(VirtualSize.Width / 8, VirtualSize.Height / 4), customColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    _irr.SetupVirtualScreenViewport();
                    break;
            }
        }
        #endregion

        #region Random Generation Helper Methods
        public void LoadWave()
        {
            Enemywave.Clear();
            tempWave.Clear();
            int formationType;
            int formationSize;
            int percentage;
            string formationName;

            formationType = random.Next(1, 6);

            switch (formationType)
            {
                case 1:
                    formationName = "delta";
                    formationSize = 10;
                    break;
                case 2:
                    formationName = "v";
                    formationSize = 7;
                    break;
                case 3:
                    formationName = "line";
                    formationSize = 5;
                    break;
                case 4:
                    formationName = "diamond";
                    formationSize = 9;
                    break;
                case 5:
                    formationName = "shockwave";
                    formationSize = 9;
                    break;
                default:
                    formationName = "delta";
                    formationSize = 10;
                    break;
            }

            for (int i = 0; i < formationSize; i++)
            {
                Enemy enemy = new Stingray(Content, VirtualSize, 1, formationName);

                percentage = random.Next(1, 101);

                if (percentage > 0 && percentage <= 40)
                {
                    enemy = new Stingray(Content, VirtualSize, i + 1, formationName);
                    StingrayParticles.Add(new ParticleEngine(StingrayTextures, new Vector2(400, 240)));
                }
                else if (percentage > 40 && percentage <= 60)
                {
                    enemy = new Stingray2(Content, VirtualSize, i + 1, formationName);
                    Stingray2Particles.Add(new ParticleEngine(Stingray2Textures, new Vector2(400, 240)));
                }
                else if (percentage > 60 && percentage <= 90)
                {
                    enemy = new VoidAngel(Content, VirtualSize, i + 1, formationName);
                }
                else if (percentage > 90 && percentage <= 100)
                {
                    enemy = new VoidVulture(Content, VirtualSize, i + 1, formationName);
                }
                tempWave.Add(enemy);
            }
            Enemywave = tempWave;
        }

        //Spawn Powerups Method *************************************************************************************************************
        public void SpawnPowerUp(Vector2 Position)
        {
            switch (random.Next(9))
            {
                case 0:
                    Powerups = PowerUps.AtkSpdUp;
                    powerUpList.Add(new PowerUp(AtkSpdUp, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;
                case 1:
                    Powerups = PowerUps.MoveSpdUp;
                    powerUpList.Add(new PowerUp(MoveSpdUp, GraphicsDevice, Powerups, playerShip, Position, 1.0f));
                    break;
                case 2:
                    Powerups = PowerUps.HealthUp;
                    powerUpList.Add(new PowerUp(HPUp, GraphicsDevice, Powerups, playerShip, Position, 1.0f));
                    break;
                case 3:
                    Powerups = PowerUps.AtkSpdDown;
                    powerUpList.Add(new PowerUp(AtkSpdDown, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;
                case 4:
                    Powerups = PowerUps.MoveSpdDown;
                    powerUpList.Add(new PowerUp(MoveSpdDown, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;
                case 5:
                    Powerups = PowerUps.HealthDown;
                    powerUpList.Add(new PowerUp(HPDown, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;
                case 6:
                    Powerups = PowerUps.GravWellAmmo;
                    powerUpList.Add(new PowerUp(GravAmmo, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;
                case 7:
                    Powerups = PowerUps.HelixAmmo;
                    powerUpList.Add(new PowerUp(RocketAmmo, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;

                case 8:
                    Powerups = PowerUps.HomingAmmo;
                    powerUpList.Add(new PowerUp(HomingAmmo, GraphicsDevice, Powerups, playerShip, Position, 2.0f));
                    break;

                default: break;
            }
        }

        //Check Powerups Method *************************************************************************************************************
        public bool CheckForPowerUps(Rectangle player, Rectangle pwerUp)
        {
            return player.Intersects(pwerUp);
        }
        #endregion

        #region Update Helper Methods
        //Primary Collisions Check Method *************************************************************************************************************
        public void checkPrimaryCollisions(int turret)
        {
            List<Weapon> weaponList = new List<Weapon>();

            // selecting which turret to update
            if (turret == 1)
            {
                weaponList = playerShip.RailLeft.Primary;
            }
            if (turret == 2)
            {
                weaponList = playerShip.RailRight.Primary;
            }

            // updating damaged enemy response
            if (Enemywave.Count > 0)
            {
                for (int i = 0; i < Enemywave.Count; i++)
                {
                    if (Enemywave[i].painSwitch == true)
                    {
                        Enemywave[i].flashCounter++;
                        Enemywave[i].hurtFlash = new Color(random.Next(0, 255), random.Next(0, 10), random.Next(0, 100));
                        if (Enemywave[i].flashCounter >= 100)
                        {
                            Enemywave[i].hurtFlash = Color.White;
                            Enemywave[i].flashCounter = 0;
                            Enemywave[i].painSwitch = false;
                        }
                    }
                }
            }

            // collision detection with shots
            if (Enemywave.Count > 0 && weaponList.Count != 0)
            {
                for (int i = 0; i < Enemywave.Count; i++)
                {
                    int collide = Enemywave[i].CollisionShot(weaponList);
                    if (collide != -1)
                    {
                        audioManager.PlaySoundEffect("hit");
                        Enemywave[i].painSwitch = true;
                        Enemywave[i].flashCounter = 0;
                        Enemywave[i].Health -= weaponList[collide].Damage;
                        weaponList.RemoveAt(collide);
                        Enemywave[i].Position += Vector2.Normalize(Enemywave[i].Position - playerShip.Position) * 2000 / Enemywave[i].frameHeight;
                        playerShip.CurrentPrimaryAmmo++;

                        if (Enemywave[i].Health <= 0f)
                        {
                            score += Enemywave[i].score;
                            playerShip.Experience += Enemywave[i].score;
                            if (Enemywave[i].enemyType == "stingRay")
                            {
                                audioManager.PlaySoundEffect("enemy dead2");
                                StingrayParticles.RemoveAt(StingrayParticles.Count - 1);
                            }
                            if (Enemywave[i].enemyType == "stingRay2")
                            {
                                audioManager.PlaySoundEffect("enemy dead2");
                                Stingray2Particles.RemoveAt(Stingray2Particles.Count - 1);
                            }
                            if (Enemywave[i].enemyType == "voidVulture")
                            {
                                shakeSwitch = true;
                                audioManager.PlaySoundEffect("enemy dead");

                                DestructionParticles.Add(new ParticleEngine(DestructionTextures, new Vector2(400, 240)));
                                DestructionRadiusCounters.Add(10);
                                DestructionAngleCounters.Add(0);
                                DestructionEmmision.Add(Enemywave[i].Position);

                                AftershockParticles.Add(new ParticleEngine(AftershockTextures, new Vector2(400, 240)));
                                AftershockRadiusCounters.Add(10);
                                AftershockAngleCounters.Add(0);
                                AftershockEmmision.Add(Enemywave[i].Position);
                            }
                            if (Enemywave[i].enemyType == "voidAngel")
                            {
                                audioManager.PlaySoundEffect("enemy dead");
                            }

                            double rand = random.NextDouble();
                            if (rand < spawnChance)
                            {
                                SpawnPowerUp(Enemywave[i].Position);
                                PowerupParticles.Add(new ParticleEngine(PowerupTextures, new Vector2(400, 240)));
                                PowerupRadiusCounters.Add(10);
                                PowerupAngleCounters.Add(0);
                                PowerupEmmision.Add(Enemywave[i].Position);
                            }
                            Enemywave[i].Alive = false;
                            DisplayScorePos.Add(new Score(Enemywave[i].Position, Enemywave[i].score, menuFont));
                            Enemywave.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void RefreshGameInfo()
        {
            if (swapScreen)
            {
                gameState = GameState.StartMenu;
                swapScreen = false;
            }
            playerShip.Health = 5000;
            playerShip.Experience = 0;
            playerShip.SecondaryAmmo = 0;
            playerShip.Alive = true;
            currentWave = 0;
            Enemywave.Clear();        
        }

        public void UpdateEnemyShots()
        {
            int totalDamage = 0;

            foreach (Enemy enemy in Enemywave)
            {
                int collide = playerShip.CollisionShot(enemy.Primary);
                if (collide != -1)
                {
                    totalDamage += (int)enemy.Primary[0].Damage;
                    audioManager.PlaySoundEffect("hit");
                    enemy.Primary.RemoveAt(collide);
                }
            }
            playerShip.Health -= (int)totalDamage;
        }
        #endregion
    }
}
