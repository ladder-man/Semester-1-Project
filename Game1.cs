using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace jumpThing2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D backgroundTxr, playerSheetTxr, platformSheetTxr, whiteBox;
        SpriteFont uiTextFont, heartFont;
        SoundEffect jumpSound, bumpSound, fanfareSound;

        Point screenSize = new Point(800, 450);
        int levelNumber = 0;
        PlayerSprite playerSprite;
        PageSprite pageSprite;

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>();
        List<Vector2> pages = new List<Vector2>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTxr = Content.Load<Texture2D>("JumpThing_background");
            playerSheetTxr = Content.Load<Texture2D>("JumpThing_spriteSheet1");
            platformSheetTxr = Content.Load<Texture2D>("JumpThing_spriteSheet2");
            uiTextFont = Content.Load<SpriteFont>("Cutesy");
            heartFont = Content.Load<SpriteFont>("Block");
            jumpSound = Content.Load<SoundEffect>("jump");
            bumpSound = Content.Load<SoundEffect>("bump");
            fanfareSound = Content.Load<SoundEffect>("fanfare");

            whiteBox = new Texture2D(GraphicsDevice, 1, 1);
            whiteBox.SetData(new[] { Color.White });

            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(100, 50), jumpSound, bumpSound);
            pageSprite = new PageSprite(playerSheetTxr, whiteBox, new Vector2(200, 200));
            BuildLevels();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            playerSprite.Update(gameTime, levels[levelNumber]);
            if (playerSprite.spritePos.Y > screenSize.Y + 50)
            {
                playerSprite.lives--;
                if (playerSprite.lives <= 0)
                {
                    playerSprite.lives = 3;
                    levelNumber = 0;
                }
                playerSprite.ResetPlayer(new Vector2(100, 50));
            }

            if (playerSprite.CheckCollision(pageSprite))
            {
                levelNumber++;
                if (levelNumber >= levels.Count) levelNumber = 0;
                pageSprite.spritePos = pages[levelNumber];
                playerSprite.ResetPlayer(new Vector2(100, 50));
                fanfareSound.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);

            playerSprite.Draw(_spriteBatch, gameTime);
            pageSprite.Draw(_spriteBatch, gameTime);
            foreach (PlatformSprite platform in levels[levelNumber]) platform.Draw(_spriteBatch, gameTime);

            string livesString = "";
            //for(int i - 0; i < playerSprite.lives; i++) liveString +- "p";

            if (playerSprite.lives == 3) livesString = "111";
            else if (playerSprite.lives == 2) livesString = "11";
            else if (playerSprite.lives == 1) livesString = "1";
            else livesString = "";

            _spriteBatch.DrawString(heartFont, livesString, new Vector2(12, 10), Color.White);

            _spriteBatch.DrawString(
                uiTextFont,
                "level " + (levelNumber + 1),
                new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("level " + (levelNumber + 1)).X, 5),
                Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void BuildLevels()
        {
            levels.Add(new List<PlatformSprite>());
            //floor
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(240, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(335, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(430, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(525, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(620, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(715, 440)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(810, 440)));
            //platforms
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(195, 230)));
            //page
            pages.Add(new Vector2(200, 200));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 400)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 350)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 250)));
            pages.Add(new Vector2(400, 200));
        }
    }
}
