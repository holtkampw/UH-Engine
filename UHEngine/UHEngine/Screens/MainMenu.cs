
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHEngine.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHEngine.InputManagement;
using UHEngine.UI;

namespace UHEngine.Screens
{

    class MainMenu : Screen
    {

        #region Fields
        SpriteFont menuFont;
        Texture2D background;
        List<MenuItem> menuItems = new List<MenuItem>();
        Vector2 baseLocation = new Vector2(20, 80);
        Vector2 offset = new Vector2(0, 90);
        Vector2 currentPosition;
        Vector2 oneOffset = new Vector2(0, 2);
        Vector2 largeIconPosition = new Vector2(200, 0);
        Rectangle backgroundRect = new Rectangle(0, 0, ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth, ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight);

        List<MainMenuUI> items = new List<MainMenuUI>();
        #endregion

        #region Initialization
        public MainMenu() : base("MainMenu")
        {

        }
        #endregion

        #region LoadContent
        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>(@"Menu\dark_brick_wall");
            menuFont = ScreenManager.Game.Content.Load<SpriteFont>(@"Menu\menuItems");

            currentPosition = baseLocation;
            menuItems.Add(new MenuItem("Play", DoNothing, currentPosition));

            items.Add(new MainMenuUI(ScreenManager.Game.Content.Load<Texture2D>(@"Menu\playGameIcon"), largeIconPosition, ScreenManager.Game.Content.Load<Texture2D>(@"Menu\playGame"), currentPosition, 260, 80));
            items[items.Count - 1].OurAction += () => {ScreenManager.ShowScreen(new PlayGame()); };
            currentPosition += offset;

            menuItems.Add(new MenuItem("Credits", DoNothing, currentPosition));
            items.Add(new MainMenuUI(ScreenManager.Game.Content.Load<Texture2D>(@"Menu\creditsIcon"), largeIconPosition, ScreenManager.Game.Content.Load<Texture2D>(@"Menu\credits"), currentPosition, 260, 80));
            items[items.Count - 1].OurAction += DoNothing;
            currentPosition += offset;

            menuItems.Add(new MenuItem("Exit", Exit, currentPosition));
            items.Add(new MainMenuUI(ScreenManager.Game.Content.Load<Texture2D>(@"Menu\exitIcon"), largeIconPosition, ScreenManager.Game.Content.Load<Texture2D>(@"Menu\exit"), currentPosition, 260, 80));
            items[items.Count - 1].OurAction += Exit;
            ResetButtonStates();

        }

        public override void Reload()
        {
            
        }
        #endregion

        #region Unloading
        public override void UnloadContent()
        {
            
        }
        #endregion

        #region Draw
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            ScreenManager.SpriteBatch.Draw(background, backgroundRect, Color.White);

            
            for (int i = 0; i < items.Count; i++)
            {
                currentPosition += offset;             
                items[i].Draw(gameTime);
            }

            ScreenManager.SpriteBatch.End();
        }
        #endregion

        #region Update
        public override void HandleInput()
        {
            ScreenManager.InputManager.Update();

            if (ScreenManager.InputManager.CheckNewReleaseAction(InputAction.ExitGame))
            {
                ScreenManager.Game.Exit();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Point mouse = new Point((int)ScreenManager.Cursor.Position.X,
               (int)ScreenManager.Cursor.Position.Y);

            Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 10, 10);
                //Check Items
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Bounds.Intersects(mouseRect))
                {
                    //Check Mouse Click To Pick Right Status
                    if (ScreenManager.Cursor.IsLeftMouseButtonPressed())
                    {
                        items[i].SetStatus(UIItemStatus.Click);
                        items[i].OurAction();
                    }
                    else
                    {
                        items[i].SetStatus(UIItemStatus.Hover);
                    }
                    continue;
                }
                else
                {
                    items[i].SetStatus(UIItemStatus.Inactive);
                }
                
            }
        }
        #endregion

        #region Actions

        public void DoNothing()
        {

        }

        public void Exit()
        {
            ScreenManager.Game.Exit();
        }

        #endregion

        #region Helpers
        private void ResetButtonStates()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Status = UIItemStatus.Inactive;
            }
        }
        #endregion
    }
}
