
namespace Platformer
{
    public class Tutorial
    {
        private Texture2D _texture;
        private List<List<Texture2D>> _textures = new List<List<Texture2D>>();
        public Vector2 MaptoVector(int x, int y) => new(x * Globals.TileSize, y * Globals.TileSize);
        public Rectangle MaptoScreen(int x, int y) => new(x * Globals.TileSize, y * Globals.TileSize, Globals.TileSize, Globals.TileSize);
        private List<Image> _images = new List<Image>();
        private List<DamageText> _texts = new List<DamageText>();
        public Image _image;
        public bool done;
        public Tutorial()
        {
            _texture = Globals.Content.Load<Texture2D>("keys");
            _textures = Globals.SpriteSheet(_texture,6,6);
            _image = new(Globals.Content.Load<Texture2D>("pixelarrow"), new(475,530, 50, 50), SpriteEffects.None);
            Refresh();
        }
        public void Refresh()
        {
            _images.Clear();
            _texts.Clear();
            done = false;
            Image w = new(_textures[3][4], MaptoScreen(4,5), SpriteEffects.None);
            Image s = new(_textures[3][0], MaptoScreen(5, 5), SpriteEffects.None);
            Image a = new(_textures[0][0], MaptoScreen(5, 4), SpriteEffects.None);
            Image d = new(_textures[0][3], MaptoScreen(6, 5), SpriteEffects.None);

            Image space = new(Globals.Content.Load<Texture2D>("spacebar"), MaptoScreen(7, 11), SpriteEffects.None);
            _images.Add(w);_images.Add(s);_images.Add(d);_images.Add(a);_images.Add(space);
            DamageText text1 = new("Use WASD to Move Around", MaptoVector(3, 6), Color.Black);
            _texts.Add(text1);
        }
        public void Update(Vector2 displacement)
        {
            foreach(var item in _images)
            {
                item.UpdatePosition(displacement);

            }
            foreach(var item in _texts)
            {
                item.UpdatePosition(displacement);
            }
        }
        public void Draw()
        {
            foreach (var item in _images)
            {
                if (Globals.OutSideOfScreen(item.Rectangle)) continue;
                item.Draw();
            }
            foreach (var item in _texts)
            {
                item.Draw();
            }
        }
    }
}
