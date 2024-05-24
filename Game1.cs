using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    //Dungeon generator: https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
    enum Screen
    {
        intro,
        game,
        exit
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Character player;
        Screen screen;

        Map map;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Globals.WindowSize = new(640, 640);
            _graphics.PreferredBackBufferWidth = Globals.WindowSize.X;
            _graphics.PreferredBackBufferHeight = Globals.WindowSize.Y;
            _graphics.ApplyChanges();

            Globals.Content = Content;

            screen = Screen.intro;

            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.SpriteBatch = _spriteBatch;
            // TODO: use this.Content to load your game content here

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Update(gameTime);
            // TODO: Add your update logic here
            InputManager.Update();
            if (screen == Screen.intro)
            {
                if (InputManager.IsKeyClicked(Keys.W))
                {
                    player = new(Globals.Content.Load<Texture2D>("character"),
                             Globals.Content.Load<Texture2D>("character2 (1)"),
                             new(_graphics.PreferredBackBufferWidth/2-40,_graphics.PreferredBackBufferHeight/2-40));
                    map = new();
                    screen = Screen.game;
                }
            }
            else if (screen == Screen.game)
            {
                player.Update(map);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.Device = GraphicsDevice;
            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            if (screen == Screen.intro)
            {

            }
            else if (screen == Screen.game)
            {
                map.Draw();
                player.Draw();
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}