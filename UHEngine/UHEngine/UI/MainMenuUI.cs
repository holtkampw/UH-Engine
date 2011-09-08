using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GermanGame.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;

namespace GermanGame.UI
{
    class MainMenuUI : UIItem
    {
        public bool Previous = false;
        private int currentTime = 0;
        private int maxTime = 500;
        Texture2D icon = null;
        int alpha = 255;
        Vector2 iconPosition = Vector2.Zero;

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


        public override void Draw(GameTime gameTime)
        {
            switch (Status)
            {
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
                Console.WriteLine(currentTime);
                
                if(Status == UIItemStatus.Hover)
                {
                    alpha += 1;
                    //showing icon
                    Console.WriteLine("IN: " + 255 * ((float)(maxTime - currentTime) / (float)maxTime));
                    // (byte)MathHelper.Clamp((255.0f * ((float)(maxTime - currentTime)/(float)maxTime)), 0, 255)
                    Color c = Color.White;
                    c = c * ((float)(maxTime - currentTime) / (float)maxTime);
                    ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, c);

                } else {
                    //hiding icon
                    alpha -= 1;
                    Console.WriteLine("OUT: " + 255.0f * ((float)currentTime/(float)maxTime));
                    Color c = Color.White;
                    c = c * ((float)currentTime / (float)maxTime);
                    //(byte)MathHelper.Clamp((255.0f * ((float)currentTime/(float)maxTime)), 0, 255)
                    ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, c);
                }

            }
            else if (Status == UIItemStatus.Hover)
            {
                ScreenManager.SpriteBatch.Draw(this.icon, iconPosition, Color.White);
                currentTime = 0;
            }

        }
    }
}
