
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
        public DamageText _text;
        public bool done;
        public Tutorial()
        {
            _texture = Globals.Content.Load<Texture2D>("keys");
            _textures = Globals.SpriteSheet(_texture,6,6);
            _image = new(Globals.Content.Load<Texture2D>("pixelarrow"), new(475,530, 50, 50), SpriteEffects.None);
            _text = new("Click Here to Buy Equipments", new(200, 480), Color.Yellow);
            Refresh();
        }
        public void Refresh()
        {
            _images.Clear();
            _texts.Clear();
            done = false;
            Image w = new(_textures[3][4], MaptoScreen(5, 4), SpriteEffects.None);
            Image s = new(_textures[3][0], MaptoScreen(5, 5), SpriteEffects.None);
            Image a = new(_textures[0][0], MaptoScreen(4, 5), SpriteEffects.None);
            Image d = new(_textures[0][3], MaptoScreen(6, 5), SpriteEffects.None);
            Image z = new(_textures[4][1], MaptoScreen(26, 20), SpriteEffects.None);
            Image x = new(_textures[3][5], MaptoScreen(34, 20), SpriteEffects.None);
            Image e = new(_textures[0][4], MaptoScreen(23, 21), SpriteEffects.None);
            Rectangle rectshift = MaptoScreen(12, 5);
            rectshift.Width *= 2;
            Image shift = new(Globals.Content.Load<Texture2D>("shift"),rectshift, SpriteEffects.None);
            Image arrow = new(Globals.Content.Load<Texture2D>("pixelarrow"), MaptoScreen(17, 8), SpriteEffects.None);
            Image space = new(Globals.Content.Load<Texture2D>("spacebar"), new(MaptoScreen(7, 11).X, MaptoScreen(7, 11).Y, MaptoScreen(7, 11).Width*3, MaptoScreen(7, 11).Height), SpriteEffects.None);
            _images.Add(w);_images.Add(s);_images.Add(d);_images.Add(a);_images.Add(space);_images.Add(shift);_images.Add(z);_images.Add(x);_images.Add(e);_images.Add(arrow);

            DamageText text1 = new("Use WASD to Move Around", MaptoVector(3, 6), Color.Black);
            DamageText text3 = new("Press SHIFT to Sprint and Avoid Damage\n           for a Brief Moment", MaptoVector(9, 6), Color.Black);
            DamageText text2 = new("SPACE to Attack and Open Chests", MaptoVector(5,10), Color.Black);
            DamageText text4 = new(" Stamina is the Line Below\n     Your HP Bar\n Jump, Sprint, Skill Attack \n    All Cost Stamina", MaptoVector(30, 13), Color.Black);
            DamageText text5 = new("When Buying Items with Special Abilities\n  You Unlock the Corresponding Skill\n   Press Z or X to Unleash Them!", MaptoVector(27, 19), Color.Black);
            DamageText text6 = new("Press E to Travel\nto the Next Level", MaptoVector(22,23)+new Vector2(-20,40), Color.Black);
            DamageText text7 = new("Caution!", MaptoVector(5, 20), Color.Red);
            _texts.Add(text1);_texts.Add(text2);_texts.Add(text3);_texts.Add(text4);_texts.Add(text5);_texts.Add(text6);_texts.Add(text7);
            
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
