#if WINDOWS_PHONE

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHEngine.ScreenManagement;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace UHEngine.InputManagement
{
    public class VirtualThumbsticks
    {
        const int maxThumbstickDistance = 50;

        public Vector2 leftThumbstickCenter = Vector2.Zero;
        public Vector2 rightThumbstickCenter = Vector2.Zero;

        public Vector2 rightPosition, leftPosition = Vector2.Zero;

        Texture2D rtexture, ltexture, backStick;
        int distanceThumbsticks = 50;
        Vector2 rightCornerPosition, leftCornerPosition;
        Vector2 rightBackStick, leftBackStick;

        TouchLocation? leftTouch = null, rightTouch = null;
        TouchCollection touches;

        SpriteBatch spriteBatch;

        #region Properties
        public Vector2 LeftThumbstick
        {
            get
            {
                // calculate the scaled vector from the touch position to the center,
                // scaled by the maximum thumbstick distance
                Vector2 l = (leftPosition - leftThumbstickCenter) / maxThumbstickDistance;

                // if the length is more than 1, normalize the vector
                if (l.LengthSquared() > 1f)
                    l.Normalize();

                return l;
            }
        }

        public Vector2 RightThumbstick
        {
            get
            {
                // calculate the scaled vector from the touch position to the center,
                // scaled by the maximum thumbstick distance
                Vector2 l = (rightPosition - rightThumbstickCenter) / maxThumbstickDistance;

                // if the length is more than 1, normalize the vector
                if (l.LengthSquared() > 1f)
                    l.Normalize();

                return l;
            }
        }
        #endregion

        public VirtualThumbsticks()
        {
            backStick = ScreenManager.Game.Content.Load<Texture2D>("Thumbsticks/BackgroundStick");
            rtexture = ScreenManager.Game.Content.Load<Texture2D>("Thumbsticks/RStick");
            ltexture = ScreenManager.Game.Content.Load<Texture2D>("Thumbsticks/LStick");
            Vector2 middleTexture = new Vector2(rtexture.Width / 2, rtexture.Height / 2);

            rightThumbstickCenter = new Vector2(ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth - distanceThumbsticks - middleTexture.X,
                                                                   ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight - distanceThumbsticks - middleTexture.Y);
            leftThumbstickCenter = new Vector2(distanceThumbsticks + middleTexture.X,
                                                                  ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight - distanceThumbsticks - middleTexture.Y);

            rightCornerPosition = rightThumbstickCenter - middleTexture;
            leftCornerPosition = leftThumbstickCenter - middleTexture;

            rightBackStick = rightCornerPosition - new Vector2(distanceThumbsticks, distanceThumbsticks);
            leftBackStick = leftCornerPosition - new Vector2(distanceThumbsticks, distanceThumbsticks);

            spriteBatch = ScreenManager.SpriteBatch;
        }

        public void Update()
        {
            leftTouch = null;
            rightTouch = null;
            touches = TouchPanel.GetState();

            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].Position.X <= TouchPanel.DisplayWidth / 2)
                {
                    leftTouch = touches[i];
                    leftPosition = touches[i].Position;
                    continue;
                }

                if (touches[i].Position.X > TouchPanel.DisplayWidth / 2 && touches[i].Position.Y > 90)
                {
                    rightTouch = touches[i];
                    rightPosition = touches[i].Position;
                    continue;
                }

                if (leftTouch.HasValue && rightTouch.HasValue)
                    break;
            }

            if (!leftTouch.HasValue)
                leftPosition = leftThumbstickCenter;

            if (!rightTouch.HasValue)
                rightPosition = rightThumbstickCenter;
        }

        public void Draw()
        {
            spriteBatch.Begin();
            if (RightThumbstick.Length() > 0)
                spriteBatch.Draw(backStick, rightBackStick, Color.White);

            spriteBatch.Draw(rtexture, rightCornerPosition + RightThumbstick * distanceThumbsticks, Color.White);

            if (LeftThumbstick.Length() > 0)
                spriteBatch.Draw(backStick, leftBackStick, Color.White);

            spriteBatch.Draw(ltexture, leftCornerPosition + LeftThumbstick * distanceThumbsticks, Color.White);
            spriteBatch.End();
        }
    }
}
#endif