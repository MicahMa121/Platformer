
using static Platformer.Character;

namespace Platformer
{
    public class Enemy
    {
        private float _time;
        private float _animationSpeed;
        private Random _gen = new Random();
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
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
        public Enemy(Texture2D spritesheet, Vector2 position)
        {
            _spriteEffect = SpriteEffects.None;
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width / 6, spritesheet.Height / 6);
            Textures = SpriteSheet(spritesheet, 5, 5);
            States = EnemyStates.Walk;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;
            _time = 0;
            _animationSpeed = 0.1f;
            _velocity.X = -Speed;
            Health = 80f;
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

                    if (List.Count < w)
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
        public void Update(Vector2 displacement, Tile[,] tiles,Character player)
        {
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
            //displacement
            Position += displacement;
            Rectangle = new((int)Position.X, (int)Position.Y, Rectangle.Width, Rectangle.Height);
            //movement
            _velocity.Y += Gravity;
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
                    newHitbox = Hitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;

                        RightDirection = !RightDirection;
                    }
                }
                newHitbox = Hitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top -68;
                        _velocity.Y = 0;
                    }
                    else if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom;
                    }
                }
            }
            Position = newPos;
        }

        public int Gravity { get; set; } = 1;
        public int Speed { get; set; } = 2;
        private Vector2 _velocity;
        public bool Hurt { get; set; }
        public Rectangle Hitbox(Vector2 pos)
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
            return new(Hitbox(Position).Center.X-x, Hitbox(Position).Y, 50, 47);
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), AttackRange(), Color.Blue);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox(Position), Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, _spriteEffect, 0f);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, 80, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, (int)Health, 10), Color.Green);
        }
        private Texture2D FlipTexture(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;
            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(0, sourceRect, data, 0, data.Length);
            Texture2D snap = new Texture2D(Globals.Device, width, height);
            Color[] pixelsFlipped = new Color[data.Length];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Array.Copy(data, j + i * width, pixelsFlipped, i * width + (width - j - 1), 1);
                }

            }

            snap.SetData(pixelsFlipped);
            return snap;
        }
    }
}
