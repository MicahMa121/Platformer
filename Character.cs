

using System.Diagnostics;
using static Platformer.Enemy;

namespace Platformer
{
    public class Character //https://craftpix.net/freebies/free-3-character-sprite-sheets-pixel-art/
    {
        private float _time;
        private float _animationSpeed;

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; } 
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }
        public enum CharacterStates
        { 
            Idle,
            Walk,
            Run,
            Push,
            Jump,
            Hurt,
            Death,
            Craft,
            Climb,
            Attack1,
            Attack2,
            Attack3,
        }
        public CharacterStates States { get; set; }
        public bool Hurt { get; set; } = false;
        public bool RightDirection { get; set; } = true;
        public bool Jumped { get; set; } = false;
        public bool Idle { get; set; } = true;
        public float Health { get; set; } 
        public SpriteEffects SpriteEffect { get; set; }
        private List<Slash> slashes = new List<Slash>();
        private List<Boomerang> boomerangs = new List<Boomerang>();
        public Character(Texture2D spritesheet,Texture2D spritesheet2, Vector2 position)
        {
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width/6,spritesheet.Height/6);
            Textures = SpriteSheet(spritesheet, 6, 6);
            Textures.AddRange(Globals.SpriteSheet(spritesheet2, 6, 6));
            States = CharacterStates.Idle;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;//new(spritesheet.Width / 12, spritesheet.Height / 12);
            _time = 0;
            Health = MaxStamina;
            Stamina = MaxStamina;
            _animationSpeed = 0.1f;
            SpriteEffect = SpriteEffects.None;
            Textures[(int)CharacterStates.Hurt].Insert(1, Textures[(int)CharacterStates.Hurt][0]);
            Textures[(int)CharacterStates.Hurt].Insert(2, Textures[(int)CharacterStates.Hurt][2]);
            SkillZ = "Locked";
            SkillX = "Locked";
        }
        public static List<List<Texture2D>> SpriteSheet(Texture2D spritesheet, int w, int h)
        {
            List<List<Texture2D>> textures = new List<List<Texture2D>>();
            for (int j = 0; j < h; j++)
            {
                List<Texture2D> List = new List<Texture2D>();
                int blanks = 0;
                if (j == 0)
                {
                    blanks = 2;
                }
                else if (j == 5)
                {
                    blanks = 3;
                }
                for (int i = 0; i < w - blanks; i++)
                {
                    int width = spritesheet.Width / w, height = spritesheet.Height / h;
                    Rectangle sourceRect = new Rectangle(i * width, j * height, width, height);
                    Texture2D cropTexture = new Texture2D(Globals.Device, width, height);
                    Color[] data = new Color[width * height];
                    spritesheet.GetData(0, sourceRect, data, 0, data.Length);
                    cropTexture.SetData(data);

                    if (List.Count <= w)
                    {
                        List.Add(cropTexture);
                    }
                }
                textures.Add(List);
            }
            return textures;
        }
        private int _count = 0;
        public bool Attacking { get; set; } = false;
        public float Stamina { get; set; } 
        public bool Casting { get; set; } = false;
        public bool Dashing { get; set; } = false;
        public float Atk { get; set; } = 5;
        public bool Dodge { get; set; } = false;
        public float MaxStamina { get; set; } = 100f;
        public float MaxHp { get; set; } = 100f;
        public bool Reviving { get; set; } = false;
        public bool Death { get; set;} = false;
        public string SkillZ { get; set; }
        public string SkillX { get; set; }
        public List<ScytheAttack> scytheAttacks = new List<ScytheAttack>();
        public int MoveIndex = 0;
        private float _cooldown = 0;
        private float _enrageDuration = 0;
        public bool Enrage = false;
        public bool SkillAttacking = false;
        public bool IsClimbing = false;
        public void Update(Map map)
        {
            //Textures
            _time += Globals.Time;
            _cooldown += Globals.Time;
            if (_enrageDuration >= 5)
            {
                _enrageDuration = 0;
                Enrage = false;
            }
            else if (Enrage)
            {
                _enrageDuration += Globals.Time;
            }

            if (Health > MaxHp)
            {
                Health = MaxHp;
            }
            if (_time >= _animationSpeed)
            {
                Stamina += MaxStamina*3/100;
                if (Stamina >= MaxStamina)
                {
                    Stamina = MaxStamina;
                }
                if (_count >= 3)
                {
                    Dashing = false;
                    Color = Color.White;
                }
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == CharacterStates.Death)
                    {
                        Death = true;
                        map.Update(MapDisplacement);
                        return;
                    }
                    if (States == CharacterStates.Jump&&_velocity.Y>= 0)
                    {
                        Jumped = false;
                    }
                    if (States == CharacterStates.Attack1)
                    {
                        Attacking = false;
                        SkillAttacking = false;
                    }
                    if (States == CharacterStates.Craft)
                    {
                        Casting = false;
                        if (SkillZ == "Slash")
                        {
                            Vector2 slashV = Vector2.Zero;
                            if (RightDirection)
                            {
                                slashV = new Vector2(Speed * 2, 0);
                            }
                            else
                            {
                                slashV = new Vector2(-Speed * 2, 0);
                            }
                            Slash slash = new Slash(Globals.Content.Load<Texture2D>("Slash"), new(Hitbox(Position).Center.X, Hitbox(Position).Center.Y), slashV, SpriteEffect, Globals.TileSize / 2, Globals.TileSize / 4);
                            slashes.Add(slash);
                        }
                        if (SkillZ == "BanAna")
                        {
                            Vector2 slashV = Vector2.Zero;
                            float angle = (float)Math.PI / 16;
                            if (RightDirection)
                            {
                                slashV = new Vector2(Speed * 2, 0);

                            }
                            else
                            {
                                slashV = new Vector2(-Speed * 2, 0);
                                angle = -(float)Math.PI / 16;
                            }
                            Boomerang slash = new(Globals.Content.Load<Texture2D>("banana"), new(Hitbox(Position).Center.X, Hitbox(Position).Center.Y +10 ), slashV, angle, SpriteEffect);
                            boomerangs.Add(slash);
                        }
                    }
                    if (States == CharacterStates.Attack2)
                    {
                        Jumped = false;
                        Attacking = false;
                        SkillAttacking = false;
                    }
                    if (States == CharacterStates.Hurt)
                    {
                        Hurt = false;
                    }
                    _count = 0;
                    Speed = 5;
                }
                Texture = Textures[(int)States][_count];
                if (!IsClimbing||InputManager.IsKeyPressed(Keys.S)||InputManager.IsKeyPressed(Keys.W) || InputManager.IsKeyPressed(Keys.A) || InputManager.IsKeyPressed(Keys.D))
                {
                    _count++;
                }

                _time = 0;

            }
            foreach (Slash slash in slashes)
            {
                foreach (var enemy in map.Enemies)
                {
                    if (enemy.Dying) continue;
                    if (!slash.Hit && slash.Rectangle.Intersects(enemy.Rectangle))
                    {
                        slash.Hit = true;
                        enemy.Health -= Atk * 4;
                        DamageText text = new(Convert.ToString(map.Player.Atk * 4), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.AliceBlue);
                        map.DamageTexts.Add(text);
                        enemy.Hurt = true;
                        enemy.States = EnemyStates.Hurt;
                        enemy.Speed = 0;
                        break;
                    }
                }
                foreach (var enemy in map.Scorpions)
                {
                    if (enemy.Dying) continue;
                    if (!slash.Hit && slash.Rectangle.Intersects(enemy.Rectangle))
                    {
                        slash.Hit = true;
                        enemy.Health -= Atk * 4;
                        DamageText text = new(Convert.ToString(map.Player.Atk * 4), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.AliceBlue);
                        map.DamageTexts.Add(text);
                        enemy.Hurt = true;
                        enemy.States = (Scorpion.EnemyStates)EnemyStates.Hurt;
                        enemy.Speed = 0;
                        break;
                    }
                }
            }
            foreach (var slash in scytheAttacks)
            {
                foreach (var enemy in map.Enemies)
                {
                    if (enemy.Dying) continue;
                    if (slash.Rectangle.Intersects(enemy.Rectangle)&&enemy.States != EnemyStates.Hurt)
                    {
                        enemy.Health -= (int)Atk * 1.5f;
                        Health += (int)MaxHp * 0.05f;
                        DamageText text = new(Convert.ToString(Math.Round(Atk * 1.5f)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.DarkRed);
                        map.DamageTexts.Add(text);
                        DamageText text1 = new(Convert.ToString(Math.Round(MaxHp * 0.05f)), Position, Color.Green);
                        map.DamageTexts.Add(text1);
                        enemy.Hurt = true;
                        enemy.States = EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
                foreach (var enemy in map.Scorpions)
                {
                    if (enemy.Dying) continue;
                    if (slash.Rectangle.Intersects(enemy.Rectangle) && enemy.States != (Scorpion.EnemyStates)EnemyStates.Hurt)
                    {
                        enemy.Health -= (int)Atk * 1.5f;
                        Health += (int)MaxHp * 0.05f;
                        DamageText text = new(Convert.ToString(Math.Round(Atk * 1.5f)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.DarkRed);
                        map.DamageTexts.Add(text);
                        DamageText text1 = new(Convert.ToString(Math.Round(MaxHp * 0.05f)), Position, Color.Green);
                        map.DamageTexts.Add(text1);
                        enemy.Hurt = true;
                        enemy.States = (Scorpion.EnemyStates)EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
            }
            foreach (var slash in boomerangs)
            {
                foreach (var enemy in map.Enemies)
                {
                    if (enemy.Dying) continue;
                    if (slash.Rectangle.Intersects(enemy.Rectangle) && enemy.States != EnemyStates.Hurt)
                    {
                        enemy.Health -= (int)MaxHp*0.05f;
                        DamageText text = new(Convert.ToString(Math.Round((int)MaxHp * 0.05f)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.GreenYellow);
                        map.DamageTexts.Add(text);
                        enemy.Hurt = true;
                        enemy.States = EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
                foreach (var enemy in map.Scorpions)
                {
                    if (enemy.Dying) continue;
                    if (slash.Rectangle.Intersects(enemy.Rectangle) && enemy.States != (Scorpion.EnemyStates)EnemyStates.Hurt)
                    {
                        enemy.Health -= (int)MaxHp * 0.05f;
                        DamageText text = new(Convert.ToString(Math.Round((int)MaxHp * 0.05f)), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.GreenYellow);
                        map.DamageTexts.Add(text);
                        enemy.Hurt = true;
                        enemy.States = (Scorpion.EnemyStates)EnemyStates.Hurt;
                        enemy.Speed = 0;
                    }
                }
            }
            _velocity.X = 0;
            if (!Jumped && !Attacking&& !Hurt&&!Casting&&!Dashing&&!SkillAttacking)
                Idle = true;
            //States
            if (!Globals.Pause && !Reviving)
            {
                if (InputManager.IsKeyPressed(Keys.D))
                {
                    _velocity.X = Speed;
                    if (!Jumped && !Attacking && !Hurt && !Casting&&!SkillAttacking)
                        States = CharacterStates.Run;
                    if (!RightDirection)
                    {
                        SpriteEffect = SpriteEffects.None;
                        RightDirection = true;
                    }
                    Idle = false;
                }
                else if (InputManager.IsKeyPressed(Keys.A))
                {
                    _velocity.X = -Speed;
                    if (!Jumped && !Attacking && !Hurt && !Casting && !SkillAttacking)
                        States = CharacterStates.Run;
                    if (RightDirection)
                    {
                        SpriteEffect = SpriteEffects.FlipHorizontally;
                        RightDirection = false;
                    }
                    Idle = false;
                }
                else if (Dashing)
                {
                    if (RightDirection)
                    {
                        _velocity.X = Speed;
                    }
                    else
                    {
                        _velocity.X = -Speed;
                    }
                }
                if (InputManager.IsKeyClicked(Keys.Z) && Stamina >= 25f && SkillZ != "Locked")
                {
                    if (!Casting)
                    {
                        _count = 0;
                        States = CharacterStates.Craft;
                        Idle = false;
                        Casting = true;
                        Hurt = false;
                        Attacking = false;
                        SkillAttacking = false;
                        Jumped = false;
                        Stamina -= 25;
                    }
                }
                if (InputManager.IsKeyClicked(Keys.X) && Stamina >= 75f && SkillX != "Locked")
                {
                    if (!Attacking && !SkillAttacking)
                        _count = 0;

                    if (Jumped)
                    {
                        States = CharacterStates.Attack2;
                        Idle = false;
                        Casting = false;
                    }
                    else
                    {
                        States = CharacterStates.Attack1;
                        Idle = false;
                        Casting = false;
                    }
                    Hurt = false;
                    Stamina -= 75;
                    if (SkillX == "firepunch")
                    {
                        Enrage = true;
                        _cooldown = 0;
                        _enrageDuration = 0;
                        if (Health >= MaxHp / 10 && Health < MaxHp / 10 + MaxHp/2)
                        {
                            Health = MaxHp / 10;
                        }
                        else if (Health > MaxHp / 2)
                        {
                            Health -= MaxHp / 2;
                        }
                    }
                }
                if (InputManager.IsKeyClicked(Keys.Q) && Stamina >= 25f && !Dashing)
                {
                    Dashing = true;
                    Speed = 10;
                    Idle = false;
                    Casting = false;
                    Hurt = false;
                    Attacking = false;
                    SkillAttacking = false;
                    Jumped = false;
                    States = CharacterStates.Run;
                    Color = new Color(Color.White, 0.5f);
                    _count = 0;
                    Stamina -= 25;
                }
                bool gravity = true;
                foreach (var item in map.Ladders)
                {
                    if (item.Rectangle.Intersects(Hitbox(Position)))
                    {
                        gravity = false; break;
                    }
                }
                if (gravity)
                {

                    if (InputManager.IsKeyClicked(Keys.W) && Stamina >= 20f)
                    {
                        _velocity.Y = -15;

                        Stamina -= 20;
                        if (!Casting)
                        {
                            States = CharacterStates.Jump;
                            Idle = false;
                            Jumped = true;
                            Attacking = false;
                            SkillAttacking = false;
                            Hurt = false;
                        }
                    }
                    IsClimbing = false;
                    if (States == CharacterStates.Climb)
                    {
                        States = CharacterStates.Idle;
                    }
                }
                else
                {
                    if (InputManager.IsKeyPressed(Keys.W))
                    {
                        Position += new Vector2(0,-Speed);
                        States = CharacterStates.Climb;
                        Idle = false;
                        IsClimbing = true;
                        _velocity.Y = 0;
                        Jumped = false;
                    }
                    else if (InputManager.IsKeyPressed(Keys.S))
                    {
                        Position += new Vector2(0,Speed);
                        States = CharacterStates.Climb;
                        Idle = false;
                        IsClimbing = true;
                        _velocity.Y = 0;
                        Jumped = false;
                    }
                }
                if (!IsClimbing)
                {
                    _velocity.Y += Globals.Gravity;
                }
                if (InputManager.IsKeyClicked(Keys.Space))
                {
                    if (!Attacking && !SkillAttacking)
                        _count = 0;

                    if (Jumped)
                    {
                        if (!Enrage)
                        {

                            Attacking = true;
                        }
                        else
                        {
                            SkillAttacking = true;
                        }
                        States = CharacterStates.Attack2;
                        Idle = false;

                        Casting = false;
                    }
                    else
                    {
                        if (!Enrage)
                        {

                            Attacking = true;
                        }
                        else
                        {
                            SkillAttacking = true;
                        }
                        States = CharacterStates.Attack1;
                        Idle = false;

                        Casting = false;
                    }
                    Hurt = false;
                    if (Enrage)
                    {
                        int x = 0;
                        if (RightDirection)
                        {
                            x = -40;
                        }
                        else
                        {
                            x = 40;
                        }
                        ScytheAttack scytheAttack = new(Globals.Content.Load<Texture2D>("fireslash"), new(Hitbox(Position).Center.X - x, Hitbox(Position).Center.Y), Globals.TileSize*2, SpriteEffect, MoveIndex);
                        scytheAttacks.Add(scytheAttack);
                        MoveIndex++;
                        if (MoveIndex >= scytheAttack._textures.Count)
                        {
                            MoveIndex = 0;
                        }
                    }
                }
                if (_cooldown >= 3)
                {
                    MoveIndex = 0;
                    _cooldown = 0;
                }
                for (int i  = 0; i < scytheAttacks.Count; i++)
                {
                    scytheAttacks[i].Update();
                    if (scytheAttacks[i].Done)
                    {
                        scytheAttacks.RemoveAt(i);
                        i--;
                    }
                }

                if (Idle)
                {
                    States = CharacterStates.Idle;
                }
                Vector2 newPos = Position + _velocity;
                Rectangle newHitbox = Hitbox(newPos);
                foreach (Tile collider in map.Tiles)
                {
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        collider.IsPlayerHere = true;
                    }
                    else
                    {
                        collider.IsPlayerHere = false;
                    }
                    if (!collider.Visible) continue;

                    newHitbox = Hitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;
                    }

                    newHitbox = Hitbox(new(Position.X, newPos.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        if (_velocity.Y >= 0)
                        {
                            newPos.Y = collider.Rectangle.Top - Rectangle.Height;
                            _velocity.Y = 0;
                        }
                        else if (_velocity.Y <= 0)
                        {
                            newPos.Y = collider.Rectangle.Bottom - (Rectangle.Height-1-newHitbox.Height);
                        }
                    }
                }
                foreach (var collider in map.Platforms)
                {
                    Rectangle prevHitbox = Hitbox(Position);
                    newHitbox = Hitbox(new(Position.X, newPos.Y));
                    if (newHitbox.Intersects(collider.Rectangle)&&prevHitbox.Bottom <= collider.Position.Y)
                    {
                        if (_velocity.Y > 0)
                        {
                            newPos.Y = collider.Rectangle.Top - Rectangle.Height;
                            _velocity.Y = 0;
                        }
                    }
                }
                foreach (Border collider in map.Borders)
                {
                    if (newPos.X != Position.X)
                    {
                        newHitbox = Hitbox(new(newPos.X, Position.Y));
                        if (newHitbox.Intersects(collider.Rectangle))
                        {
                            newPos.X = Position.X;
                        }
                    }
                    newHitbox = Hitbox(new(Position.X, newPos.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        if (_velocity.Y >= 0)
                        {
                            newPos.Y = collider.Rectangle.Top - Rectangle.Height;
                            _velocity.Y = 0;
                        }
                        else if (_velocity.Y < 0)
                        {
                            newPos.Y = collider.Rectangle.Bottom - (Rectangle.Height - 1 - newHitbox.Height);
                        }
                    }
                }
                MapDisplacement = new(0, 0);
                if (newPos.Y >= Globals.WindowSize.Y*3/4 - newHitbox.Height)//down
                {
                    MapDisplacement += new Vector2(0, Globals.WindowSize.Y * 3 / 4 - newHitbox.Height - newPos.Y);
                    newPos.Y = Globals.WindowSize.Y * 3 / 4 - newHitbox.Height;
                }
                else if (newPos.Y <= Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height))//up
                {
                    MapDisplacement += new Vector2(0, Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height) - newPos.Y);
                    newPos.Y = Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height);
                }
                if (newPos.X >= Globals.WindowSize.X * 3 / 4 - 57)//right
                {
                    MapDisplacement += new Vector2(Globals.WindowSize.X * 3 / 4 - (Globals.TileSize - newHitbox.Width) - newPos.X, 0);
                    newPos.X = Globals.WindowSize.X * 3 / 4 - (Globals.TileSize - newHitbox.Width);
                }
                else if (newPos.X <= Globals.WindowSize.X / 4 - 23)//left
                {
                    MapDisplacement += new Vector2(Globals.WindowSize.X / 4 - newHitbox.Width - newPos.X, 0);
                    newPos.X = Globals.WindowSize.X / 4 - newHitbox.Width;
                }
                Position = newPos;
                for (int i =  0; i < slashes.Count; i++)
                {
                    slashes[i].Update(MapDisplacement, map.Tiles);
                    if (slashes[i].Hit)
                    {
                        slashes.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < boomerangs.Count; i++)
                {
                    boomerangs[i].Update(MapDisplacement, new(Hitbox(Position).Center.X, Hitbox(Position).Center.Y));
                    if (boomerangs[i].Rectangle.Intersects(Hitbox(Position)) && boomerangs[i].Returning)
                    {
                        boomerangs.RemoveAt(i);
                        i--;
                    }
                }
            }
            map.Update(MapDisplacement);
        }
        public Vector2 MapDisplacement { get; set; }
        public int Speed { get; set; } = 5;
        private Vector2 _velocity;
        public Rectangle Hitbox(Vector2 pos)
        {
            return new((int)pos.X + 30, (int)pos.Y +22, 20, 57);
        }
        public Rectangle AttackRange()
        {
            int x;
            if (RightDirection)
            {
                x = Hitbox(Position).X + 20;
            }
            else
            {
                x = Hitbox(Position).X - 30;
            }
            return new(x, Hitbox(Position).Y, 30, 57);
        }
        public void Draw()
        {
            if (Attacking)
            {
                //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), AttackRange(), Color.Blue);
            }
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox(Position), Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffect, 0f);
            foreach (Slash slash in slashes)
            {
                slash.Draw();
            }
            foreach (Boomerang boomerang in boomerangs)
            {
                boomerang.Draw();
            }
            foreach (var item in scytheAttacks)
            {
                item.Draw();
            }
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, Globals.TileSize, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, (int)(Health/MaxHp*Globals.TileSize), 10),Color.Green);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y+10, (int)(Stamina/MaxStamina*Globals.TileSize), 2), Color.LightGoldenrodYellow);
            if (Enrage)
            {
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("firepunch"), new Rectangle((int)Position.X, (int)Position.Y - 20, Globals.TileSize / 4, Globals.TileSize / 4), new Color(Color.White, 1 - 0.2f * (float)_enrageDuration));
            }
            
        }
        private Texture2D FlipTexture(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;
            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(0, sourceRect, data, 0, data.Length);
            Texture2D snap = new Texture2D(Globals.Device,width, height);
            Color[] pixelsFlipped = new Color[data.Length];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                   Array.Copy(data, j + i*width, pixelsFlipped, i*width + (width -j-1) , 1);
                }

            }

            snap.SetData(pixelsFlipped);
            return snap;
        }
    }
}
