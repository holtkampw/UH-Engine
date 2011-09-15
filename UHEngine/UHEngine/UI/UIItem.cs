using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHEngine.ScreenManagement;

namespace UHEngine.UI
{
    public enum UIItemStatus { Inactive, Hover, Click }

    public class UIItem
    {
        #region Fields
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        protected Rectangle InactiveSource { get; set; }
        protected Rectangle HoverSource { get; set; }
        protected Rectangle ClickSource { get; set; }
        public bool Found { get; set; }
        protected Texture2D NotFoundTexture { get; set; }
        protected Texture2D backgroundTexture { get; set; }

        public UIItemStatus Status { get; set; }
        public Action OurAction { get; set; }

        public int SpriteSizeX = 60;
        public int SpriteSizeY = 0;
        #endregion

        #region Initialization
        public UIItem(Texture2D texture, Vector2 position)
        {
            Helper(texture, position);
        }

        public UIItem(Texture2D texture, Vector2 position, bool found)
        {
            this.Found = found;
            Helper(texture, position);
        }

        public UIItem(Texture2D texture, Vector2 position, int SpriteSizeX, int SpriteSizeY)
        {
            this.SpriteSizeX = SpriteSizeX;
            this.SpriteSizeY = SpriteSizeY;
            Helper(texture, position);
        }

        private void Helper(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;

            if (SpriteSizeY == 0)
                SpriteSizeY = SpriteSizeX;

            this.Bounds = new Rectangle((int)position.X, (int)position.Y, SpriteSizeX, SpriteSizeY);
            this.InactiveSource = new Rectangle(0, 0, SpriteSizeX, SpriteSizeY);
            this.HoverSource = new Rectangle(0, SpriteSizeY, SpriteSizeX, SpriteSizeY);
            this.ClickSource = new Rectangle(0, SpriteSizeY * 2, SpriteSizeX, SpriteSizeY);
            this.NotFoundTexture = ScreenManagement.ScreenManager.Game.Content.Load<Texture2D>(@"UI\lock_icon");
            this.backgroundTexture = ScreenManagement.ScreenManager.Game.Content.Load<Texture2D>(@"UI\icon_bg");
            this.Status = UIItemStatus.Inactive;

        }
        #endregion

        #region Draw
        public virtual void Draw(GameTime gameTime)
        {
            if (Found)
            {
                //Draw the correct Sprite
                switch (Status)
                {
                    case UIItemStatus.Inactive:
                        ScreenManager.SpriteBatch.Draw(backgroundTexture, Bounds, InactiveSource, Color.White);
                        break;
                    case UIItemStatus.Hover:
                        ScreenManager.SpriteBatch.Draw(backgroundTexture, Bounds, HoverSource, Color.White);
                        break;
                    case UIItemStatus.Click:
                        ScreenManager.SpriteBatch.Draw(backgroundTexture, Bounds, ClickSource, Color.White);
                        break;
                }
            } else
                ScreenManager.SpriteBatch.Draw(backgroundTexture, Bounds, InactiveSource, Color.White);

            //Different sprite for a used UIItem
            if(!Found)
                ScreenManager.SpriteBatch.Draw(NotFoundTexture, Position, Color.White);
            else
                ScreenManager.SpriteBatch.Draw(Texture, Position, Color.White);
        }
        #endregion
    }
}
