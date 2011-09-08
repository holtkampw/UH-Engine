using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GermanGame.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;
using GermanGame.CoreObjects;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace GermanGame.Screens
{
    class ItemDetailScreen : Screen
    {
        FindableObject model;
        SpriteFont font;

        public ItemDetailScreen(FindableObject model)
            : base("ItemDetail") 
        {
            this.model = model;
            model.IsFound = true;
            this.SetStatus(ScreenStatus.Overlay);
        }

        public override void LoadContent()
        {
            font = ScreenManager.Game.Content.Load<SpriteFont>("Menu\\menuItems");
        }

        public override void UnloadContent()
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public override void HandleInput()
        {
            ScreenManager.InputManager.Update();

            if (ScreenManager.InputManager.CheckNewReleaseAction(InputManagement.InputAction.PlaySound))
            {
                MediaPlayer.Play(model.SoundByte);
            }

            if (ScreenManager.InputManager.CheckNewReleaseAction(InputManagement.InputAction.ExitGame))
            {
                ScreenManager.RemoveScreen(this);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(font, model.GermanName, new Vector2(50, 50), Color.White);
            ScreenManager.SpriteBatch.End();
        }

        public override void Reload()
        {

        }
    }
}
