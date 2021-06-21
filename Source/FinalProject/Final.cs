using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


using GameEngine;

namespace FinalProject
{
    public class Final : Game
    {
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        Random random = new Random();
        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Effect effect;

        //Background
        TerrainRenderer terrain;
        ScrollingBackground background;

        //Sounds
        SoundEffect laser;
        SoundEffect lvlclear;
        SoundEffect[] chink = new SoundEffect[4];
        SoundEffect[] hit = new SoundEffect[2];
        SoundEffect[] crash = new SoundEffect[2];
        SoundEffect[] power = new SoundEffect[3];
        SoundEffect[] menu = new SoundEffect[3];
        SoundEffectInstance soundInstance;

        Song menuMusic, stage1Music, stage2Music, bossMusic;

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        //Enemies
        BasicEnemy[] basics = new BasicEnemy[10];
        CircleEnemy[] circles = new CircleEnemy[10];

        //Pick Up
        Health heart;
        Big big;
        Spread spread;

        //GUI
        Button contButton = new Button();
        Button startButton = new Button();
        Button exitButton = new Button();
        Button howtoButton = new Button();
        Button backtoMenu = new Button();
        Button creditsMenu = new Button();
        Texture2D health;
        SpriteFont Font;
        SpriteEffects spriteEffect;

        Camera camera;
        Light light;

        Ship player;        
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        float powerstart;
        float barPercent;
        bool[] waves = new bool[5];
        bool songSwitch = true;
        int destroyed = 0;
        float score = 0;
        int time = 0;
        int level = 1;

        //Demo Screen stuff
        Demo[] demos = new Demo[9];

        public Final()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            

            Time.Initialize();
            InputManager.Initialize();

            ScreenManager.Initialize(graphics);
            ScreenManager.Setup(false, 900, 1200);

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            health = Content.Load<Texture2D>("Textures/emptySquare");
            Font = Content.Load<SpriteFont>("font");
            spriteEffect = new SpriteEffects();


            //Load Sounds
            laser = Content.Load<SoundEffect>("Sounds/laser");
            lvlclear = Content.Load<SoundEffect>("Sounds/tuturu_1");
            chink[0] = Content.Load<SoundEffect>("Sounds/metal1");
            chink[1] = Content.Load<SoundEffect>("Sounds/metal2");
            chink[2] = Content.Load<SoundEffect>("Sounds/metal3");
            chink[3] = Content.Load<SoundEffect>("Sounds/metal4");

            hit[0] = Content.Load<SoundEffect>("Sounds/hit1");
            hit[1] = Content.Load<SoundEffect>("Sounds/hit2");

            crash[0] = Content.Load<SoundEffect>("Sounds/crash1");
            crash[1] = Content.Load<SoundEffect>("Sounds/crash2");

            power[0] = Content.Load<SoundEffect>("Sounds/powerHealth");
            power[1] = Content.Load<SoundEffect>("Sounds/powerBig");
            power[2] = Content.Load<SoundEffect>("Sounds/powerSpread");

            menu[0] = Content.Load<SoundEffect>("Sounds/menu1");
            menu[1] = Content.Load<SoundEffect>("Sounds/menu2");
            menu[2] = Content.Load<SoundEffect>("Sounds/menu3");

            menuMusic = Content.Load<Song>("Music/title1");
            stage1Music = Content.Load<Song>("Music/stage1");
            stage2Music = Content.Load<Song>("Music/stage2");
            bossMusic = Content.Load<Song>("Music/boss1");
            MediaPlayer.IsRepeating = true;

            //Load Background
            background = new ScrollingBackground(Content.Load<Texture2D>("Textures/stars2"));
            background.Speed = 5;

            // Load Particles
            particleManager = new ParticleManager(GraphicsDevice, 30);
            particleEffect = Content.Load<Effect>("Models/ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("Textures/fire");

            //Load Buttons
            startButton = new Button();
            startButton.Texture = Content.Load<Texture2D>("Textures/Square");
            startButton.Text = "START";
            startButton.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 200, (int)ScreenManager.Height / 2 - 100, 400, 75);
            startButton.TextScale = 1.5f;
            startButton.Action += ChangeScene;
            guiElements.Add(startButton);

            contButton = new Button();
            contButton.Texture = Content.Load<Texture2D>("Textures/Square");
            contButton.Text = "CONTINUE";
            contButton.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 150, (int)ScreenManager.Height / 2 - 50, 300, 50);
            contButton.TextScale = 1.5f;
            contButton.Action += NextLevel;
            guiElements.Add(contButton);

