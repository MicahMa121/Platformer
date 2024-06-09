
using System;
using System.IO;

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
        public Button LevelBtn;
        public Button GravityBtn; 
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
            EditLevelBtn =  new(Origin - new Vector2(0, 80), "Edit Level");
            Buttons.Add(EditLevelBtn);
            SaveEditBtn = new(Origin - new Vector2(0, 120), "Save Edit");
            Buttons.Add(SaveEditBtn);
            LevelBtn = new(Origin - new Vector2(0, 160), "Level  "+ Globals.Level);
            Buttons.Add(LevelBtn);
            GravityBtn = new(Origin - new Vector2(0, 40), "Gravity:  " + Globals.Gravity*2);
            Buttons.Add(GravityBtn);
            Soil = Rectangle(80, 80, new Vector2(Origin.X - 150, 570));
            Enemy = Rectangle(80, 80, new Vector2(Origin.X - 50, 570));
            Treasure = Rectangle(60,45, new Vector2(Origin.X + 50, 570));
            PreviousLevel = 1;
        }
        public int PreviousLevel { get; set; }
        public void Update(Map map)
        {
            foreach (Button button in Buttons)
                button.Update();
            if (open)
            {
                if (InputManager.MouseClicked &&
GravityBtn.Rectangle(GravityBtn.Width, GravityBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    Globals.Gravity -= 0.25f;
                    if (Globals.Gravity < 0.5)
                    {
                        Globals.Gravity = 2;
                    }
                    GravityBtn.Text = "Gravity:  " + Math.Round(Globals.Gravity * 2,2);
                }
                if (InputManager.MouseClicked &&
LevelBtn.Rectangle(LevelBtn.Width, LevelBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    Globals.Level++;
                    if (Globals.Level >= 5)
                    {
                        Globals.Level = 1;
                    }
                    LevelBtn.Text = "level " + Globals.Level;
                }
                if (InputManager.MouseClicked &&
    EditLevelBtn.Rectangle(EditLevelBtn.Width, EditLevelBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    open = false;
                    UIrect = Rectangle(0, 0, Origin);
                    EditOpen = true;
                    Menurect = Rectangle(400, 100, new(Origin.X, 570));
                }
                if (InputManager.MouseClicked &&
        NewGameBtn.Rectangle(NewGameBtn.Width, NewGameBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    open = false;
                    UIrect = Rectangle(0, 0, Origin);
                    map.NewGame();
                    map.Player.Health = 80;
                    PreviousLevel = Globals.Level;
                }
                if (InputManager.MouseClicked &&
        SaveEditBtn.Rectangle(SaveEditBtn.Width, SaveEditBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    SaveEditBtn.Text = "Saved";
                    EditOpen = false;
                    Menurect = Rectangle(0, 0, Origin);
                    MouseState = null;
                    StreamWriter writer = File.CreateText(@"level" + Globals.Level + ".txt");
                    for (int y = 0; y < map.Tiles.GetLength(0); y++)
                    {
                        for (int x = 0; x < map.Tiles.GetLength(1); x++)
                        {
                            bool empty = true;
                            if (map.Tiles[y, x].Visible)
                            {
                                writer.Write('1');
                                empty = false;
                            }
                            else
                            {
                                foreach (Enemy enemy in map.Enemies)
                                {
                                    if (empty&&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)enemy.Rectangle.Center.X, (int)enemy.Rectangle.Center.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('2');
                                        continue;
                                    }
                                }
                                foreach (Treasure treasure in map.Treasures)
                                {
                                    if (empty&&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)treasure.Position.X, (int)treasure.Position.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('3');
                                        continue;
                                    }
                                }
                            }
                            if (empty)
                                writer.Write('0');
                        }
                        writer.WriteLine();
                    }
                    writer.Close();
                }
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
                    Globals.Level = PreviousLevel;
                    LevelBtn.Text = "level" + Globals.Level;
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
                Rectangle rect = Rectangle(Menurect.Width - Menurect.Height / 20, Menurect.Height - Menurect.Height / 20, new(Origin.X, 570));
                Globals.SpriteBatch.Draw(_tex, rect, Color.SandyBrown);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Soil"), Soil, Color.White);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Dog1"), Enemy, Color.White);
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), Treasure, Color.White);
            }
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("setting"), Settingrect, Color.White);
        }
    }
}
