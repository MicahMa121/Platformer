

using static Platformer.Character;

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
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }
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
        public Enemy(Texture2D spritesheet, Vector2 position)
        {
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width / 6, spritesheet.Height / 6);
            Textures = SpriteSheet(spritesheet, 5, 5);
            States = EnemyStates.Idle;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;
            _time = 0;
            _animationSpeed = 0.1f;
            _velocity.X = -Speed;
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
        private int _count = 0;
        public void Update(Vector2 displacement, List<Tile> tiles)
        {
            //Textures
            _time += Globals.Time;

            if (_time >= _animationSpeed)
            {
                if (_count >= Textures[(int)States].Count)
                {
                    _count = 0;
                }
                Texture = Textures[(int)States][_count];
                _count++;
                _time = 0;
            }
            //displacement
            Position += displacement;
            Rectangle = new((int)Position.X, (int)Position.Y, Rectangle.Width,Rectangle.Height);
            //movement
            _velocity.Y += Gravity;
            //collision
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            foreach (Tile collider in tiles)
            {
                if (newPos.X != Position.X)
                {
                    newHitbox = Hitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        if (!RightDirection)
                        {
                            newPos.X = Position.X;
                            _velocity.X = -_velocity.X;
                            for (int i = 0; i < Textures.Count; i++)
                            {
                                for (int j = 0; j < Textures[i].Count; j++)
                                {
                                    Textures[i][j] = FlipTexture(Textures[i][j]);
                                }
                            }
                            RightDirection = true;
                        }
                        else
                        {
                            newPos.X = Position.X;
                            _velocity.X = -_velocity.X;
                            for (int i = 0; i < Textures.Count; i++)
                            {
                                for (int j = 0; j < Textures[i].Count; j++)
                                {
                                    Textures[i][j] = FlipTexture(Textures[i][j]);
                                }
                            }
                            RightDirection = false ;
                        }
                    }
                }
                newHitbox = Hitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top - 70;
                        _grounded = true;
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
        private bool _grounded = true;
        public Rectangle Hitbox(Vector2 pos)
        {
            int offset;
            if (RightDirection)
            {
                offset = 15;
            }
            else
            {
                offset = -15;
            }
            return new((int)pos.X +25 + offset, (int)pos.Y +10, 30, 60);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox(Position), Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffects.None, 0f);
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
