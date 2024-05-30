
namespace Platformer
{
    public class UserInterface
    {
        public string MouseState { get; set; }
        private Texture2D _tex = Globals.Content.Load<Texture2D>("rectangle");
        public Rectangle UIrect;
        public Rectangle Settingrect;
        public Rectangle Menurect;
        public bool open = false;
        public Vector2 Origin = new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2);
        public Rectangle Rectangle(int width, int height,Vector2 origin)
        {
            return new Rectangle((int)origin.X - width / 2, (int)origin.Y - height / 2, width, height);
        }
        public List<Button> Buttons = new List<Button>();
        public Button NewGameBtn;
        public Button EditLevelBtn;
        public Button SaveEditBtn;
        public bool EditOpen = false;
        public Rectangle Soil, Enemy, Treasure;
        public UserInterface()
        {
            MouseState = null;
            Settingrect = Rectangle(40,40, new(600, 600));
            UIrect = Rectangle(0,0, Origin);
            Menurect = Rectangle(0,0, Origin);
            NewGameBtn = new(Origin - new Vector2(0, 200), "New Game");
            Buttons.Add(NewGameBtn);
            EditLevelBtn =  new(Origin - new Vector2(0, 160), "Edit Level");
            Buttons.Add(EditLevelBtn);
            SaveEditBtn = new(Origin - new Vector2(0, 120), "Save Edit");
            Buttons.Add(SaveEditBtn);
            Soil = Rectangle(80, 80, new Vector2(Origin.X - 150, 550));
            Enemy = Rectangle(80, 80, new Vector2(Origin.X - 50, 550));
            Treasure = Rectangle(60,45, new Vector2(Origin.X + 50, 550));
        }
        public void Update()
        {
            foreach (Button button in Buttons)
                button.Update();
            if (InputManager.MouseClicked && 
                EditLevelBtn.Rectangle(EditLevelBtn.Width, EditLevelBtn.Height).Contains(InputManager.MouseRectangle))
            {
                open = false;
                UIrect = Rectangle(0, 0, Origin);
                EditOpen = true;
                Menurect = Rectangle(400, 100, new(Origin.X,550));
            }
            if (InputManager.MouseClicked &&
    SaveEditBtn.Rectangle(SaveEditBtn.Width, SaveEditBtn.Height).Contains(InputManager.MouseRectangle))
            {
                SaveEditBtn.Text = "Saved";
                EditOpen = false;
                Menurect  = Rectangle(0, 0, Origin);
                MouseState = null;
            }
            if (InputManager.MouseClicked && Settingrect.Contains(InputManager.MouseRectangle))
            {
                if (open)
                {
                    open = false;
                    UIrect = Rectangle(0,0, Origin);
                }
                else
                {
                    open = true;
                    SaveEditBtn.Text = "Save Edit";
                    UIrect = Rectangle(320, 480, Origin);
                }
            }
            if (InputManager.MouseClicked && EditOpen)
            {
                if (Soil.Contains(InputManager.MouseRectangle))
                {
                    MouseState = "soil";
                }
                else if (Enemy.Contains(InputManager.MouseRectangle))
                {
                    MouseState = "enemy";
                }
                else if (Treasure.Contains(InputManager.MouseRectangle))
                {
                    MouseState = "treasure";
                }
            }
        }
        public void Draw()
        {
            if (open)
            {
                Globals.SpriteBatch.Draw(_tex, UIrect, Color.SaddleBrown);
                Rectangle rect = Rectangle(UIrect.Width - UIrect.Height / 20, UIrect.Height - UIrect.Height / 20, Origin);
                Globals.SpriteBatch.Draw(_tex, rect, Color.SandyBrown);
                foreach (Button button in Buttons)
                    button.Draw();

            }
            if (EditOpen)
            {
                Globals.SpriteBatch.Draw(_tex, Menurect, Color.SaddleBrown);
                Rectangle rect = Rectangle(Menurect.Width - Menurect.Height / 20, Menurect.Height - Menurect.Height / 20, new(Origin.X, 550));
                Globals.SpriteBatch.Draw(_tex, rect, Color.SandyBrown);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Soil"), Soil, Color.White);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Enemy1"), Enemy, Color.White);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), Treasure, Color.White);
            }
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("setting"), Settingrect, Color.White);
        }
    }
}
