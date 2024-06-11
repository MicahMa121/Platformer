
using static Platformer.Enemy;

namespace Platformer
{
    public class Slash
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rect;
        public Rectangle Rectangle { get { return rect; } }
        public bool Hit { get; set; }
        private SpriteEffects spriteEffects;
        public Slash(Texture2D texture, Vector2 position, Vector2 velocity, SpriteEffects spriteEffects)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            Hit = false;
            rect = Globals.Rectangle(Globals.TileSize / 2, Globals.TileSize / 2, position);
            this.spriteEffects = spriteEffects;
        }
        public void Update(Vector2 displacement,Map map)
        {
            position += displacement;
            position += velocity;
            rect = Globals.Rectangle(Globals.TileSize / 2, Globals.TileSize / 2, position);
            foreach (var tile in map.Tiles)
            {
                if (!tile.Visible) { continue; }
                if (rect.Intersects(tile.Rectangle))
                {
                    Hit = true;
                    return;
                }
            }
            foreach (var enemy in map.Enemies)
            {
                if(enemy.Dying) continue;
                if (!Hit&& rect.Intersects(enemy.Rectangle))
                {
                    Hit = true;
                    enemy.Health -= map.Player.Atk * 4;
                    DamageText text = new(Convert.ToString(map.Player.Atk*4), new(enemy.Rectangle.Center.X, enemy.Position.Y), Color.AliceBlue);
                    map.DamageTexts.Add(text);
                    enemy.Hurt = true;
                    enemy.States = EnemyStates.Hurt;
                    enemy.Speed = 0;
                    return;
                }
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture, rect,null, Color.White,0f,Vector2.Zero,spriteEffects,0f);
        }
    }
}