            exitButton = new Button();
            exitButton.Texture = Content.Load<Texture2D>("Textures/Square");
            exitButton.Text = "Exit Game";
            exitButton.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 150, (int)ScreenManager.Height / 2 + 400 , 300, 50);
            exitButton.TextScale = 1.5f;
            exitButton.Action += ExitGame;
            guiElements.Add(exitButton);

            howtoButton = new Button();
            howtoButton.Texture = Content.Load<Texture2D>("Textures/Square");
            howtoButton.Text = "How To Play";
            howtoButton.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 150, (int)ScreenManager.Height / 2 + 50, 300, 50);
            howtoButton.TextScale = 1.5f;
            howtoButton.Action += HowToScene;
            guiElements.Add(howtoButton);

            backtoMenu = new Button();
            backtoMenu.Texture = Content.Load<Texture2D>("Textures/Square");
            backtoMenu.Text = "Back to Menu";
            backtoMenu.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 150, (int)ScreenManager.Height / 2 + 400, 300, 50);
            backtoMenu.TextScale = 1.5f;
            backtoMenu.Action += BacktoMenu;
            guiElements.Add(backtoMenu);

            creditsMenu = new Button();
            creditsMenu.Texture = Content.Load<Texture2D>("Textures/Square");
            creditsMenu.Text = "Credits";
            creditsMenu.Bounds = new Rectangle((int)ScreenManager.Width / 2 - 150, (int)ScreenManager.Height / 2 + 150, 300, 50);
            creditsMenu.TextScale = 1.5f;
            creditsMenu.Action += CreditsMenu;
            guiElements.Add(creditsMenu);

            //Load Scenes
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("lvl1", new Scene(lvl1Update, PlayDraw));
            scenes.Add("lvl2", new Scene(lvl2Update, PlayDraw));
            scenes.Add("lvl3", new Scene(lvl3Update, PlayDraw));
            scenes.Add("lvl4", new Scene(lvl4Update, PlayDraw));
            scenes.Add("lvl5", new Scene(lvl5Update, PlayDraw));
            scenes.Add("lvl6", new Scene(lvl6Update, PlayDraw));
            scenes.Add("lvl7", new Scene(lvl7Update, PlayDraw));
            scenes.Add("lvl8", new Scene(lvl8Update, PlayDraw));
            scenes.Add("lvl9", new Scene(lvl9Update, PlayDraw));
            scenes.Add("lvl10", new Scene(lvl10Update, PlayDraw));
            scenes.Add("win", new Scene(WinMenuUpdate, WinMenuDraw));
            scenes.Add("lose", new Scene(LoseMenuUpdate, LoseMenuDraw));
            scenes.Add("howto", new Scene(HowToMenuUpdate, HowToMenuDraw));
            scenes.Add("Score", new Scene(ScoreMenuUpdate, ScoreMenuDraw));
            scenes.Add("credits", new Scene(CreditsMenuUpdate, CreditsMenuDraw));
            currentScene = scenes["Menu"];

            for (int i = 0; i < waves.Length; i++)
                waves[i] = true;

            terrain = new TerrainRenderer(Content.Load<Texture2D>("Textures/heightmap"),
                Vector2.One * 100,
                Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("Textures/heightmap");
            float height = terrain.GetHeight(new Vector2(0.5f, 0.5f));
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(200, 75, 200);

            // Effect
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            // Load Camera and Lights
            camera = new Camera();
            camera.Transform = new Transform();
            camera.NearPlane = GameConstants.CameraHeight - 1000f;
            camera.FarPlane = GameConstants.CameraHeight + 1000f;
            camera.AspectRatio = 0.8f;
            camera.Transform.LocalPosition = Vector3.Up * GameConstants.CameraHeight;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;


            //Load Player Enemies and Bullets
            player = new Ship(terrain, Content, camera, GraphicsDevice, light);
            player.Transform.Scale *= 1;
            player.Transform.LocalPosition = new Vector3(0, 0, 0);

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light, false);
                bulletList[i].Transform.Scale *= 1;
            }

            for (int i = 0; i < circles.Length; i++)
            {
                circles[i] = new CircleEnemy(terrain, Content, camera, GraphicsDevice, light, 50);
                circles[i].Transform.Position = new Vector3(-8000 + (16000 * i), 0, 0);
                circles[i].destination = new Vector3(-4000 + (2000 * i), 0, -7000);
            }


            for (int i = 0; i < basics.Length; i++)
            {
                basics[i] = new BasicEnemy(terrain, Content, camera, GraphicsDevice, light, 50);
                basics[i].Transform.Position = new Vector3(-4000 + (2000 * i), 0, -12000);
                basics[i].destination = new Vector3(-4000 + (2000 * i), 0, -7000);
            }

            //Load Powerups
            heart = new Health(terrain, Content, camera, GraphicsDevice, light, GameConstants.pickupSpeed);
            heart.Transform.LocalScale *= 5f;

            big = new Big(terrain, Content, camera, GraphicsDevice, light, GameConstants.pickupSpeed);
            big.Transform.LocalScale *= 5f;

            spread = new Spread(terrain, Content, camera, GraphicsDevice, light, GameConstants.pickupSpeed);
            spread.Transform.LocalScale *= 5f;

            //Demo Load Stuff
            for(int i = 0; i < 5; i++)
            {
                demos[i] = new Demo(Content.Load<Model>("Models/p1_wedge"), Content, camera, GraphicsDevice, light);
            }
            demos[4].Renderer.Material.Diffuse = new Vector3(GameConstants.circleRed, GameConstants.circleGreen, GameConstants.circleBlue);
            demos[5] = new Demo(Content.Load<Model>("Models/Spaceships_3"), Content, camera, GraphicsDevice, light);
            demos[5].Renderer.Material.Diffuse = new Vector3(GameConstants.basicRed, GameConstants.basicGreen, GameConstants.basicBlue);
            for (int i = 6; i < demos.Length; i++)
            {
                demos[i] = new Demo(Content.Load<Model>("Models/Sphere"), Content, camera, GraphicsDevice, light);
                demos[i].Transform.LocalScale *= 1000f;
            }
            demos[6].Renderer.Material.Diffuse = new Vector3(3f, 0.5f, 0.5f);
            demos[7].Renderer.Material.Diffuse = new Vector3(0.5f, 3f, 0.5f);
            demos[8].Renderer.Material.Diffuse = new Vector3(0.5f, 0.5f, 3f);

            //Demo Scale Rotation and Positions
            demos[0].Transform.Position = new Vector3(-4000, 0, -6500);
            demos[0].Transform.Rotate(Vector3.Up, -(float)MathHelper.PiOver2);
            demos[0].Rigidbody.Acceleration += demos[0].Transform.Forward * GameConstants.ShipSpeed * (demos[0].reverse ? -1 : 1);
            demos[1].Transform.Position = new Vector3(-2000, 0, -3500);
            demos[2].Transform.Position = new Vector3(2000, 0, -3500);
            demos[3].Transform.Position = new Vector3(-4000, 0, -1000);
            demos[3].Transform.Rotate(Vector3.Up, -(float)MathHelper.PiOver2);
            demos[3].shoots = true;
            demos[4].Transform.Position = new Vector3(2000, 0, 1800);
            demos[4].Transform.Rotate(Vector3.Up, -(float)MathHelper.PiOver2);
            demos[4].Transform.Scale *= 1.2f;
            demos[5].Transform.Position = new Vector3(-2000, 0, 2000);
            demos[5].Transform.Rotate(Vector3.Up, -(float)MathHelper.PiOver2);
            demos[5].Transform.Scale *= 0.3f;
            demos[6].Transform.Position = new Vector3(-3000, 0, 4500);
            demos[6].Renderer.Material.Texture = Content.Load<Texture2D>("Textures/fire");
            demos[7].Transform.Position = new Vector3(0, 0, 4500);
            demos[7].Renderer.Material.Texture = Content.Load<Texture2D>("Textures/power1tex");
            demos[8].Transform.Position = new Vector3(3000, 0, 4500);
            demos[8].Renderer.Material.Texture = Content.Load<Texture2D>("Textures/power2tex");


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);
            particleManager.Update();
            currentScene.Update();
            background.Update();
            terrain.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);

            if (player.isActive)
            {
                //Player
                player.Update(gameTime);
                if (player.Transform.Position.X < -GameConstants.PlayfieldSizeX + 2000)
                {
                    player.Transform.Position = new Vector3(-GameConstants.PlayfieldSizeX + 2000, 0, player.Transform.Position.Z);
                    player.Rigidbody.Acceleration *= 0;
                    player.Rigidbody.Velocity = new Vector3(0, 0, player.Rigidbody.Velocity.Z);
                }
                if (player.Transform.Position.X > GameConstants.PlayfieldSizeX - 2000)
                {
                    player.Transform.Position = new Vector3(GameConstants.PlayfieldSizeX - 2000, 0, player.Transform.Position.Z);
                    player.Rigidbody.Acceleration *= 0;
                    player.Rigidbody.Velocity = new Vector3(0, 0, player.Rigidbody.Velocity.Z);
                }
                if (player.Transform.Position.Z < -GameConstants.PlayfieldSizeY + 2000)
                {
                    player.Transform.Position = new Vector3(player.Transform.Position.X, 0, -GameConstants.PlayfieldSizeY + 2000);
                    player.Rigidbody.Acceleration *= 0;
                    player.Rigidbody.Velocity = new Vector3(player.Rigidbody.Velocity.X, 0, 0);
                }
                if (player.Transform.Position.Z > GameConstants.PlayfieldSizeY - 2000)
                {
                    player.Transform.Position = new Vector3(player.Transform.Position.X, 0, GameConstants.PlayfieldSizeY - 2000);
                    player.Rigidbody.Acceleration *= 0;
                    player.Rigidbody.Velocity = new Vector3(player.Rigidbody.Velocity.X, 0, 0);
                }

                if (InputManager.IsKeyPressed(Keys.Space))
                {
                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        if (!bulletList[i].isActive)
                        {
                            bulletList[i].Rigidbody.Velocity = (player.Transform.Forward) * (GameConstants.BulletSpeedAdjustment);
                            bulletList[i].Transform.LocalPosition = player.Transform.Position;
                            bulletList[i].isActive = true;
                            score -= GameConstants.ShotPenalty;
                            if (player.spread)
                            {
                                int temp = i;
                                bool left = false;
                                bool right = false;
                                do
                                {
                                    temp++;
                                    if (temp > bulletList.Length)
                                        temp = 0;
                                    if (!left && !bulletList[temp].isActive)
                                    {
                                        bulletList[temp].Rigidbody.Velocity = (player.Transform.Forward + player.Transform.Left / 2) * (GameConstants.BulletSpeedAdjustment);
                                        bulletList[temp].Transform.LocalPosition = player.Transform.Position;
                                        bulletList[temp].isActive = true;
                                        if (player.big)
                                            bulletList[temp].Transform.Scale = Vector3.One * 3;
                                        else
                                            bulletList[temp].Transform.Scale = Vector3.One;
                                        left = true;
                                    }
                                    temp++;
                                    if (temp > bulletList.Length)
                                        temp = 0;
                                    if (!right && !bulletList[temp].isActive)
                                    {
                                        bulletList[temp].Rigidbody.Velocity = (player.Transform.Forward + player.Transform.Right / 2) * (GameConstants.BulletSpeedAdjustment);
                                        bulletList[temp].Transform.LocalPosition = player.Transform.Position;
                                        bulletList[temp].isActive = true;
                                        if (player.big)
                                            bulletList[temp].Transform.Scale = Vector3.One * 3;
                                        else
                                            bulletList[temp].Transform.Scale = Vector3.One;
                                        right = true;
                                    }

                                } while (!left && !right);

                            }

                            if (player.big)
                                bulletList[i].Transform.Scale = Vector3.One * 3;
                            else
                                bulletList[i].Transform.Scale = Vector3.One;
                            soundInstance = laser.CreateInstance();
                            soundInstance.Play();
                            break;
                        }
                    }


                }

                foreach (GameObject gameObject in bulletList)
                    gameObject.Update();

                hitDetection(gameTime);

                barPercent = (float)((gameTime.TotalGameTime.TotalSeconds - powerstart) / GameConstants.powerUpTime);

                heart.Update();
                big.Update();
                spread.Update();
            }
            else
                currentScene = scenes["lose"];

            //Enemies
            foreach (CircleEnemy circleEnemy in circles)
            {
                circleEnemy.Update(player, gameTime);
            }

            foreach (BasicEnemy basicEnemy in basics)
            {
                basicEnemy.Update(player, gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {


            currentScene.Draw();
            base.Draw(gameTime);
        }
        
        // Game Methods
        private void hitDetection(GameTime gameTime)
        {
            //Collisions

            //Basic Enemies
            Vector3 normal;
            for (int i = 0; i < basics.Length; i++)
                if (basics[i].isActive)
                {
                    // Check for Player Bullet to Enemy Collision
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive)
                            if (basics[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = basics[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 50), 2, random.Next(-15, 150));
                                particle.Acceleration = new Vector3(0, 30, 0);
                                particle.MaxAge = random.Next(1, 5);
                                particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                particle.Init();
                                soundInstance = chink[random.Next(0, chink.Length)].CreateInstance();
                                soundInstance.Play();
                                basics[i].hits++;
                                score += GameConstants.HitBonus;
                                if (player.big)
                                {
                                    basics[i].hits += 2;
                                    score += GameConstants.HitBonus;
                                }
                                if (basics[i].hits >= basics[i].maxHP)
                                {
                                    for (int k = 0; k < random.Next(5, 10); k++)
                                    {
                                        particle = particleManager.getNext();
                                        particle.Position = basics[i].Transform.Position;
                                        particle.Velocity = new Vector3(
                                          random.Next(-300, 600), 0, random.Next(-200, 500));
                                        particle.Acceleration = new Vector3(0, 30, 0);
                                        particle.MaxAge = random.Next(1, 5);
                                        particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                        particle.Init();
                                    }
                                    soundInstance = crash[random.Next(0, crash.Length)].CreateInstance();
                                    soundInstance.Play();
                                    score += GameConstants.KillBonus;
                                    destroyed++;
                                    spawnPowerUp(basics[i].Transform.Position);
                                }
                                bulletList[j].isActive = false;


                                break; //no need to check other bullets
                            }

                    //Check for Enemy bullet to Player Collision
                    for (int j = 0; j < basics[i].bulletList.Length; j++)
                        if (basics[i].bulletList[j].isActive)
                            if (basics[i].bulletList[j].Collider.Collides(player.Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = player.Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 50), 2, random.Next(-15, 150));
                                particle.Acceleration = new Vector3(0, 30, 0);
                                particle.MaxAge = random.Next(1, 5);
                                particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                particle.Init();
                                soundInstance = hit[random.Next(0, hit.Length)].CreateInstance();
                                soundInstance.Play();
                                player.hits++;
                                basics[i].bulletList[j].isActive = false;
                                score -= GameConstants.HitBonus;
                                break; //no need to check other bullets
                            }


                    // Check for Player Ship to Enemy Ship Collision
                    if (basics[i].Collider.Collides(player.Collider, out normal))
                    {
                        // Particles
                        Particle particle = particleManager.getNext();
                        particle.Position = basics[i].Transform.Position;
                        particle.Velocity = new Vector3(
                          random.Next(-5, 5), 2, random.Next(-50, 50));
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 6);
                        particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                        particle.Init();
                        soundInstance = crash[random.Next(0, crash.Length)].CreateInstance();
                        soundInstance.Play();
                        if (!basics[i].isBoss)
                        {
                            destroyed++;
                            basics[i].isActive = false;
                            player.hits += 2;
                        }
                        else
                        {
                            player.hits += 5;
                        }
                        break; //no need to check other asteroids
                    }



                }

            //Circle Enemies
            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                {
                    // Check for Player Bullet to Enemy Collision
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive)
                            if (circles[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = circles[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 50), 2, random.Next(-15, 150));
                                particle.Acceleration = new Vector3(0, 30, 0);
                                particle.MaxAge = random.Next(1, 5);
                                particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                particle.Init();
                                soundInstance = chink[random.Next(0, chink.Length)].CreateInstance();
                                soundInstance.Play();
                                circles[i].hits++;
                                score += GameConstants.HitBonus;
                                if (player.big)
                                {
                                    circles[i].hits += 2;
                                    score += GameConstants.HitBonus;
                                }
                                if (circles[i].hits >= circles[i].maxHP)
                                {
                                    for (int k = 0; k < random.Next(5, 10); k++)
                                    {
                                        particle = particleManager.getNext();
                                        particle.Position = circles[i].Transform.Position;
                                        particle.Velocity = new Vector3(
                                          random.Next(-300, 600), 0, random.Next(-200, 500));
                                        particle.Acceleration = new Vector3(0, 30, 0);
                                        particle.MaxAge = random.Next(1, 5);
                                        particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                        particle.Init();
                                    }

                                    soundInstance = crash[random.Next(0, crash.Length)].CreateInstance();
                                    soundInstance.Play();
                                    score += GameConstants.KillBonus;
                                    destroyed++;
                                    spawnPowerUp(circles[i].Transform.Position);
                                }
                                bulletList[j].isActive = false;

                                break; //no need to check other bullets
                            }

                    //Check for Enemy bullet to Player Collision
                    for (int j = 0; j < circles[i].bulletList.Length; j++)
                        if (circles[i].bulletList[j].isActive)
                            if (circles[i].bulletList[j].Collider.Collides(player.Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = player.Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 50), 2, random.Next(-15, 150));
                                particle.Acceleration = new Vector3(0, 30, 0);
                                particle.MaxAge = random.Next(1, 5);
                                particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                                particle.Init();
                                soundInstance = hit[random.Next(0, hit.Length)].CreateInstance();
                                soundInstance.Play();
                                player.hits++;
                                circles[i].bulletList[j].isActive = false;
                                //destroyed++;
                                //score += GameConstants.KillBonus;
                                break; //no need to check other bullets
                            }


                    // Check for Player Ship to Enemy Ship Collision
                    if (circles[i].Collider.Collides(player.Collider, out normal))
                    {
                        // Particles
                        Particle particle = particleManager.getNext();
                        particle.Position = circles[i].Transform.Position;
                        particle.Velocity = new Vector3(
                          random.Next(-5, 5), 2, random.Next(-50, 50));
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 6);
                        particle.Color = new Vector3(random.Next(150, 255), 0, 0);
                        particle.Init();

                        soundInstance = crash[random.Next(0, crash.Length)].CreateInstance();
                        soundInstance.Play();
                        circles[i].isActive = false;
                        destroyed++;
                        player.hits++;
                        player.hits++;
                        break; //no need to check other asteroids
                    }

                    //Spread Circles out
                    for (int j = i+1; j < circles.Length; j++)
                        if (circles[j].isActive)
                            if (Vector3.Distance(circles[i].Transform.Position, circles[j].Transform.Position) < GameConstants.circleSpread)
                            {
                                circles[i].Transform.Position += Vector3.Normalize(circles[i].Transform.Position - circles[j].Transform.Position) * 50;
                                circles[j].Transform.Position += Vector3.Normalize(circles[j].Transform.Position - circles[i].Transform.Position) * 50;
                            }




                }

            if (heart.isActive && heart.Collider.Collides(player.Collider, out normal))
            {
                heart.isActive = false;
                player.hits--;
                if (player.hits <= 0)
                    player.hits = 0;
                soundInstance = power[0].CreateInstance();
                soundInstance.Play();
            }


            if (big.isActive && big.Collider.Collides(player.Collider, out normal))
            {
                big.isActive = false;
                player.big = true;
                player.powerup = (float)gameTime.TotalGameTime.TotalSeconds + GameConstants.powerUpTime;
                soundInstance = power[1].CreateInstance();
                soundInstance.Play();
                powerstart = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            if (spread.isActive && spread.Collider.Collides(player.Collider, out normal))
            {
                spread.isActive = false;
                player.spread = true;
                player.powerup = (float)gameTime.TotalGameTime.TotalSeconds + GameConstants.powerUpTime;
                soundInstance = power[2].CreateInstance();
                soundInstance.Play();
                powerstart = (float)gameTime.TotalGameTime.TotalSeconds;
            }
        }     

        private void spawnPowerUp(Vector3 location)
        {
            int temp = random.Next(0, 100);
            if(temp > GameConstants.healthChance && temp < GameConstants.bigChance)
            {
                heart.Transform.Position = location;
                heart.isActive = true;
            }
            if (temp > GameConstants.bigChance && temp < GameConstants.spreadChance)
            {
                big.Transform.Position = location;
                big.isActive = true;
            }
            if (temp > GameConstants.spreadChance)
            {
                spread.Transform.Position = location;
                spread.isActive = true;
            }
        }

        private bool ClearCheck()
        {
            for (int i = 0; i < basics.Length; i++)
                if (basics[i].isActive)
                    return false;

            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    return false;

            return true;
        }

        //Scene Methods and Draws
        void MainMenuUpdate()
        {
            if(songSwitch)
            {
                MediaPlayer.Play(menuMusic);
                songSwitch = false;
            }
            score = 0;
            guiElements[0].Update();
            guiElements[3].Update();
            guiElements[2].Update();
            guiElements[5].Update();

        }
        void MainMenuDraw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.Blue);
            guiElements[0].Draw(spriteBatch, Font);
            guiElements[2].Draw(spriteBatch, Font);
            guiElements[3].Draw(spriteBatch, Font);
            guiElements[5].Draw(spriteBatch, Font);
            spriteBatch.End();
        }
        void HowToMenuUpdate()
        {
            guiElements[4].Update();

            if (demos[0].Transform.Position.X > 100f)
                demos[0].Rigidbody.Acceleration = -demos[0].Transform.Forward * GameConstants.ShipSpeed;
            if (demos[0].Transform.Position.X < -100f)
                demos[0].Rigidbody.Acceleration = demos[0].Transform.Forward * GameConstants.ShipSpeed;


            foreach (Demo demo in demos)
                demo.Update();

            demos[1].Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            demos[2].Transform.Rotate(Vector3.Down, Time.ElapsedGameTime);
            demos[4].Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            demos[5].Transform.Rotate(Vector3.Down, Time.ElapsedGameTime);
            demos[6].Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            demos[7].Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            demos[8].Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);


            if ((float)Time.TotalGameTime.TotalSeconds > demos[3].shotTime)
            {
                demos[3].shotTime = (float)(Time.TotalGameTime.TotalSeconds + GameConstants.timeBetweenShots / 2);
                demos[3].shoots = true;
            }
            else
                demos[3].shoots = false;
            if (demos[3].shoots)
                for (int i = 0; i < bulletList.Length; i++)
                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity = demos[3].Transform.Forward * (GameConstants.BulletSpeedAdjustment);
                        bulletList[i].Transform.LocalPosition = demos[3].Transform.Position;
                        bulletList[i].isActive = true;
                        break;
                    }
        }

        void HowToMenuDraw()
        {
            string temp = "Press \"W\" and \"S\" to Speed Up and Down";
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/stars"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.AliceBlue);
         //   spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.AliceBlue);
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 550), Color.White, 0, Vector2.Zero, 1.25f, new SpriteEffects(), 0);

            temp = "Press \"A\" and \"D\" to Turn Left and Right";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 325), Color.White, 0, Vector2.Zero, 1.25f, new SpriteEffects(), 0);

            temp = "Press \"Space\" to Shoot";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 150), Color.White, 0, Vector2.Zero, 1.25f, new SpriteEffects(), 0);

            temp = "Dodge Enemies and their Bullets";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 +25), Color.White, 0, Vector2.Zero, 1.25f, new SpriteEffects(), 0);

            temp = "Pick up Health and PowerUps";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 + 200), Color.White, 0, Vector2.Zero, 1.25f, new SpriteEffects(), 0);

            guiElements[4].Draw(spriteBatch, Font);
            spriteBatch.End();

            foreach (Demo demo in demos)
                demo.Draw();

            foreach (Bullet bullets in bulletList)
                bullets.Draw();

        }
        void CreditsMenuUpdate()
        {
            guiElements[4].Update();          
        }

        void CreditsMenuDraw()
        {
            string temp = "Created By:";
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.AliceBlue);
            //   spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.AliceBlue);
            spriteBatch.DrawString(Font, temp, new Vector2(350, ScreenManager.Height / 2 - 550), Color.Black, 0, Vector2.Zero, 1.1f, new SpriteEffects(), 0);

            temp = "Christopher Olson";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 525), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);

            temp = "Coded By:";
            spriteBatch.DrawString(Font, temp, new Vector2(350, ScreenManager.Height / 2 - 450), Color.Black, 0, Vector2.Zero, 1.1f, new SpriteEffects(), 0);

            temp = "Christopher Olson";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 425), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);

            temp = "For Class:";
            spriteBatch.DrawString(Font, temp, new Vector2(350, ScreenManager.Height / 2 - 350), Color.Black, 0, Vector2.Zero, 1.1f, new SpriteEffects(), 0);

            temp = "CSE311 Fall 2020";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 325), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);

            temp = "Special Thanks:";
            spriteBatch.DrawString(Font, temp, new Vector2(350, ScreenManager.Height / 2 - 250), Color.Black, 0, Vector2.Zero, 1.1f, new SpriteEffects(), 0);

            temp = "CGtrader.com (3d models)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 225), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "ZapSplat.com (sound effects)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 200), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "SoundImage.org (music)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 175), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Google (textures)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 150), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Youtube (my CS degree)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 125), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Budweiser (moral support)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 100), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Satan (also moral support)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 75), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Coronavirus (covid coming for you)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 50), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);
            temp = "Arizona (for electing Biden)";
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 1.25f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 25), Color.Black, 0, Vector2.Zero, 1.5f, new SpriteEffects(), 0);

            guiElements[4].Draw(spriteBatch, Font);
            spriteBatch.End();

        }
        void ScoreMenuUpdate()
        {
            guiElements[1].Update();

            destroyed = 0;

            player.Transform.Position = new Vector3(0, 0, 0);
            player.Rigidbody.Acceleration *= 0;
            player.Rigidbody.Velocity *= 0;

            // Reset Enemies and Enemy Bullets
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i].isActive = false;
                circles[i].Transform.Position = new Vector3(0, 0, -12000);
                circles[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in circles[i].bulletList)
                    bullets.isActive = false;
            }

            for (int i = 0; i < basics.Length; i++)
            {
                basics[i].isActive = false;
                basics[i].isBoss = false;
                basics[i].maxHP = GameConstants.basicHealth;
                basics[i].shotMod = 1f;
                basics[i].Transform.Scale = Vector3.One * 0.1f;
                basics[i].Transform.Position = new Vector3(-4000 + (2000 * i), 0, -12000);
                basics[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in basics[i].bulletList)
                    bullets.isActive = false;
            }

            //Reset PowerUps
            heart.isActive = false;
            big.isActive = false;
            spread.isActive = false;

            for (int i = 0; i < waves.Length; i++)
                waves[i] = true;


        }        
        void ScoreMenuDraw()
        {
            string temp = "Level " + level.ToString() + " Complete";
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.Blue);
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f*(float)Font.MeasureString(temp).X/2, ScreenManager.Height / 2 - 500), Color.Black, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);
            temp = "Score: " + score.ToString();
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 300), Color.Black, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);

            guiElements[1].Draw(spriteBatch, Font);
            spriteBatch.End();
        }
        void WinMenuUpdate()
        {
            guiElements[2].Update();

            player.Transform.Position = new Vector3(0, 0, 0);
            player.Rigidbody.Acceleration *= 0;
            player.Rigidbody.Velocity *= 0;

            // Reset Enemies and Enemy Bullets
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i].Transform.Position = new Vector3(0, 0, -12000);
                circles[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in circles[i].bulletList)
                    bullets.isActive = false;
            }

            for (int i = 0; i < basics.Length; i++)
            {
                basics[i].isActive = false;
                basics[i].isBoss = false;
                basics[i].maxHP = GameConstants.basicHealth;
                basics[i].shotMod = 1f;
                basics[i].Transform.Scale = Vector3.One * 0.1f;
                basics[i].Transform.Position = new Vector3(-4000 + (2000 * i), 0, -12000);
                basics[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in basics[i].bulletList)
                    bullets.isActive = false;
            }

            //Reset PowerUps
            heart.isActive = false;
            big.isActive = false;
            spread.isActive = false;

            for (int i = 0; i < waves.Length; i++)
                waves[i] = true;


        }
        void WinMenuDraw()
        {
            string temp = "YOU WIN";
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.Blue);
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 500), Color.Black, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);
            temp = "Final Score: " + score.ToString();
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 300), Color.Black, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);

            guiElements[2].Draw(spriteBatch, Font);
            spriteBatch.End();
        }
        void LoseMenuUpdate()
        {
            songSwitch = true;
            guiElements[4].Update();

            destroyed = 0;

            player.Transform.Position = new Vector3(0, 0, 0);
            player.Rigidbody.Acceleration *= 0;
            player.Rigidbody.Velocity *= 0;

            // Reset Enemies and Enemy Bullets
            for (int i = 0; i < circles.Length; i++)
            {
                circles[i].isActive = false;
                circles[i].Transform.Position = new Vector3(0, 0, -12000);
                circles[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in circles[i].bulletList)
                    bullets.isActive = false;
            }

            for (int i = 0; i < basics.Length; i++)
            {
                basics[i].isActive = false;
                basics[i].isBoss = false;
                basics[i].maxHP = GameConstants.basicHealth;
                basics[i].shotMod = 1f;
                basics[i].Transform.Scale = Vector3.One * 0.1f;
                basics[i].Transform.Position = new Vector3(-4000 + (2000 * i), 0, -12000);
                basics[i].destination = new Vector3(0, 0, -7000);
                foreach (Bullet bullets in basics[i].bulletList)
                    bullets.isActive = false;
            }

            //Reset PowerUps
            heart.isActive = false;
            big.isActive = false;
            spread.isActive = false;

            for (int i = 0; i < waves.Length; i++)
                waves[i] = true;


        }
        void LoseMenuDraw()
        {
            string temp = "You Died...";
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/emptySquare"), new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height), Color.Black);
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 500), Color.Red, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);
            temp = "Final Score: " + score.ToString();
            spriteBatch.DrawString(Font, temp, new Vector2((float)ScreenManager.Width / 2 - 2f * (float)Font.MeasureString(temp).X / 2, ScreenManager.Height / 2 - 300), Color.White, 0, Vector2.Zero, 2f, new SpriteEffects(), 0);

            guiElements[4].Draw(spriteBatch, Font);
            spriteBatch.End();
        }


        //Button Methods
        void ChangeScene(GUIElement element)
        {
            songSwitch = true;
            soundInstance = menu[1].CreateInstance();
            soundInstance.Play();
            currentScene = scenes["lvl1"];
        }

        void NextLevel(GUIElement element)
        {
            soundInstance = menu[2].CreateInstance();
            soundInstance.Play();

            for (int i = 0; i < waves.Length; i++)
                waves[i] = true;
            destroyed = 0;

            level++;
            if (level == 5 || level == 6 || level == 10)
                songSwitch = true;
          
            currentScene = scenes["lvl" + level.ToString()];

        }
        void ExitGame(GUIElement element)
        {
            Environment.Exit(0);
        }

        void HowToScene(GUIElement element)
        {
            soundInstance = menu[2].CreateInstance();
            soundInstance.Play();
            currentScene = scenes["howto"];
        }

        void BacktoMenu(GUIElement element)
        {
            soundInstance = menu[0].CreateInstance();
            soundInstance.Play();
            player.hits = 0;
            player.isActive = true;
            level = 1;
            currentScene = scenes["Menu"];
        }

        void CreditsMenu(GUIElement element)
        {
            soundInstance = menu[2].CreateInstance();
            soundInstance.Play();
            currentScene = scenes["credits"];
        }

        //Level Scenes and Updates
        void lvl1Update()
        {
            if (songSwitch)
            {
                MediaPlayer.Play(stage1Music);
                songSwitch = false;
            }

            int totalenemy = 2;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if(waves[0])
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                    basics[i].destination = new Vector3(-4000 + (8000 * i), 0, -7000);
                }

                for (int i = 0; i < 0; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl2Update()
        {
            int totalenemy = 3;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0])
            {
                for (int i = 0; i < 1; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000);
                    basics[i].destination = new Vector3(0, 0, -7000);
                }

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl3Update()
        {
            int totalenemy = 4;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0])
            {
                for (int i = 0; i < 3; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000);
                    basics[i].destination = new Vector3(-4000 + 4000*i, 0, -2000 -5000*(float)Math.Pow(-1,i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl4Update()
        {
            int totalenemy = 6;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0] && ClearCheck())
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000*(float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000*(float)Math.Pow(-1, i), 0, -7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
            }

            if(waves[1] && ClearCheck())
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
                waves[1] = false;

            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl5Update()
        {
            if(songSwitch)
            {
                MediaPlayer.Play(bossMusic);
                songSwitch = false;
            }
            int totalenemy = 1;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0])
            {
                for (int i = 0; i < 1; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Scale *= 3f;
                    basics[i].maxHP = 30;
                    basics[i].shotMod = 0.1f;
                    basics[i].isBoss = true;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000);
                    basics[i].destination = new Vector3(0, 0, 0);
                }

                for (int i = 0; i < 0; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(-4000 + (8000 * i), 0, -12000);
                }

                waves[0] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl6Update()
        {
            if (songSwitch)
            {
                MediaPlayer.Play(stage2Music);
                songSwitch = false;
            }
            int totalenemy = 5;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, -7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
            }

            if (waves[1] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
            }

            if (waves[2] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
            }

            if (waves[3] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
            }

            if (waves[4] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
                waves[4] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl7Update()
        {
            int totalenemy = 10;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, -7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
            }

            if (waves[1] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
            }

            if (waves[2] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 3; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
            }

            if (waves[3] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, 12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 4; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl8Update()
        {
            int totalenemy = 15;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0] && ClearCheck())
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, -7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
            }

            if (waves[1] && ClearCheck())
            {
                for (int i = 0; i < 3; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000);
                    basics[i].destination = new Vector3(-4000 + 4000 * i, 0, -2000 - 5000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
            }

            if (waves[2] && ClearCheck())
            {
                for (int i = 0; i < 4; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                }
                basics[0].destination = new Vector3(-4000, 0, 7000);
                basics[1].destination = new Vector3(4000, 0, -7000);
                basics[2].destination = new Vector3(4000, 0, 7000);
                basics[3].destination = new Vector3(-4000, 0, -7000);

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl9Update()
        {
            int totalenemy = 16;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
            if (waves[0] && ClearCheck())
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].isBoss = true;                 
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(-4000 * (float)Math.Pow(-1, i), 0, -7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
            }

            if (waves[1] && ClearCheck())
            {
                for (int i = 0; i < 2; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].isBoss = true;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                    basics[i].destination = new Vector3(4000 * (float)Math.Pow(-1, i), 0, 7000 * (float)Math.Pow(-1, i));
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
            }

            if (waves[2] && ClearCheck())
            {
                for (int i = 0; i < 0; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                }
                basics[0].destination = new Vector3(-4000, 0, 7000);
                basics[1].destination = new Vector3(4000, 0, -7000);
                basics[2].destination = new Vector3(4000, 0, 7000);
                basics[3].destination = new Vector3(-4000, 0, -7000);

                for (int i = 0; i < 6; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
            }

            if (waves[3] && ClearCheck())
            {
                for (int i = 0; i < 4; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].isBoss = true;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000 * (float)Math.Pow(-1, i));
                }
                basics[0].destination = new Vector3(-4000, 0, 7000);
                basics[1].destination = new Vector3(4000, 0, -7000);
                basics[2].destination = new Vector3(4000, 0, 7000);
                basics[3].destination = new Vector3(-4000, 0, -7000);

                for (int i = 0; i < 0; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000 * (float)Math.Pow(-1, i), 0, random.Next(-16000, 16000));
                }

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void lvl10Update()
        {
            if (songSwitch)
            {
                MediaPlayer.Play(bossMusic);
                songSwitch = false;
            }

            int totalenemy = 16;
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];


            bool load = true;
            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    load = false;


            if (waves[0] && load)
            {
                for (int i = 0; i < 1; i++)
                {
                    basics[i].isActive = true;
                    basics[i].hits = 0;
                    basics[i].Transform.Scale *= 3f;
                    basics[i].maxHP = 30;
                    basics[i].shotMod = 0.1f;
                    basics[i].isBoss = true;
                    basics[i].Transform.Position = new Vector3(0, 0, -12000);
                    basics[i].destination = new Vector3(0, 0, 0);
                }

                for (int i = 0; i < 1; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000, 0, random.Next(-16000, 16000));
                }

                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
            }

            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    load = false;

            if (waves[1] && load)
            {

                for (int i = 0; i < 2; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
            }

            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    load = false;

            if (waves[2] && load)
            {
                for (int i = 0; i < 3; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(-12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
            }

            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    load = false;

            if (waves[3] && load)
            {
                for (int i = 0; i < 4; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
            }

            for (int i = 0; i < circles.Length; i++)
                if (circles[i].isActive)
                    load = false;

            if (waves[4] && load)
            {
                for (int i = 0; i < 5; i++)
                {
                    circles[i].isActive = true;
                    circles[i].hits = 0;
                    circles[i].Transform.Position = new Vector3(12000, 0, random.Next(-16000, 16000));
                }
                heart.isActive = true;
                heart.Transform.Position = new Vector3(random.Next(-4000, 4000), 0, -12000);

                waves[0] = false;
                waves[1] = false;
                waves[2] = false;
                waves[3] = false;
                waves[4] = false;
            }

            if ((int)Time.TotalGameTime.TotalSeconds > time && totalenemy > destroyed)
            {
                time = (int)Time.TotalGameTime.TotalSeconds;
                score--;
            }

            if (destroyed >= totalenemy)
            {
                soundInstance = lvlclear.CreateInstance();
                soundInstance.Play();
                currentScene = scenes["Score"];
            }
        }

        void PlayDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw Background
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            spriteBatch.End();

            

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);


            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //particle draw
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(
            Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);
            particleManager.Draw(GraphicsDevice);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;



            if (player.isActive)
            {                
                for (int i = 0; i < GameConstants.NumBullets; i++)
                    bulletList[i].Draw();
                player.Draw();
            }

            if (heart.isActive)
                heart.Draw();
            if (big.isActive)
                big.Draw();
            if (spread.isActive)
                spread.Draw();

            foreach (CircleEnemy circleEnemy in circles)
            {
                if (circleEnemy.isActive)
                {
                    foreach (Bullet bullets in circleEnemy.bulletList)
                    {
                        bullets.Draw();
                    }
                    circleEnemy.Draw();
                }
            }
            foreach (BasicEnemy basicEnemy in basics)
            {
                if (basicEnemy.isActive)
                {
                    foreach (Bullet bullets in basicEnemy.bulletList)
                    {
                        bullets.Draw();
                    }
                    basicEnemy.Draw();
                }
            }


            //GUI Bars
            spriteBatch.Begin();
            spriteBatch.Draw(health, new Rectangle(50, ScreenManager.Height - 70, 130, 20), Color.White);
            spriteBatch.DrawString(Font, "SHIP HEALTH", new Vector2(55, ScreenManager.Height - 67), Color.Red);
            spriteBatch.Draw(health, new Rectangle(50, ScreenManager.Height - 50, 300, 30), Color.White);
            spriteBatch.Draw(health, new Rectangle(52, ScreenManager.Height - 48, (int)(296 * (1 - (player.hits / player.maxHP))), 26), Color.Red);

            //Score
            spriteBatch.Draw(health, new Rectangle(ScreenManager.Width/2 - 50, ScreenManager.Height - 70, 100, 50), Color.White);
            spriteBatch.DrawString(Font, score.ToString(), new Vector2(ScreenManager.Width /2 - Font.MeasureString(score.ToString()).X, ScreenManager.Height - 63), Color.Blue, 0, Vector2.Zero, 2f, spriteEffect, 0);

            //Powerup Sidebar
            if (player.big || player.spread)
                spriteBatch.Draw(health, new Rectangle(5, 100, 20, 1000), Color.White);
            if (player.big)
                spriteBatch.Draw(health, new Rectangle(
                    7,
                    98 + (int)(996 * barPercent),
                    16,
                    996 - (int)(996 * barPercent)), Color.GreenYellow);
            if (player.spread)
                spriteBatch.Draw(health, new Rectangle(
                    7,
                    98 + (int)(996 * barPercent),
                    16,
                    996 - (int)(996 * barPercent)), Color.MediumPurple);

            spriteBatch.Draw(health, new Rectangle(720, ScreenManager.Height - 70, 130, 20), Color.White);
            spriteBatch.DrawString(Font, "SHIP SPEED", new Vector2(745, ScreenManager.Height - 67), Color.Green);
            spriteBatch.Draw(health, new Rectangle(550, ScreenManager.Height - 50, 300, 30), Color.White);
            spriteBatch.Draw(health, new Rectangle(
                (552 + (int)(296 * (1 - (player.Rigidbody.Velocity.Length() / GameConstants.MaxSpeed)))) < 552 ? 552 : 552 + (int)(296 * (1 - (player.Rigidbody.Velocity.Length() / GameConstants.MaxSpeed))),
                ScreenManager.Height - 48,
                (296 - (int)(296 * (1 - (player.Rigidbody.Velocity.Length() / GameConstants.MaxSpeed)))) > 296 ? 296 : 296 - (int)(296 * (1 - (player.Rigidbody.Velocity.Length() / GameConstants.MaxSpeed))),
                26), Color.Green);
            spriteBatch.End();

        }
    }
}
