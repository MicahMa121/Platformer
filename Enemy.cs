namespace Platformer
{
    public class Enemy
    {
        private float _time;
        private float _animationSpeed;
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public int Poisoned { get; set; } = 0;
        public float PoisonedSpeed { get; set; } = 0;
        public Rectangle Rectangle { get; set; }
        public float Atk { get; set; } = 5f*Globals.Level;
        
        public float Def { get; set; }

        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }

        public SpriteEffects _spriteEffect;
        public enum EnemyStates
        {
            Attack,
            Death,
            Hurt,
            Idle,
            Walk,
        }
        public EnemyStates States { get; set; }
        public bool RightDirection { get; set; } = true;
        public float Health { get; set; }
        public float MaxHp { get; set; } = 50f*Globals.Level;
        public Enemy(Texture2D spritesheet, Vector2 position)
        {
            _spriteEffect = SpriteEffects.None;
            Position = position;
            Hitbox = ToHitbox(position);
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width / 5, spritesheet.Height / 5);
            Textures = SpriteSheet(spritesheet, 5, 5);
            States = EnemyStates.Walk;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;
            _time = 0;
            _animationSpeed = 0.1f;
            _velocity.X = -Speed;
            Health = MaxHp;
            for (int i = 0; i < 4; i++) 
                Textures[(int)EnemyStates.Hurt].Insert(0, Textures[(int)EnemyStates.Hurt][1]);
        }
        public static List<List<Texture2D>> SpriteSheet(Texture2D spritesheet, int w, int h)
        {
            List<List<Texture2D>> textures = new List<List<Texture2D>>();
            for (int j = 0; j < h; j++)
            {
                List<Texture2D> List = new List<Texture2D>();
                int blanks = 0;
                if (j == 2)
                {
                    blanks = 3;
                }
                else if (j == 3 )
                {
                    blanks = 1;
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
        public int _count = 0;
        public bool IsAttacking { get; set; } = false;
        public bool Dying { get; set; } = false;
        public bool Died { get; set; } = false;

        public void Update(Vector2 displacement, Tile[,] tiles,List<Platform> Platforms,Character player)
        {
            //displacement
            Position += displacement;
            Hitbox = ToHitbox(Position);
            Rectangle = new((int)Position.X, (int)Position.Y, Rectangle.Width, Rectangle.Height);
            if (Health<= 0&&!Dying) 
            { 
                Dying = true;
                States = EnemyStates.Death;
                _count = 0;
            }
            if (Dying )
            {
                _time += Globals.Time;
                if (_time >= _animationSpeed)
                {
                    _count++;
                    if (_count >= Textures[(int)States].Count)
                    {
                        Died = true;
                    }
                    else
                    {
                                  Texture = Textures[(int)States][_count];          
                    }
                    _time = 0;
                }
                return;
            }
            if (Globals.OutSideOfScreen(ToHitbox(Position)))
            {
                return;
            }
            if (RightDirection)
            {
                _velocity.X = -Speed;
                _spriteEffect = SpriteEffects.None;
            }
            else 
            {
                _velocity.X = Speed;
                _spriteEffect = SpriteEffects.FlipHorizontally;
            }
            //Textures
            _time += Globals.Time;
            if (!IsAttacking&&!Hurt)
            {
                States = EnemyStates.Walk;
                Speed = 2;
            }

            if (_time >= _animationSpeed)
            {
                _count++;
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == EnemyStates.Attack)
                    {
                        IsAttacking = false;
                        Hurt = false;
                    }
                    if (States == EnemyStates.Hurt)
                    {
                        Hurt = false;
                        IsAttacking = false;
                    }
                    _count = 0;
                }
                Texture = Textures[(int)States][_count];
                _time = 0;

            }

            //movement
            _velocity.Y += Globals.Gravity;
            //collision
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            if (!IsAttacking && AttackRange().Intersects(player.Hitbox(player.Position)))
            {
                IsAttacking = true;
                States = EnemyStates.Attack;
                _count = 0;
                Speed = 0;

            }
            foreach (Tile collider in tiles)
            {
                if (!collider.Visible) continue;
                if (newPos.X != Position.X)
                {
                    newHitbox = ToHitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;

                        RightDirection = !RightDirection;
                    }
                }
                newHitbox = ToHitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top - 67;
                        _velocity.Y = 0;
                    }
                    else if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom;
                    }
                }
            }
            foreach (var collider in Platforms)
            {
                Rectangle prevHitbox = Hitbox;
                newHitbox = ToHitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle) && prevHitbox.Bottom <= collider.Position.Y)
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top - 67;
                        _velocity.Y = 0;
                    }
                }
            }
            Position = newPos;
            Hitbox = ToHitbox(Position);
        }
        public int Speed { get; set; } = 2;
        private Vector2 _velocity;
        public bool Hurt { get; set; }
        public Rectangle Hitbox { get; set; }
        public Rectangle ToHitbox(Vector2 pos)
        {
            return new((int)pos.X+5, (int)pos.Y+20, 70, 47);
        }
        public Rectangle AttackRange()
        {
            int x;
            if (RightDirection)
            {
                x = 65;
            }
            else
            {
                x = -15;
            }
            return new(Hitbox.Center.X-x, Hitbox.Y, 50, 47);
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), AttackRange(), Color.Blue);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox, Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, _spriteEffect, 0f);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, Globals.TileSize, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, (int)(Health/MaxHp*Globals.TileSize), 10), Color.Green);
        }
    }
}
