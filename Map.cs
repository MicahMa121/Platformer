
using System;
using System.IO;
using static Platformer.Enemy;

namespace Platformer
{
    public class Map
    {
        public int[,] Map2D()
        {
            int[,] level = new int[102,102];
            
            if (!File.Exists(@"level" + Globals.Level + ".txt"))
            {
                StreamWriter writer = File.CreateText(@"level" + Globals.Level + ".txt");
                for (int j = 0; j < 102; j++)
                {
                    writer.Write('1');
                }
                writer.WriteLine();
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 102; j++)
                    {
                        if ((j==0) || (j == 101))
                        {
                            writer.Write('1');
                        }
                        else
                        {
                            writer.Write('1');
                        }
                    }
                    writer.WriteLine();
                }
                for (int j = 0; j < 102; j++)
                {
                    writer.Write('1');
                }
                writer.Close();

            }
            StreamReader reader = new StreamReader(@"level" + Globals.Level + ".txt");

            for (int i = 0; i < 102; i++)
            {
                for (int j = 0; j < 102; j++)
                {
                    int data = Convert.ToInt32(((char)reader.Read()).ToString());
                    level[i, j] = data;
                }
                reader.ReadLine();
            }
            reader.Close();
            return level;
        }
        private Random _gen = new Random();
        public Tile[,] Tiles;
        private Texture2D _soilTexture = Globals.Content.Load<Texture2D>("grass_block (1)");
        private Texture2D _soil2Texture = Globals.Content.Load<Texture2D>("dirt_block (1)");
        private int tileSize = Globals.TileSize;
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Scorpion> Scorpions = new List<Scorpion>();
        public List<Kitsune> Kitsunes = new List<Kitsune>();
        public UserInterface UserInterface { get; set; } = new UserInterface();
        public bool Clickable()
        {
            return !UserInterface.UIrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Settingrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Menurect.Contains(InputManager.MouseRectangle) &&
                !ShopBtn.Rectangle(ShopBtn.Width, ShopBtn.Height).Contains(InputManager.MouseRectangle)
                ;
        }
        public Vector2 MaptoScreen(int x, int y) => new(x * tileSize, y * tileSize);
        public (int x, int y) ScreentoMap (Vector2 position)=> ((int)position.X/ tileSize, (int)position.Y/ tileSize);
        public Map()
        {

            NewGame();
        }
        public Shop shop { get;set; }
        public void Revive()
        {
            if (Player != null)
            {
                Player.Position = new Vector2(Globals.WindowSize.X/2, Globals.WindowSize.Y/2);
                Player.Atk = UserInterface.atk;
                Player.Health = UserInterface.hp;
                Player.MaxHp = UserInterface.maxhp;
                Player.MaxStamina = UserInterface.stamina;
                Player.SkillZ = UserInterface.skillz;
                Player.SkillX = UserInterface.skillx;
                Player.States = Character.CharacterStates.Idle;
                Player.Death = false;
                Player.Reviving = false;
                Player.Attacking = false;
                Player.SkillAttacking = false;
                Player.Casting = false;
                Player.Jumped = false;
                Player.Hurt = false;
                Player.Enrage = false;
                Player.capples.Clear();
            }
            if (shop != null)
            {
                shop.RefreshShop();
            }
            Money = (int)UserInterface.money;
            ShopBtn = new Button(new(500, 600), "$ " + Money);
        }
        public void NewGame()
        {
            Scorpions.Clear();
            Portals.Clear();
            Coins.Clear();
            Borders.Clear();
            Treasures.Clear();
            Enemies.Clear();
            Platforms.Clear();
            Ladders.Clear();
            Backgrounds.Clear();
            Spikes.Clear();
            Kitsunes.Clear();

            if (Player!= null)
            {
                Player.Position = new Vector2(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2);
            }

            ShopBtn = new Button(new(500, 600), "$ " + Money);
            int[,] map = Map2D();
            AddBorder(2* tileSize, 100 * tileSize);
            Tiles = new Tile[map.GetLength(0), map.GetLength(1)];
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    Tile tile = new(_soilTexture, MaptoScreen(x, y),(x,y));
                    if (map[x, y] == 1)
                    {
                        if (y - 1 >= 0 && map[x, y - 1] == 1)
                        {
                            tile.Texture = _soil2Texture;
                        }
                        tile.Visible = true;
                    }
                    Tiles[x, y] = tile;
                    if (map[x, y] == 2)
                    {
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Dog (2)"), MaptoScreen(x, y));
                        Enemies.Add(enemy);
                    }
                    if (map[x, y] == 3)
                    {
                        Treasure treasure = new(Globals.Content.Load<Texture2D>("treasure"), MaptoScreen(x, y)+new Vector2(Globals.TileSize/2, Globals.TileSize / 2));
                        Treasures.Add(treasure);
                    }
                    if (map[x,y] == 4)
                    {
                        Portal portal = new(Globals.Content.Load<Texture2D>("PortalTex"), MaptoScreen(x, y) + new Vector2(Globals.TileSize / 2, Globals.TileSize / 2));
                        Portals.Add(portal);
                    }
                    if (map[x, y] == 5)
                    {
                        Scorpion scorpion = new(Globals.Content.Load<Texture2D>("ScorpionTex"), MaptoScreen(x, y));
                        Scorpions.Add(scorpion);
                    }
                    if (map[x, y] == 6)
                    {
                        Platform item = new(Globals.Content.Load<Texture2D>("platform"), new((int)MaptoScreen(x, y).X,(int)MaptoScreen(x,y).Y,Globals.TileSize,Globals.TileSize));
                        Platforms.Add(item);
                    }
                    if (map[x, y] == 7)
                    {
                        Platform item = new(Globals.Content.Load<Texture2D>("ladder"), new((int)MaptoScreen(x, y).X, (int)MaptoScreen(x, y).Y, Globals.TileSize, Globals.TileSize));
                        Ladders.Add(item);
                    }
                    if (map[x, y] == 8)
                    {
                        Image item = new(Globals.Content.Load<Texture2D>("cave_wall"), new((int)MaptoScreen(x, y).X, (int)MaptoScreen(x, y).Y, Globals.TileSize, Globals.TileSize), SpriteEffects.None);
                        Backgrounds.Add(item);
                    }
                    if (map[x, y] == 9)
                    {
                        Image item = new(Globals.Content.Load<Texture2D>("spikes"), new((int)MaptoScreen(x, y).X, (int)MaptoScreen(x, y).Y, Globals.TileSize, Globals.TileSize), SpriteEffects.None);
                        Spikes.Add(item);
                    }
                }
            }
        }
        public Character Player { get; set; } 
        public List<Border> Borders { get; set; } = new List<Border> ();
        public int Money { get; set; }
        private void AddBorder(int width, int length)
        {

            Border westwall = new(new Rectangle(tileSize - width, tileSize, width, length));
            Border eastwall = new(new Rectangle(tileSize +length, tileSize, width, length));
            Border northwall = new(new Rectangle(tileSize, tileSize - width, length, width));
            Border southwall = new(new Rectangle(tileSize, tileSize + length, length, width));
            Borders.Add(westwall);
            Borders.Add(eastwall);
            Borders.Add(northwall);
            Borders.Add (southwall);
        }
        public List<Coin> Coins { get; set; } = new List<Coin> ();
        public List<Portal> Portals { get; set; } = new List<Portal> ();
        public List<Image> Trails { get; set; } = new List<Image> ();
        public List<Platform> Platforms { get; set; } = new List<Platform> ();
        public List<Platform> Ladders { get; set; } = new List<Platform>();
        public List<Image> Backgrounds { get; set; } = new List<Image>();
        public List<Image> Spikes { get; set; } = new List<Image>();
        public Button ShopBtn { get; set; } 
        public Button Restart { get; set; }
        public Tutorial tuto { get; set; }
        public void Update(Vector2 displacement)
        {
            if (InputManager.IsKeyClicked(Keys.C))
            {
                Kitsune kitsune = new(new(320, 320));
                Kitsunes.Add(kitsune);
            }
            foreach(var item in Backgrounds)
            {
                item.UpdatePosition(displacement);
            }
            for (int i = 0; i < DamageTexts.Count; i++)
            {
                DamageTexts[i].Update(displacement);
                if (DamageTexts[i].Opacity<= 0f)
                {
                    DamageTexts.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Trails.Count; i++)
            {
                Trails[i].Update();
                if (Trails[i].Opacity <= 0f)
                {
                    Trails.RemoveAt(i);
                    i--;
                }
            }
            if (Player.Health <= 0 && !Player.Reviving)
            {
                Player.Reviving = true;
                Player.MapDisplacement = Vector2.Zero;
                Player.States = Character.CharacterStates.Death;

            }
            if (Player.Death)
            {
                if (Restart == null)
                {
                    Restart = new(new(Globals.WindowSize.X / 2, 0), "Restart?");
                }
                else
                {
                    if (Restart.Position.Y >= Globals.WindowSize.Y/2)
                    {
                        Restart.Position = new(Globals.WindowSize.X / 2,Globals.WindowSize.Y / 2);
                        Restart.Update();
                        if (Restart.ButtonPressed())
                        {
                            NewGame();
                            Revive();
                            if (tuto != null)
                            {
                                tuto.Refresh();
                            }
                            Restart = null;
                            return;
                        }
                    }
                    else
                    {
                        Restart.Position += new Vector2(0,Globals.WindowSize.Y/60);
                    }
                }
            }
            foreach (var portal in Portals)
            {
                portal.Update(displacement);
            }
            foreach(var border in Borders)
            {
                border.Update(displacement);
            }
            foreach (var item in Platforms)
            {
                item.Update(displacement);  
            }
            foreach (var item in Ladders)
            {
                item.Update(displacement);
            }
            foreach (var coin in Coins)
            {
                coin.Update(displacement, Tiles);
                if (!coin.Collected&&coin.Rectangle.Intersects(Player.Hitbox(Player.Position)))
                {
                    coin.Collected = true;
                    Money += coin.Value *5*Globals.Level;
                    ShopBtn.Text = "$ " + Money;

                }
            }
            ShopBtn.Update();
            if (InputManager.IsKeyPressed(Keys.H)&&InputManager.IsKeyPressed(Keys.M ))
            {
                Money += 1000;
                ShopBtn.Text = "$ " + Money;
            }
            for (int i = 0; i < Coins.Count; i++)
            {
                if (Coins[i].Collected && Coins[i].Opacity <= 0f)
                {
                    Coins.RemoveAt(i);
                    i--;
                }
            }
            foreach (var tile in Tiles)
            {
                tile.Position += displacement;
                tile.Rectangle =  new((int)tile.Position.X, (int)tile.Position.Y, tile.Rectangle.Width, tile.Rectangle.Height);
                
            }
            foreach (Treasure treasure in Treasures)
            {
                treasure.Update(displacement, Tiles,Platforms);
                if (!treasure.Opened&&Player.Attacking && Player.AttackRange().Intersects(treasure.Rectangle))
                {
                    treasure.Opened = true;
                    treasure.Time = 0;
                }
                else if (treasure.Opened)
                {
                    if (treasure.Time >= 0.05f)
                    {
                        treasure.Time = 0;
                        treasure.Opacity -= 0.2f;
                        Coin coin = new Coin(Globals.Content.Load<Texture2D>("coin"), treasure.Position, 5*_gen.Next(1,3));
                        Coins.Add(coin);
                    }
                }
            }
            for (int i = 0; i < Treasures.Count; i++)
            {
                if (Treasures[i].Opacity <= 0)
                {
                    Treasures.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].Died)
                {
                    Coin coin = new(Globals.Content.Load<Texture2D>("coin"), new(Enemies[i].Rectangle.Center.X, Enemies[i].Rectangle.Center.Y), 5 * _gen.Next(1, 3));
                    Coins.Add(coin);
                    Enemies.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Kitsunes.Count; i++)
            {
                if (Kitsunes[i].Died)
                {
                    Treasure treasure = new(Globals.Content.Load<Texture2D>("treasure"), new(Kitsunes[i].Hitbox.Center.X, Kitsunes[i].Hitbox.Center.Y));
                    Treasures.Add(treasure);
                    Kitsunes.RemoveAt(i);
                    i--;
                }
            }
            foreach ( var item in Spikes)
            {
                item.UpdatePosition(displacement);
                if (item.Rectangle.Intersects(Player.Hitbox(Player.Position))&&!Player.Hurt)
                {
                    Player.Health -= (int)(Player.MaxHp / 4);
                    DamageText text = new(Convert.ToString((int)(Player.MaxHp / 4)), new(Player.Hitbox(Player.Position).Center.X, Player.Position.Y), Color.Red);
                    DamageTexts.Add(text);
                    Player.States = Character.CharacterStates.Hurt;
                    Player.Hurt = true;
                    
                    Player.Idle = false;
                    Player.Attacking = false;
                    Player.Jumped = false;
                    Player.Casting = false;
                }
                foreach(var enemy in Enemies)
                {
                    if (item.Rectangle.Intersects(enemy.Hitbox) && !enemy.Hurt)
                    {
                        enemy.Health -= (int)(enemy.MaxHp / 4);
                        DamageText text = new(Convert.ToString((int)(enemy.MaxHp / 4)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.Red);
                        DamageTexts.Add(text);

                        enemy.Hurt = true;
                        enemy._count = 0;
                        enemy.States = EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
                foreach (var enemy in Kitsunes)
                {
                    if (item.Rectangle.Intersects(enemy.Hitbox) && !enemy.Hurt)
                    {
                        enemy.Health -= (int)(enemy.MaxHp / 4);
                        DamageText text = new(Convert.ToString((int)(enemy.MaxHp / 4)), new(enemy.Hitbox.Center.X, enemy.Position.Y), Color.Red);
                        DamageTexts.Add(text);

                        enemy.Hurt = true;
                        enemy._count = 0;
                        enemy.States = (Kitsune.EnemyStates)EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
                foreach (var enemy in Scorpions)
                {
                    if (item.Rectangle.Intersects(enemy.Hitbox(enemy.Position)) && !enemy.Hurt)
                    {
                        enemy.Health -= (int)(enemy.MaxHp / 4);
                        DamageText text = new(Convert.ToString((int)(enemy.MaxHp / 4)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.Red);
                        DamageTexts.Add(text);

                        enemy.Hurt = true;
                        enemy._count = 0;
                        enemy.States = (Scorpion.EnemyStates)EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
            }
            foreach (Enemy enemy in Enemies)
            {
                if (UserInterface.open|| UserInterface.EditOpen||Player.Reviving)
                {
                    enemy.Speed = 0;
                }
                else
                {
                    if (Player.Attacking && enemy.Hitbox.Intersects(Player.AttackRange()))
                    {
                        if (!enemy.Hurt)
                        {
                            if (enemy._spriteEffect != Player.SpriteEffect)
                            {
                                enemy.Health -= Player.Atk * 2;
                                DamageText text = new(Convert.ToString(Player.Atk * 2), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }
                            else
                            {
                                enemy.Health -= Player.Atk;
                                DamageText text = new(Convert.ToString(Player.Atk), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }

                            enemy.Hurt = true;
                            enemy._count = 0;
                            enemy.States = EnemyStates.Hurt;
                            enemy.Speed = 0;

                        }

                    }
                    if (enemy.Hurt != true && enemy.AttackRange().Intersects(Player.Hitbox(Player.Position)) && !Player.Hurt)
                    {
                        if (!enemy.IsAttacking)
                        {
                            enemy.IsAttacking = true;
                            enemy.States = EnemyStates.Attack;
                            enemy._count = 0;
                            enemy.Speed = 0;
                        }
                        else if (!Player.Dashing)
                        {
                            Player.States = Character.CharacterStates.Hurt;
                            Player.Hurt = true;
                            Player.Idle = false;
                            Player.Attacking = false;
                            Player.Jumped = false;
                            Player.Casting = false;
                            Player.Health -= enemy.Atk;
                            DamageText text = new(Convert.ToString(enemy.Atk), new(Player.Hitbox(Player.Position).Center.X, Player.Position.Y), Color.Red);
                            DamageTexts.Add(text);
                            Player.Dodge = false;
                        }
                        else if (Player.Dashing)
                        {
                            Player.Dodge = true;
                            Image image = new(Player.Texture, new Rectangle((int)Player.Position.X, (int)Player.Position.Y, Player.Rectangle.Width, Player.Rectangle.Height), Player.SpriteEffect);
                            Trails.Add(image);
                        }
                    }

                }
                enemy.Update(displacement, Tiles,Platforms, Player);
                if (enemy.Poisoned > 0)
                {
                    enemy.PoisonedSpeed += Globals.Time;
                    if (enemy.PoisonedSpeed > 1)
                    {
                        enemy.Poisoned += -1;
                        enemy.Health += -(int)(0.1f * Player.Atk);
                        DamageText text = new(Convert.ToString((int)(0.1f * Player.Atk)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.Turquoise);
                        DamageTexts.Add(text);
                        if (enemy.Poisoned == 0)
                        {
                            enemy.Color = Color.White;
                        }
                        enemy.PoisonedSpeed = 0;
                    }

                }
            }
            foreach (var enemy in Kitsunes)
            {
                if (UserInterface.open || UserInterface.EditOpen || Player.Reviving)
                {
                    enemy.Speed = 0;
                }
                else
                {
                    if (Player.Attacking && enemy.Hitbox.Intersects(Player.AttackRange()))
                    {
                        if (!enemy.Hurt)
                        {
                            if (enemy._spriteEffect != Player.SpriteEffect)
                            {
                                enemy.Health -= Player.Atk * 2;
                                DamageText text = new(Convert.ToString(Player.Atk * 2), new(enemy.Hitbox.Center.X, enemy.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }
                            else
                            {
                                enemy.Health -= Player.Atk;
                                DamageText text = new(Convert.ToString(Player.Atk), new(enemy.Hitbox.Center.X, enemy.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }

                            enemy.Hurt = true;
                            enemy._count = 0;
                            enemy.States = (Kitsune.EnemyStates)EnemyStates.Hurt;
                            enemy.Speed = 0;

                        }

                    }
                }
                enemy.Update(displacement, Tiles, Platforms);
                if (enemy.Poisoned > 0)
                {
                    enemy.PoisonedSpeed += Globals.Time;
                    if (enemy.PoisonedSpeed > 1)
                    {
                        enemy.Poisoned += -1;
                        enemy.Health += -(int)(0.1f * Player.Atk);
                        DamageText text = new(Convert.ToString((int)(0.1f * Player.Atk)), new(enemy.Hitbox.Center.X, enemy.Position.Y), Color.Turquoise);
                        DamageTexts.Add(text);
                        if (enemy.Poisoned == 0)
                        {
                            enemy.Color = Color.White;
                        }
                        enemy.PoisonedSpeed = 0;
                    }

                }
            }
            for (int i = 0; i < Scorpions.Count; i++)
            {
                if (Scorpions[i].Died)
                {
                    Coin coin = new(Globals.Content.Load<Texture2D>("coin"), new(Scorpions[i].Rectangle.Center.X, Scorpions[i].Rectangle.Center.Y), 5 * _gen.Next(1, 5));
                    Coins.Add(coin);
                    Scorpions.RemoveAt(i);
                    i--;
                }
            }
            foreach (Scorpion scorpion in Scorpions)
            {
                if (UserInterface.open || UserInterface.EditOpen||Player.Reviving)
                {
                    scorpion.Speed = 0;
                }
                else
                {
                    foreach (Slash slash in scorpion.Slashes)
                    {
                        if (!Player.Dashing && Player.Hitbox(Player.Position).Intersects(slash.Rectangle))
                        {
                            slash.Hit = true;
                            Player.States = Character.CharacterStates.Hurt;
                            Player.Hurt = true;
                            Player.Idle = false;
                            Player.Attacking = false;
                            Player.Jumped = false;
                            Player.Casting = false;
                            Player.Health -= scorpion.Atk;
                            DamageText text = new(Convert.ToString(scorpion.Atk), new(Player.Hitbox(Player.Position).Center.X, Player.Position.Y), Color.Red);
                            DamageTexts.Add(text);
                            Player.Dodge = false;
                        }
                        else if (Player.Dashing&& Player.Hitbox(Player.Position).Intersects(slash.Rectangle))
                        {
                            Player.Dodge = true;
                            Image image = new(Player.Texture, new Rectangle((int)Player.Position.X, (int)Player.Position.Y, Player.Rectangle.Width, Player.Rectangle.Height), Player.SpriteEffect);
                            Trails.Add(image);
                        }
                    }
                    if (Player.Attacking && scorpion.Hitbox(scorpion.Position).Intersects(Player.AttackRange()))
                    {
                        if (!scorpion.Hurt)
                        {
                            if (scorpion._spriteEffects != Player.SpriteEffect)
                            {
                                scorpion.Health -= Player.Atk * 2;
                                DamageText text = new(Convert.ToString(Player.Atk * 2), new(scorpion.Rectangle.Center.X, scorpion.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }
                            else
                            {
                                scorpion.Health -= Player.Atk;
                                DamageText text = new(Convert.ToString(Player.Atk), new(scorpion.Rectangle.Center.X, scorpion.Position.Y), Color.Yellow);
                                DamageTexts.Add(text);
                            }

                            scorpion.Hurt = true;
                            scorpion._count = 0;
                            scorpion.States = Scorpion.EnemyStates.Hurt;
                            scorpion.Speed = 0;

                        }
                    }
                    if (!scorpion.IsAttacking)
                    {
                        int x = 0;
                        int y = 0;
                        foreach (Tile tile in Tiles)
                        {
                            if (tile.Rectangle.Contains(new Rectangle((int)scorpion.Rectangle.Center.X, (int)scorpion.Rectangle.Center.Y, 1, 1)))
                            {
                                x = tile.Location.x;
                                y = tile.Location.y; break;
                            }
                        }
                        bool attack = false;
                        if (scorpion.RightDirection)
                        {
                            for (int i = 1; i < 6; i++)
                            {
                                if (x - i < 0) break;
                                if (Tiles[x - i, y].Visible)
                                {
                                    attack = false;
                                    break;
                                }
                                if (Tiles[x-i, y].IsPlayerHere)
                                {
                                    attack = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 1; i < 6; i++)
                            {
                                if (x + i >102) break;
                                if (Tiles[x + i, y].Visible)
                                {
                                    attack = false;
                                    break;
                                }
                                if (Tiles[x+i, y ].IsPlayerHere)
                                {
                                    attack = true;
                                    break;
                                }
                            }
                        }
                        if (attack)
                        {
                            scorpion.IsAttacking = true;
                            scorpion.States = Scorpion.EnemyStates.Attack;
                            scorpion._count = 0;
                            scorpion.Speed = 0;
                        }
                    }
                    if (scorpion.Poisoned > 0)
                    {
                        scorpion.PoisonedSpeed += Globals.Time;
                        if (scorpion.PoisonedSpeed > 1)
                        {
                            scorpion.Poisoned += -1;
                            scorpion.Health += (int)(-0.1f * Player.Atk);
                            DamageText text = new(Convert.ToString((int)(0.1f * Player.Atk)), new(scorpion.Rectangle.Center.X, scorpion.Position.Y), Color.Turquoise);
                            DamageTexts.Add(text);
                            if (scorpion.Poisoned == 0)
                            {
                                scorpion.Color = Color.White;
                            }
                            scorpion.PoisonedSpeed = 0;
                        }

                    }
                }
                scorpion.Update(displacement, Tiles, Platforms);
            }

            if (UserInterface.MouseState == "soil")
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    for (int i = 0; i < Tiles.GetLength(0); i++)
                    {
                        if (!Tiles[i, j].IsPlayerHere&&
                            Clickable() && Tiles[i, j].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                        {
                            bool add = true;
                            foreach (Enemy enemy in Enemies)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(enemy.Rectangle))
                                {
                                    add = false;break;
                                }
                            }
                            foreach (Treasure treasure in Treasures)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(treasure.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Kitsunes)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Hitbox))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Portals)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Platforms)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Ladders)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Scorpions)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            foreach (var item in Spikes)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(item.Rectangle))
                                {
                                    add = false; break;
                                }
                            }
                            if (Tiles[i, j].Visible)
                            {
                                Tiles[i, j].Visible = false;
                                if (j + 1 < Tiles.GetLength(1) && Tiles[i, j + 1].Visible)
                                {
                                    Tiles[i, j + 1].Texture = _soilTexture;
                                }
                            }
                            else if (add)
                            {
                                Tiles[i, j].Visible = true;
                                if (j + 1 < Tiles.GetLength(1) && Tiles[i, j + 1].Visible)
                                {
                                    Tiles[i, j + 1].Texture = _soil2Texture;
                                }
                                if (j - 1 > 0 && Tiles[i, j - 1].Visible)
                                {
                                    Tiles[i, j].Texture = _soil2Texture;
                                }
                                if (j - 1 > 0 && !Tiles[i, j - 1].Visible)
                                {
                                    Tiles[i, j].Texture = _soilTexture;
                                }
                            }
                        }
                        if (Clickable() && Tiles[i, j].Rectangle.Contains(InputManager.MouseRectangle) && !Tiles[i, j].Visible)
                        {
                            Tiles[i, j].Hover = true;
                        }
                        else
                        {
                            Tiles[i, j].Hover = false;
                        }
                    }
                }
            }
            bool IsTouchingSoil = false;
            foreach (Tile tile in Tiles)
            {
                if (tile.Visible&&tile.Rectangle.Contains(InputManager.MouseRectangle)) { IsTouchingSoil = true; break; }
            }
            if (UserInterface.MouseState == "enemy")
            {
                bool Add = true;
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Clickable() && Enemies[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked&&Add)
                    { 
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Dog (2)"), new(InputManager.MouseRectangle.X-20, InputManager.MouseRectangle.Y-40));
                        Enemies.Add(enemy);
                    }
                }
                else
                {
                    DrawItem = false;

                }
            }
            if (UserInterface.MouseState == "treasure")
            {
                bool Add = true;
                for (int i = 0; i < Treasures.Count; i++)
                {
                    if (Clickable() && Treasures[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Treasures.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Treasure treasure = new(Globals.Content.Load<Texture2D>("treasure"), new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y));
                        Treasures.Add(treasure);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "portal")
            {
                bool Add = true;
                for (int i = 0; i < Portals.Count; i++)
                {
                    if (Clickable() && Portals[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Portals.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Portal portal = new(Globals.Content.Load<Texture2D>("PortalTex"), new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y));
                        Portals.Add(portal);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "scorpion")
            {
                bool Add = true;
                for (int i = 0; i < Scorpions.Count; i++)
                {
                    if (Clickable() && Scorpions[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Scorpions.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Scorpion scorpion = new(Globals.Content.Load<Texture2D>("scorpionTex"), new(InputManager.MouseRectangle.X-40, InputManager.MouseRectangle.Y-40));
                        Scorpions.Add(scorpion);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "platform")
            {
                bool Add = true;
                for (int i = 0; i < Platforms.Count; i++)
                {
                    if (Clickable() && Platforms[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Platforms.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Rectangle rect = new();
                        foreach(Tile tile in Tiles)
                        {
                            if (tile.Rectangle.Contains(InputManager.MouseRectangle))
                            {
                                rect = tile.Rectangle; break;
                            }
                        }
                        Platform platform = new(Globals.Content.Load<Texture2D>("platform"), rect);
                        Platforms.Add(platform);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "ladder")
            {
                bool Add = true;
                for (int i = 0; i < Ladders.Count; i++)
                {
                    if (Clickable() && Ladders[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Ladders.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Rectangle rect = new();
                        foreach (Tile tile in Tiles)
                        {
                            if (tile.Rectangle.Contains(InputManager.MouseRectangle))
                            {
                                rect = tile.Rectangle; break;
                            }
                        }
                        Platform platform = new(Globals.Content.Load<Texture2D>("ladder"), rect);
                        Ladders.Add(platform);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "background")
            {
                bool Add = true;
                for (int i = 0; i < Backgrounds.Count; i++)
                {
                    if (Clickable() && Backgrounds[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Backgrounds.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Rectangle rect = new();
                        foreach (Tile tile in Tiles)
                        {
                            if (tile.Rectangle.Contains(InputManager.MouseRectangle))
                            {
                                rect = tile.Rectangle; break;
                            }
                        }
                        Image item = new(Globals.Content.Load<Texture2D>("cave_wall"), rect,SpriteEffects.None);
                        Backgrounds.Add(item);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "spike")
            {
                bool Add = true;
                for (int i = 0; i < Spikes.Count; i++)
                {
                    if (Clickable() && Spikes[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Spikes.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Rectangle rect = new();
                        foreach (Tile tile in Tiles)
                        {
                            if (tile.Rectangle.Contains(InputManager.MouseRectangle))
                            {
                                rect = tile.Rectangle; break;
                            }
                        }
                        Image item = new(Globals.Content.Load<Texture2D>("spikes"), rect, SpriteEffects.None);
                        Spikes.Add(item);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
            if (UserInterface.MouseState == "kitsune")
            {
                bool Add = true;
                for (int i = 0; i < Spikes.Count; i++)
                {
                    if (Clickable() && Spikes[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Spikes.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Kitsune item = new(new(InputManager.MouseRectangle.X - 32, InputManager.MouseRectangle.Y - 32));
                        Kitsunes.Add(item);
                    }
                }
                else
                {
                    DrawItem = false;
                }
            }
        }
        public List<DamageText> DamageTexts { get; set; } = new List<DamageText>();
        public List<Treasure> Treasures { get; set; } = new List<Treasure>();
        public bool DrawItem { get; set; } = false;
        public void Draw()
        {
            foreach(var item in Backgrounds)
            {
                item.Draw();
            }
            foreach (Tile tile in Tiles)
            {
                if (tile == null) continue;
                tile.Draw();
            }
            foreach(var item in Platforms)
            {
                item.Draw();
            }
            foreach (var item in Ladders)
            {
                item.Draw();
            }
            if (DrawItem&&UserInterface.MouseState == "enemy")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Dog1"), Globals.Rectangle(tileSize,tileSize,new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    ,new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "treasure")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), Globals.Rectangle(60,45, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "portal")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("portal"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "scorpion")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("scorpion"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "platform")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("platform"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "ladder")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("ladder"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "background")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("cave_wall"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "spike")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("spikes"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "kitsune")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Kitsune"), Globals.Rectangle(tileSize, tileSize, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            foreach (var item in Spikes)
            {
                item.Draw();
            }
            foreach (var image in Trails)
            {
                image.Draw();
            }
            foreach (Portal portal in Portals)
            {
                portal.Draw();
            }
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
            foreach (var scorpion in Scorpions)
            {
                scorpion.Draw();
            }
            foreach(var item in Kitsunes)
            {
                item.Draw();
            }
            foreach (Treasure treasure in Treasures)
            {
                treasure.Draw();
            }
            foreach (var coin in Coins)
            {
                coin.Draw() ;
            }
            foreach (var item in Player.capples)
            {
                item.Draw();
            }
            foreach (var text in DamageTexts)
            {
                text.Draw();
            }
            ShopBtn.Draw();
            if (Restart != null)
                Restart.Draw();
        }
    }
}
