using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHEngine.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;

namespace UHEngine.UI
{
    class MainMenuUI : UIItem
    {
        #region Fields
        public bool Previous = false;
        private int currentTime = 0;
        private int maxTime = 500;
        Texture2D icon = null;
        int alpha = 255;
        Vector2 iconPosition = Vector2.Zero;
        #endregion

        #region Initialization
        public MainMenuUI(Texture2D iconTexture, Vector2 iconPosition, Texture2D texture, Vector2 position) : base(texture, position)
        {
            this.icon = iconTexture;
            this.iconPosition = iconPosition;
        }

        public MainMenuUI(Texture2D iconTexture, Vector2 iconPosition, Texture2D texture, Vector2 position, bool found)
            : base(texture, position, found)
        {
            this.icon = iconTexture;
            this.iconPosition = iconPosition;
        }

        public MainMenuUI(Texture2D iconTexture, Vector2 iconPosition, Texture2D texture, Vector2 position, int SpriteSizeX, int SpriteSizeY)
            : base(texture, position, SpriteSizeX, SpriteSizeY)
        {
            this.icon = iconTexture;
            this.iconPosition = iconPosition;
        }
        #endregion

        public void SetStatus(UIItemStatus status)
        {
            UIItemStatus oldStatus = this.Status;
            this.Status = status;

            if (oldStatus == UIItemStatus.Hover && status != UIItemStatus.Hover)
            {
                if (currentTime <= 0)
                {
                    currentTime = maxTime;
                    alpha = 255;
                }
            }
            else if (oldStatus != UIItemStatus.Hover && status == UIItemStatus.Hover)
            {
                currentTime = maxTime;
                    alpha = 0;
            }
            else if (oldStatus != UIItemStatus.Inactive && status == UIItemStatus.Inactive)
            {
                currentTime = maxTime;
            }
        }

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            switch (Status)
            {
                //Draw Correct Icon State
                case UIItemStatus.Inactive:
                    ScreenManager.SpriteBatch.Draw(base.Texture, Bounds, base.InactiveSource, Color.White);
                    break;
                case UIItemStatus.Hover:
                    ScreenManager.SpriteBatch.Draw(base.Texture, Bounds, base.HoverSource, Color.White);
                    break;
                case UIItemStatus.Click:
                    ScreenManager.SpriteBatch.Draw(base.Texture, Bounds, base.ClickSource, Color.White);
                    break;
            }

            if (currentTime - gameTime.ElapsedGameTime.Milliseconds >0)
            {
                currentTime -= gameTime.ElapsedGameTime.Milliseconds;
                
                if(Status == UIItemStatus.Hover)
                {
                    alpha += 1;
                    //showing icon - FADE in effect
                    Color c = Color.White;
                    c = c * ((float)(maxTime - currentTime) / (float)maxTime);
                    ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, c);

                } else {
                    //hiding icon - FADE OUT effect
                    alpha -= 1;
                    Color c = Color.White;
                    c = c * ((float)currentTime / (float)maxTime);
                    ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, c);
                }

            }
            else if (Status == UIItemStatus.Hover)
            {
                ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, Color.White);
                currentTime = 0;
            }

        }
        #endregion
    }
}
