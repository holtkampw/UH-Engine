
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GermanGame.ScreenManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GermanGame.InputManagement;
using GermanGame.UI;

namespace GermanGame.Screens
{

    class MainMenu : Screen
    {

        #region Variables
        SpriteFont menuFont;
        Texture2D background;
        List<MenuItem> menuItems = new List<MenuItem>();
        Vector2 baseLocation = new Vector2(100, 300);
        Vector2 offset = new Vector2(0, 90);
        Vector2 currentPosition;
        Vector2 oneOffset = new Vector2(0, 2);
        Vector2 largeIconPosition = new Vector2(700, 200);

        List<MainMenuUI> items = new List<MainMenuUI>();
        #endregion

        #region Initialization
        public MainMenu() : base("MainMenu")
        {

        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>(@"Menu\mainMenu");
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

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);

            
            for (int i = 0; i < items.Count; i++)
            {
               // ScreenManager.SpriteBatch.DrawString(menuFont, menuItems[i].Name, menuItems[i].Position, Color.White);
                currentPosition += offset;
                
                items[i].Draw(gameTime);
            }

            ScreenManager.SpriteBatch.End();
        }

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
            //ResetButtonStates();
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

        #region Actions

        public void DoNothing()
        {

        }

        public void Exit()
        {
            ScreenManager.Game.Exit();
        }

        #endregion

        private void ResetButtonStates()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Status = UIItemStatus.Inactive;
                items[i].Found = true; //FIX THIS - Remove before Done! (not all items are always found)
            }
        }
    }
}
