using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    //Dungeon generator: https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
    enum Screen
    {
        intro,
        load,
        game,
        exit
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Character player;
        Screen screen;
        List<Background> backgrounds = new List<Background> ();

        Map map;
        string loadTxt = "Click to Start";
        Button button;
        UserInterface userInterface;
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
            button = new(new(Globals.WindowSize.X/2, Globals.WindowSize.Y/2), loadTxt);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.SpriteBatch = _spriteBatch;
            // TODO: use this.Content to load your game content here
            Globals.Font = Content.Load<SpriteFont>("spritefont");
            Background mountain = new(Content.Load<Texture2D>("MountainBackground"),640,320,100,0,0.2f);
            backgrounds.Add(mountain);
            Background forest = new(Content.Load<Texture2D>("Trees"), 640, 240, 0, 0, 0.3f);
            backgrounds.Add(forest);
            Background cloud = new(Content.Load<Texture2D>("Cloud"), 640, 250, 480, 1, 0.4f);
            backgrounds.Add(cloud);
        }

        protected void GameRun()
        {
            player = new(Globals.Content.Load<Texture2D>("character"),
         Globals.Content.Load<Texture2D>("character2 (1)"),
         new(300,300));
            userInterface = new UserInterface();
            this.Window.Title = "about to load file";
            map = new();
            this.Window.Title = "file loaded";
            map.UserInterface = userInterface;
            map.Player = player;
            screen = Screen.game;
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
                button.Update();
                if (button.State == Button.ButtonStates.Pressed)
                {
                    button.Text = "Loading...";
                    screen = Screen.load;
                }
                foreach (Background background in backgrounds)
                {
                    background.Update(0f);
                }
            }
            else if (screen == Screen.load)
            {
                GameRun();
            }
            else if (screen == Screen.game)
            {

                player.Update(map);
                userInterface.Update(map);
                foreach (Background background in backgrounds)
                {
                    background.Update(player.MapDisplacement.X);
            }
                if (map.UserInterface.open)
                {
                    player.CanMove = false;
                }
                else if (!map.UserInterface.open)
                {
                    player.CanMove = true;
                }
            }
            this.Window.Title = _graphics.PreferredBackBufferWidth.ToString() +" "+ Globals.WindowSize.X;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.Device = GraphicsDevice;
            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            foreach (Background background in backgrounds)
            {
                background.Draw();
            }
            if (screen == Screen.intro)
            {
                button.Draw();
            }
            else if (screen == Screen.load)
            {
                button.Draw();
            }
            else if (screen == Screen.game)
            {

                map.Draw();
                player.Draw();
                userInterface.Draw();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}