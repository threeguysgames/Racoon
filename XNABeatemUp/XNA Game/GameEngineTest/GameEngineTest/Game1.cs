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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TextLib;

namespace GameEngineTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D mainTarget;

        Character[] characters = new Character[12];

        Texture2D backTexture;

        Map map;
        Texture2D[] mapsText = new Texture2D[1];
        Texture2D[] character = new Texture2D[16];
        public static CharDef[] charDef = new CharDef[16];

        public static float frameTime = 0f;
        public static float realTime = 0f;
        public static Vector2 scroll = new Vector2();

        public const float gravity = 900f;
        public const float friction = 1300f;
        public static float slowTime = 0f;

        public static ParticleManager pManager;
        Texture2D spritesTex;

        Effect negative;

        Text text;

        public static NumberDraw numDraw;

        public enum GameModes : int
        {
            Menu = 0, 
            Playing = 1
        }

        private static Menu menu;
        private static long score = 0;
        private static GameModes gameMode;

        public static GameModes GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }

        }

        public static Menu Menu
        {
            get { return menu; }
            set { menu = value; }
        }

        public static long Score
        {
            get { return score; }
            set { score = value; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        protected override void Initialize()
        {

            map = new Map();
            map.Path = "maps/map";

            map.Read();

            charDef[2] = new CharDef("lizard");
            charDef[1] = new CharDef("orc");
            charDef[0] = new CharDef("knight");

            Character.loadTextures(Content);

            characters[0] = new Character(charDef[0],   new Vector3(500, 1000, 0),   0, TeamName.goodGuys);
            characters[1] = new Character(charDef[0], new Vector3(500, 1100, 0), 1, TeamName.goodGuys);

            characters[0].Name = "Sir Awesome";
            characters[1].Name = "Sir AI";

            characters[1].setRocketAI();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainTarget = new RenderTarget2D(GraphicsDevice,
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                1,
                SurfaceFormat.Color);

            map.iconsTex = Content.Load<Texture2D>(@"gfx/icons");        
            
            pManager = new ParticleManager(spriteBatch);
            spritesTex = Content.Load<Texture2D>(@"gfx/sprites");

            numDraw = new NumberDraw();

            //map.iconsTex = Content.Load<Texture2D>(@"gfx/icons");

            for (int i = 0; i < mapsText.Length; i++)
            {
                mapsText[i] = Content.Load<Texture2D>(@"gfx/maps/maps" + (i + 1).ToString());
            }

            backTexture = Content.Load<Texture2D>(@"gfx/background");

            menu = new Menu(Content.Load<Texture2D>(@"gfx/menutex"),
                Content.Load<Texture2D>(@"gfx/menufore"),
                Content.Load<Texture2D>(@"gfx/options"),
                backTexture,
                spritesTex,
                spriteBatch);

            negative = Content.Load<Effect>(@"fx/Negative");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState kState = Keyboard.GetState();

            QuakeManager.Update();

            frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            realTime = frameTime;

            if (slowTime > 0f)
            {
                slowTime -= frameTime;
                frameTime /= 5f;
            }

            switch (gameMode)
            {
                case GameModes.Playing:
                    UpdateGame();
                    break;

                case GameModes.Menu:
                    if (menu.menuMode == Menu.MenuMode.Dead)
                    {
                        float pTime = frameTime;
                        frameTime /= 3f;
                        UpdateGame();

                        frameTime = pTime;
                    }
                    menu.Update(this);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameMode)
            {
                case GameModes.Playing:
                    drawGame();
                    break;
                case GameModes.Menu:

                    if (menu.menuMode == Menu.MenuMode.Pause ||
                        menu.menuMode == Menu.MenuMode.Dead)
                    {

                        drawGame();
                    }
                    menu.Draw();
                    break;
            }

            //numDraw.Draw(spriteBatch, spritesTex, 235423, new Vector2(50,50), Color.White);

            base.Draw(gameTime);
        }

        private void UpdateGame()
        {
            if (characters[0] != null)
            {
                Vector2 charScroll = new Vector2(characters[0].position.X, characters[0].position.Y);

                scroll += ((charScroll - new Vector2(400f, 400f)) - scroll) * realTime * 20f;
                scroll += QuakeManager.Quake.Vector;


                characters[0].processInput();
            }

            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    characters[i].update(map, pManager, characters);

                    if (characters[i].DyingFrame > 1f)
                    {
                        if (characters[i].TEAM == TeamName.goodGuys && i == 0)
                            characters[i].DyingFrame = 1f;
                        else
                        {
                            if (characters[i].Name != "")
                                map.mapScript.Flags.SetFlag(characters[i].Name);
                            characters[i] = null;
                        }
                    }
                }
            }

            pManager.UpdateParticles(frameTime, map, characters);

            foreach (Character c in characters)
            {
                if (c != null)
                {
                    bool flag1 = map.Ledges[0].isInside(c.shadowPosition);
                    bool flag2 = map.Ledges[0].isInside(c.prevShadowPosition);

                    if (flag2 && !flag1)
                    {
                        //Console.WriteLine("Stepped out ");
                        c.trajectory = -c.trajectory;
                    }
                }
            }

            map.Update(pManager, characters);
        }
        private void drawGame()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.SetRenderTarget(0, mainTarget);
            graphics.GraphicsDevice.Clear(Color.Black);

            drawBackground();
            map.Draw(spriteBatch, mapsText, 0, 2);
            pManager.DrawParticles(spritesTex, true);
            drawCharacter();


            pManager.DrawParticles(spritesTex, false);

            map.Draw(spriteBatch, mapsText, 2, 3);


            graphics.GraphicsDevice.SetRenderTarget(0, null);

            if (gameMode == GameModes.Menu)
            {
                negative.Begin();

                spriteBatch.Begin(SpriteBlendMode.None,
                    SpriteSortMode.Immediate,
                    SaveStateMode.SaveState);

                EffectPass pass = negative.CurrentTechnique.Passes[0];
                pass.Begin();
                spriteBatch.Draw(mainTarget.GetTexture(),
                    new Vector2(), Color.White);
                pass.End();
                spriteBatch.End();
                negative.End();
            }
            else
            {
                spriteBatch.Begin(SpriteBlendMode.None);
                spriteBatch.Draw(mainTarget.GetTexture(),
                    new Vector2(), Color.White);

                spriteBatch.End();
            }
            drawQuakes();
          
        }
        private void drawCharacter()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.SaveState);
            foreach (Character c in characters)
            {
                if (c != null)
                {
                    
                    //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                    c.draw(spriteBatch); 
                    
                }
            }
            spriteBatch.End();
        }
        private void drawBackground()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backTexture, Vector2.Zero,
            new Rectangle(0, 0, 800, 600),
            Color.White, 0.0f,
            new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
        private void drawQuakes()
        {
            if (QuakeManager.Blast.Value > 0f)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(
                        mainTarget.GetTexture(),
                        QuakeManager.Blast.Center - scroll,
                        new Rectangle(0, 0, 800, 00),
                        new Color(new Vector4(1, 1, 1, .35f * (QuakeManager.Blast.Magnitude))),
                        0f,
                        QuakeManager.Blast.Center - scroll,
                        (1.0f + (QuakeManager.Blast.Magnitude - QuakeManager.Blast.Value) * .1f + ((float)(i + 1) / 100f)),
                        SpriteEffects.None,
                        1f);

                }
                spriteBatch.End();

            }
        }

        public static float SlowTime
        {
            get { return slowTime; }
            set { slowTime = value; }
        }

        public void NewGame()
        {
            gameMode = GameModes.Playing;

            characters[0] = new Character(charDef[0], new Vector3(100f, 100f, 0f), 0, TeamName.goodGuys);
            for (int i = 1; i < characters.Length; i++)
                characters[i] = null;

            pManager.Reset();
            map.Path = "maps/map";
            map.GlobalFlags = new MapFlags(64);
            map.Read();
            map.transDir = TransitionDirection.Intro;
            map.transInFrame = 1f;

        }

        public  void Quit()
        {
            this.Exit();
        }
    }
}
