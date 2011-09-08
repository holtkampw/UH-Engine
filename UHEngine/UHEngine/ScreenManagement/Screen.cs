#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GermanGame.InputManagement;
#endregion


namespace GermanGame.ScreenManagement
{
    public enum ScreenStatus { Visible, Disabled, Overlay }

    public abstract class Screen
    {
        #region Class Variables
        public string Name;
        public ScreenStatus Status;
        public ScreenManager ScreenManager;
        #endregion

        #region Properties
        public bool IsVisible
        {
            get { return Status == ScreenStatus.Visible; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor that creates a screen base class
        /// </summary>
        /// <param name="name">This should be a unique way to identify each screen</param>
        public Screen(string name)
        {
            this.Name = name;
            this.Status = ScreenStatus.Disabled;
        }

        /// <summary>
        /// Sets the current screen display status
        /// </summary>
        /// <param name="status">Current screen status</param>
        public void SetStatus(ScreenStatus status)
        {
            this.Status = status;
        }
        #endregion

        #region Load and Unload Content
        public abstract void LoadContent();
        public abstract void UnloadContent();
        #endregion Load and Unload Content

        #region Update and Draw
        /// <summary>
        /// Function that contains code that will update the screen
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Handles input logic
        /// </summary>
        /// <param name="input">The input manager for the game</param>
        public abstract void HandleInput();

        /// <summary>
        /// Function that contains code to draw the current screen state
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public abstract void Draw(GameTime gameTime);

        public abstract void Reload();
        #endregion

        #region Helpers
        public void ExitScreen()
        {
            ScreenManager.RemoveScreen(this);
        }

        /// <summary>
        /// Reset the render states so spritebatch and models render correctly
        /// </summary>
        public void ResetRenderStates()
        {
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.BlendState = BlendState.Opaque;//AlphaBlend
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;//LinearClamp
        }
        #endregion

    }
}
