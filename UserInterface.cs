

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
        private Button _left;
        private Button _right;
        public Image Soil, Enemy, Treasure,Portal,Scorpion,Platform,Ladder;
        public List<Image> Images = new List<Image>();
        public UserInterface()
        {
            MouseState = null;
            Settingrect = Rectangle(40,40, new(600, 600));
            UIrect = Rectangle(0,0, Origin);
            Menurect = Rectangle(0,0, Origin);
            NewGameBtn = new(Origin - new Vector2(0, Globals.WindowSize.Y/640*200), "New Game");
            Buttons.Add(NewGameBtn);
            EditLevelBtn = new(NewGameBtn.Position + new Vector2(0, NewGameBtn.Height*4/3), "Edit Level");
            Buttons.Add(EditLevelBtn);
            SaveEditBtn = new(NewGameBtn.Position + new Vector2(0, NewGameBtn.Height * 4 / 3*2), "Save Edit");
            Buttons.Add(SaveEditBtn);
            LevelBtn = new(NewGameBtn.Position + new Vector2(0, NewGameBtn.Height * 4 / 3*3), "Level  "+ Globals.Level);
            Buttons.Add(LevelBtn);
            GravityBtn = new(NewGameBtn.Position + new Vector2(0, NewGameBtn.Height * 4 / 3*4), "Gravity:  " + Globals.Gravity);
            Buttons.Add(GravityBtn);

            SetImages();
            _left = new Button(new(Origin.X - 250, 570), "<<<<<");
            _right = new Button(new(Origin.X + 250, 570), ">>>>>");
            PreviousLevel = 1;
        }
        private void SetImages()
        {
            Images.Clear();
            int side = Globals.TileSize;
            Soil = new(Globals.Content.Load<Texture2D>("Soil"), Rectangle(side, side, new Vector2(Origin.X - 150, 570)), SpriteEffects.None);
            Enemy = new(Globals.Content.Load<Texture2D>("Dog1"), Rectangle(side, side, new Vector2(Origin.X - 50, 570)), SpriteEffects.None);
            Treasure = new(Globals.Content.Load<Texture2D>("treasure"), Rectangle(side, side, new Vector2(Origin.X + 50, 570)), SpriteEffects.None);
            Portal = new(Globals.Content.Load<Texture2D>("portal"), Rectangle(side, side, new Vector2(Origin.X + 150, 570)), SpriteEffects.None);
            Scorpion = new(Globals.Content.Load<Texture2D>("scorpion"), Rectangle(side, side, new Vector2(Origin.X + 250, 570)), SpriteEffects.None);
            Platform = new(Globals.Content.Load<Texture2D>("platform"), Rectangle(side, side, new Vector2(Origin.X + 350, 570)), SpriteEffects.None);
            Ladder = new(Globals.Content.Load<Texture2D>("ladder"), Rectangle(side, side, new Vector2(Origin.X + 450, 570)), SpriteEffects.None);
            Images.Add(Enemy); Images.Add(Treasure); Images.Add(Portal); Images.Add(Soil); Images.Add(Scorpion);Images.Add(Platform);Images.Add(Ladder);
        }
        public int PreviousLevel { get; set; }
        public void Update(Map map)
        {
            foreach (Button button in Buttons)
                button.Update();
            _left.Update();
            _right.Update();
            if (InputManager.IsKeyClicked(Keys.E))
            {
                foreach (var portal in map.Portals)
                {
                    if (map.Player.Hitbox(map.Player.Position).Intersects(portal.Rectangle))
                    {
                        Globals.Level++;
                        if (Globals.Level >= 5)
                        {
                            Globals.Level = 1;
                        }
                        LevelBtn.Text = "level " + Globals.Level;
                        map.NewGame();
                        
                        PreviousLevel = Globals.Level;
                        break;
                    }
                }
            }
            if (open)
            {
                if (InputManager.MouseClicked &&
GravityBtn.Rectangle(GravityBtn.Width, GravityBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    Globals.Gravity -= 0.1f;
                    if (Globals.Gravity <0.1f)
                    {
                        Globals.Gravity = 2;
                    }
                    GravityBtn.Text = "Gravity:  " + Math.Round(Globals.Gravity ,2);
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
                    Globals.Pause = false;
                    open = false;
                    UIrect = Rectangle(0, 0, Origin);
                    EditOpen = true;
                    SetImages();
                    Menurect = Rectangle(580, 100, new(Origin.X, 570));
                }
                if (InputManager.MouseClicked &&
        NewGameBtn.Rectangle(NewGameBtn.Width, NewGameBtn.Height).Contains(InputManager.MouseRectangle))
                {
                    Globals.Pause = false;
                    open = false;
                    UIrect = Rectangle(0, 0, Origin);
                    map.NewGame();
                    map.Revive();
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
                                foreach (var portal in map.Portals)
                                {
                                    if (empty &&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)portal.Position.X, (int)portal.Position.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('4');
                                        continue;
                                    }
                                }
                                foreach (var scorpion in map.Scorpions)
                                {
                                    if (empty &&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)scorpion.Position.X, (int)scorpion.Position.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('5');
                                        continue;
                                    }
                                }
                                foreach (var item in map.Platforms)
                                {
                                    if (empty &&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)item.Position.X, (int)item.Position.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('6');
                                        continue;
                                    }
                                }
                                foreach (var item in map.Ladders)
                                {
                                    if (empty &&
                                        map.Tiles[y, x].Rectangle.Contains(new Rectangle((int)item.Position.X, (int)item.Position.Y, 1, 1)))
                                    {
                                        empty = false;
                                        writer.Write('7');
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
                    Globals.Pause = false;
                    open = false;
                    UIrect = Rectangle(0,0, Origin);
                }
                else
                {
                    Globals.Pause = true;
                    map.Player.MapDisplacement = Vector2.Zero;
                    open = true;
                    SaveEditBtn.Text = "Save Edit";
                    Globals.Level = PreviousLevel;
                    LevelBtn.Text = "level" + Globals.Level;
                    UIrect = Rectangle(320, 480, Origin);
                }
            }
            if (InputManager.MouseClicked && EditOpen)
            {
                if (InputManager.MouseClicked &&
        _left.Rectangle(_left.Width, _left.Height).Contains(InputManager.MouseRectangle))
                {
                    foreach (Image img in Images)
                    {
                        img.X += 100;
                    }
                }
                if (InputManager.MouseClicked &&
_right.Rectangle(_left.Width, _left.Height).Contains(InputManager.MouseRectangle))
                {
                    foreach (Image img in Images)
                    {
                        img.X -= 100;
                    }
                }
                if (Soil.Rectangle.Contains(InputManager.MouseRectangle)&&Editable(Soil))
                {
                    MouseState = "soil";
                }
                else if (Enemy.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Enemy))
                {
                    MouseState = "enemy";
                }
                else if (Treasure.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Treasure))
                {
                    MouseState = "treasure";
                }
                else if (Portal.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Portal))
                {
                    MouseState = "portal";
                }
                else if (Scorpion.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Scorpion))
                {
                    MouseState = "scorpion";
                }
                else if (Platform.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Platform))
                {
                    MouseState = "platform";
                }
                else if (Ladder.Rectangle.Contains(InputManager.MouseRectangle) && Editable(Ladder))
                {
                    MouseState = "ladder";
                }
            }
        }
        private bool Editable(Image image)
        {
            return image.Position.X <= Origin.X + 110 && image.Position.X >= Origin.X - 190;
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
                foreach (var image in Images)
                {
                    if (Editable(image))
                        image.Draw();
                }
                _left.Draw();
                _right.Draw();
            }
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("setting"), Settingrect, Color.White);
        }
    }
}
