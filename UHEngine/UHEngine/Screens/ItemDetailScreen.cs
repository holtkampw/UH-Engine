using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHEngine.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;
using UHEngine.CoreObjects;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace UHEngine.Screens
{
    class ItemDetailScreen : Screen
    {
        #region Fields
        FindableObject model;
        SpriteFont font;
        #endregion

        #region Initialization
        public ItemDetailScreen(FindableObject model)
            : base("ItemDetail") 
        {
            this.model = model;
            model.IsFound = true;
            this.SetStatus(ScreenStatus.Overlay);
        }
        #endregion

        #region LoadContent
        public override void LoadContent()
        {
            font = ScreenManager.Game.Content.Load<SpriteFont>("Menu\\menuItems");
        }

        public override void UnloadContent()
        {

        }
        #endregion

        #region Update
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
        #endregion

        #region Draw
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(font, model.GermanName, new Vector2(50, 50), Color.White);
            ScreenManager.SpriteBatch.End();
        }

        public override void Reload()
        {

        }
        #endregion
    }
}
