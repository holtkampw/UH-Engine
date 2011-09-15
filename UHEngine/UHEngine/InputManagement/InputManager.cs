#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using UHEngine.EnumHelper;
#endregion


namespace UHEngine.InputManagement
{
    public enum InputAction
    {
        Selection,
        MenuSelect, MenuCancel,
        ExitGame, BackToMainMenu, StartGame,
        Pause,
        MoveLeft, MoveRight, MoveUp, MoveDown,
        RotateLeft, RotateRight,
        LookUp,
        LookDown, PlaySound
    };

    public sealed class InputManager
    {

        #region Class Variables
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        MouseState currentMouseState;
        MouseState previousMouseState;
        Texture2D mouseCursorTexture;

        List<GamePadState> currGamePadStates;
        List<GamePadState> prevGamePadStates;
        List<List<Keys>> keyActionList;
        List<List<Buttons>> buttonActionList;

        List<Keys> tempKeys;
        List<Buttons> tempButtons;

        Enum[] inputActionsArray = EnumHelper.EnumHelper.EnumToArray(new InputAction());

        PlayerIndex[] playerIndexes;
        #endregion

        #region Initialization
        public InputManager()
        {
            previousKeyboardState = new KeyboardState();
            currentKeyboardState = new KeyboardState();

            previousMouseState = new MouseState();
            currentMouseState = new MouseState();
            mouseCursorTexture = ScreenManagement.ScreenManager.Game.Content.Load<Texture2D>("cursor");

            playerIndexes = new PlayerIndex[4];
            playerIndexes[0] = PlayerIndex.One;
            playerIndexes[1] = PlayerIndex.Two;
            playerIndexes[2] = PlayerIndex.Three;
            playerIndexes[3] = PlayerIndex.Four;

            //New AwesomeNess
            currGamePadStates = new List<GamePadState>();
            prevGamePadStates = new List<GamePadState>();
            keyActionList = new List<List<Keys>>();
            buttonActionList = new List<List<Buttons>>();

            for (int i = 0; i < inputActionsArray.Length; i++)
            {
                keyActionList.Add(new List<Keys>());
                buttonActionList.Add(new List<Buttons>());
            }

            for (int i = 0; i < 4; i++)
            {
                currGamePadStates.Add(new GamePadState());
                prevGamePadStates.Add(new GamePadState());
            }
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates previous and current input devices
        /// </summary>
        public void Update()
        {
            //GamePad.GetState(PlayerIndex.One).Buttons.
#if !XBOX
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

#else
            for (int i = 0; i < playerIndexes.Length; i++)
            {
                prevGamePadStates[i] = currGamePadStates[i];
                currGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
#endif
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="key">The key to trigger the action</param>
        public void AddInput(InputAction action, Keys key)
        {
            tempKeys = keyActionList[(int)action];
            if (tempKeys.Count > 0)
            {
                if (!tempKeys.Contains(key))
                    tempKeys.Add(key);
            }
            else
            {
                tempKeys.Add(key);
            }

        }

        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="button">The button to trigger the action</param>
        public void AddInput(InputAction action, Buttons button)
        {
            tempButtons = buttonActionList[(int)action];
            if (tempButtons.Count > 0)
            {
                if (tempButtons.Contains(button))
                    tempButtons.Add(button);
            }
            else
            {
                tempButtons.Add(button);
            }
        }

        /// <summary>
        /// Checks if the action has just been triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the action has just been triggered</returns>
        public bool CheckNewAction(InputAction action)
        {
            return CheckNewAction(action, null);
        }

        /// <summary>
        /// Checks if the action has just been triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the action has just been triggered</returns>
        public bool CheckNewAction(InputAction action, PlayerIndex? playerIndex)
        {

#if XBOX
            tempButtons = buttonActionList[(int)action];
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < tempButtons.Count; j++)
                        if (IsNewButtonPressed(tempButtons[j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < tempButtons.Count; j++)
                    if (IsNewButtonPressed(tempButtons[j], playerIndex.Value))
                        return true;
            }
#else
            if (playerIndex.HasValue && playerIndex.Value != PlayerIndex.One)
                return false;

            tempKeys = keyActionList[(int)action];
            for (int i = 0; i < tempKeys.Count; i++)
                if (IsNewKeyPressed(tempKeys[i]))
                    return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks if the action has is triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the action is triggered</returns>
        public bool CheckAction(InputAction action)
        {
            return CheckAction(action, null);
        }

        /// <summary>
        /// Checks if the action has is triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the action is triggered</returns>
        public bool CheckAction(InputAction action, PlayerIndex? playerIndex)
        {

#if XBOX
            tempButtons = buttonActionList[(int)action];
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < tempButtons.Count; j++)
                        if (IsButtonPressed(tempButtons[j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < tempButtons.Count; j++)
                    if (IsButtonPressed(tempButtons[j], playerIndex.Value))
                        return true;
            }

#else
            if (playerIndex.HasValue && playerIndex.Value != PlayerIndex.One)
                return false;

            tempKeys = keyActionList[(int)action];
            for (int i = 0; i < tempKeys.Count; i++)
                if (IsKeyPressed(tempKeys[i]))
                    return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks if the action has just been released
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the aciton has just been released</returns>
        public bool CheckReleaseAction(InputAction action)
        {
            return CheckReleaseAction(action, null);
        }

        public bool CheckNewReleaseAction(InputAction action)
        {
            return CheckNewReleaseAction(action, null);
        }

        public bool CheckNewReleaseAction(InputAction action, PlayerIndex? playerIndex)
        {
#if XBOX
            tempButtons = buttonActionList[(int)action];
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < tempButtons.Count; j++)
                        if (IsNewButtonReleased(tempButtons[j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < tempButtons.Count; j++)
                    if (IsNewButtonReleased(tempButtons[j], playerIndex.Value))
                        return true;
            }
#else

            if (playerIndex.HasValue && playerIndex.Value != PlayerIndex.One)
                return false;

            tempKeys = keyActionList[(int)action];
            for (int i = 0; i < tempKeys.Count; i++)
                if (IsNewKeyReleased(tempKeys[i]))
                    return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks if the action has just been released
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the aciton has just been released</returns>
        public bool CheckReleaseAction(InputAction action, PlayerIndex? playerIndex)
        {
            return true;
        }

        #region ButtonHelpers
        private bool IsNewButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            return IsButtonPressed(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonUp(button);
        }

        private bool IsNewButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return IsButtonReleased(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        private bool IsButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            return currGamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        private bool IsButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return !IsButtonPressed(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonDown(button);
        }
        #endregion ButtonHelpers

        #region KeyHelpers
        private bool IsNewKeyPressed(Keys key)
        {
            return IsKeyPressed(key) && previousKeyboardState.IsKeyUp(key);
        }

        private bool IsNewKeyReleased(Keys key)
        {
            return IsKeyReleased(key) && previousKeyboardState.IsKeyDown(key);
        }

        private bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        private bool IsKeyReleased(Keys key)
        {
            return !IsKeyPressed(key) && previousKeyboardState.IsKeyDown(key);
        }
        #endregion KeyHelpers

        #region Mouse Helpers

        public Point GetPreviousMousePos()
        {
            return new Point(previousMouseState.X, previousMouseState.Y);
        }

        public Point GetCurrentMousePos()
        {
            return new Point(currentMouseState.X, currentMouseState.Y);
        }

        public bool IsLeftMouseButtonPressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsLeftMouseButtonReleased()
        {
            return currentMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsLeftMouseButtonJustClicked()
        {
            return IsLeftMouseButtonPressed() && previousMouseState.LeftButton == ButtonState.Released;
        }

        public bool isLeftMouseButtonJustReleased()
        {
            return IsLeftMouseButtonReleased() && previousMouseState.LeftButton == ButtonState.Pressed;
        }

        #endregion Mouse Helpers



        #endregion
    }
}
