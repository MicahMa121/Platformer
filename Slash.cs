
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
        public Slash(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            Hit = false;
            rect = Globals.Rectangle(Globals.TileSize, Globals.TileSize, position);
        }
        public void Update(Vector2 displacement,Map map)
        {
            position += displacement;
            position += velocity;
            rect = Globals.Rectangle(Globals.TileSize, Globals.TileSize, position);
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
                if (!Hit&& rect.Intersects(enemy.Rectangle))
                {
                    Hit = true;
                    enemy.Health -=40;
                    enemy.Hurt = true;
                    enemy.States = EnemyStates.Hurt;
                    enemy.Speed = 0;
                    return;
                }
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture, rect, Color.White);
        }
    }
}
