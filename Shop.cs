using System.Diagnostics;

namespace Platformer
{
    public class Shop
    {
        private Rectangle ShopInterface = Globals.Rectangle(Globals.WindowSize.X*3/4,Globals.WindowSize.Y/2,new Vector2(Globals.WindowSize.X/2,Globals.WindowSize.Y/2));
        private Rectangle Border = Globals.Rectangle(Globals.WindowSize.X * 3 / 4-10, Globals.WindowSize.Y / 2-10, new Vector2(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));
        private List<Image> ShopImages = new List<Image>();
        private List<Button> ShopButtons = new List<Button>();
        public List<Item> ShopItems = new List<Item>();
        private Texture2D RectTex;
        private float attack, maxHealth, strength,health;
        private Button swordbtn, hpbtn, staminabtn,skill1btn,skill2btn;
        private Random _gen = new Random();
        private Vector2 ItemPosition;
        public Button refreshBtn;
        private Image imgZ, ImgX;
        public Button ExitBtn = new(new(Globals.WindowSize.X*7/8,Globals.WindowSize.Y/5), " X ");
        public List<Button> BuyButtons = new List<Button>();
        public bool OpenShop { get; set; } = false;
        List<Item> Items = new List<Item>();
        public Shop()
        {
            int height = (int)Globals.Font.MeasureString("test").Y;
            RectTex = Globals.Content.Load<Texture2D>("rectangle");
            Image sword = new(Globals.Content.Load<Texture2D>("sword"),Globals.Rectangle(Globals.TileSize,Globals.TileSize,new Vector2(ShopInterface.Left + Globals.TileSize/2, ShopInterface.Top + 50)),SpriteEffects.None);
            ShopImages.Add(sword);
            Image heart = new(Globals.Content.Load<Texture2D>("heart"), Globals.Rectangle(Globals.TileSize*3/4, Globals.TileSize*3/4, new Vector2(ShopInterface.Left + Globals.TileSize/2 + Globals.WindowSize.X * 3 / 20, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(heart);
            Image stamina = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + Globals.TileSize/2 + Globals.WindowSize.X * 3 / 20 * 2, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(stamina);
            imgZ = new(Globals.Content.Load<Texture2D>("Slash"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + Globals.TileSize / 2 + Globals.WindowSize.X * 3 / 20 * 3, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(imgZ);
            ImgX = new(Globals.Content.Load<Texture2D>("Locked"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + Globals.TileSize / 2 + Globals.WindowSize.X * 3 / 20 * 4, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(ImgX);
            swordbtn = new(new(sword.Rectangle.Center.X, sword.Rectangle.Bottom + height), "Atk " + attack);
            ShopButtons.Add(swordbtn);
            hpbtn = new(new(heart.Rectangle.Center.X, sword.Rectangle.Bottom + height), "MaxHp " + maxHealth);
            ShopButtons.Add(hpbtn);
            staminabtn = new(new(stamina.Rectangle.Center.X, stamina.Rectangle.Bottom + height), "Stamina " + strength);
            ShopButtons.Add(staminabtn);
            refreshBtn = new(new(stamina.Rectangle.Center.X, Globals.WindowSize.Y * 3 / 4 - height-10), "Refresh $50");
            ShopButtons.Add(refreshBtn);
            skill1btn = new(new(imgZ.Rectangle.Center.X, stamina.Rectangle.Bottom + height), "_skillz");
            ShopButtons.Add(skill1btn);
            skill2btn = new(new(ImgX.Rectangle.Center.X, stamina.Rectangle.Bottom + height), "_skillx");
            ShopButtons.Add(skill2btn);

            ItemPosition = new(sword.Rectangle.Center.X, sword.Rectangle.Bottom + 5 * height);
            Item _water = new(Globals.Content.Load<Texture2D>("water"), Vector2.Zero, 100, 1, 5, 5, 0, "Clean water\nGulp...Gulp...Gulp...\n\nAtk Bonus: 1\nHp Increase: 5",null,null);
            Item _HpPotion = new(Globals.Content.Load<Texture2D>("hpPotion"), Vector2.Zero, 100, 0, 0, 50, 0, "Health Potion\n\nRestores Your Health, No Special Effects\nHealth Recover: 50",null,null);
            Item _scythe = new(Globals.Content.Load<Texture2D>("scythe"), Vector2.Zero, 2500, 25, -25, 0, 10, "Scythe\nWeapon of a Past Shinigami\nBecareful!\nSacrifice Your Health to Unleash the Power\n\nSpecial Skill(X): FirePunch\nAtk Bonus: 25\nStamina Increase:10",null,"firepunch");
            Item dagger = new(Globals.Content.Load<Texture2D>("dagger"), Vector2.Zero, 100, 3, -10, 0, 0, "Black Dagger\nNo Pain, No Gain\nIncrease ATK by Decreasing MAXHP\n\nAtk Bonus: 3\nHp Decrease: -10",null,null);
            Item boot = new(Globals.Content.Load<Texture2D>("boot"), Vector2.Zero, 100, 0, 5, 5, 5, "Swift Boot\nIncrease HP and Stamina \nI think I might be flash...\n\nStamina Increase: 5\nHp Increase: 5",null,null);
            Item excalibur = new(Globals.Content.Load<Texture2D>("excalibur"), Vector2.Zero, 500, 10, 0, 0, 5, "EXCALIBURR\nFreshly Pulled from a Stone\n\nCommon Skill(Z): Slash\nAtk Bonus: 10\nStamina Increase: 5", "Slash",null);
            Item banana = new(Globals.Content.Load<Texture2D>("Banana"), Vector2.Zero, 500, 0, 100, 100, 5, "Strange Banana\nWhat If I Can Throw A Banana\n\nCommon Skill(Z): BanAna\nHP Increase: 100\nStamina Increase: 5", "BanAna", null);
            Item blueberry = new(Globals.Content.Load<Texture2D>("bluberryslingshot"), Vector2.Zero, 500, 5, 50, 50, 0, "Blub Berry\nThrow Disgusting Berries to Piss Enemies Off!\nDesigned By BlubBlub\n\nCommon Skill(Z): BlubBerry\nAtk Bonus: 5\nHP Increase: 50", "Blubberry", null);
            Item capple = new(Globals.Content.Load<Texture2D>("capple"), Vector2.Zero, 2500, 10, 25, 25, 25, "Capple\n???\nA Cat? An Apple? Boom!\nDemolish Everything No Cap!\nDesigned By BlubBlub\n\nSpecial Skill(X): Le Capple\nAtk Bonus: 10\nHP Increase: 25\nStamina Increase: 25", null, "Capple");

            Items.Add(_water);
            Items.Add(_HpPotion);
            Items.Add(_scythe);
            Items.Add(dagger);
            Items.Add(boot);
            Items.Add(excalibur);
            Items.Add(banana);
            Items.Add(blueberry);
            Items.Add(capple);

            RefreshShop();
        }
        public void RefreshShop()
        {
            ShopItems.Clear();
            BuyButtons.Clear();
            for (int i = 0; i < 5; i++)
            {
                Item item = Items[_gen.Next(0, Items.Count)];
                Item clone = new(item._texture, item.Position, item._cost, item._atk, item._maxHp, item._hp, item._sta, item._description, item._skillz, item._skillx);
                ShopItems.Add(clone);
                Vector2 position = new(ItemPosition.X + Globals.WindowSize.X * 3 / 20 * (4 - i), ItemPosition.Y);
                ShopItems[i].Position = position;
                Button button = new(new(position.X,position.Y+Globals.TileSize*3/4 - 5), " Buy ");
                BuyButtons.Add(button);
            }
        }
        private string _skillz, _skillx;
        public void Update(float atk, float maxhp,float hp, float stamina,string skillz,string skillx)
        {
            attack = atk;
            maxHealth = maxhp;
            strength = stamina;
            health = hp;
            _skillx = skillx;
            _skillz = skillz;
            swordbtn.Text = "Atk " + attack;
            hpbtn.Text = "Hp " + maxHealth;
            staminabtn.Text = "STA " + strength;
            skill1btn.Text = _skillz;
            skill2btn.Text = _skillx;
            imgZ.Texture = Globals.Content.Load<Texture2D>(_skillz);
            ImgX.Texture = Globals.Content.Load<Texture2D>(_skillx);
            if (OpenShop)
            {
                foreach (var item in ShopItems)
                {
                    item.Update();
                }
                foreach (var item in BuyButtons)
                {
                    item.Update();
                }
                bool skip = false;
                foreach(var item in ShopItems)
                {
                    if (item.DescriptionOpen)
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    for (int i = 0; i < ShopItems.Count; i++)
                    {
                        if (BuyButtons[i].ButtonPressed() && !ShopItems[i].IsBought)
                        {
                            ShopItems[i].DescriptionOpen = true;
                        }
                    }
                }
            }
        }

        public void Draw()
        {
            if (OpenShop)
            {
                Globals.SpriteBatch.Draw(RectTex, ShopInterface, Color.SaddleBrown);
                Globals.SpriteBatch.Draw(RectTex, Border, Color.SandyBrown);
                foreach(var item in ShopImages)
                {
                    item.Draw();
                }
                foreach(var item in ShopButtons)
                {
                    item.Draw();
                }
                foreach(var item in ShopItems)
                {
                    item.Draw();
                }
                foreach(var item in BuyButtons)
                {
                    item.Draw();
                }
                ExitBtn.Draw();
                bool shade = false;
                var description = ShopItems[0] ;
                foreach(var item in ShopItems)
                {
                    if (item.DescriptionOpen)
                    {
                        description = item;
                        shade = true;
                    }
                }
                if (shade)
                {
                    Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle(0, 0, Globals.WindowSize.X, Globals.WindowSize.Y), new Color(Color.Black, 0.8f));
                    description.DrawDescription();
                }

            }
        }

    }
}
