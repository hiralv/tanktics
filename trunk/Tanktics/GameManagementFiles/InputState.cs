//prebuilt sample code modified by Christopher Wykel
#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Tanktics
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
        }


        #endregion

        /// <summary>
        /// A bunch of checks that I have placed into the InputState.cs file allowing
        /// us to check to see if a key is being held down. --Christopher Wykel
        /// </summary>
        #region Keydown Checks

        /// <summary>
        /// A Check to see if the Escape Key is being held down. --Christopher Wykel
        /// </summary>
        public bool IsKeyDownEscape()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyDownEscape((PlayerIndex)i))
                    return true;
            }

            return false;
        }
        public bool IsKeyDownEscape(PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Escape) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Escape));
        }

        /// <summary>
        /// A Check to see if the Up Key is being held down. --Christopher Wykel
        /// </summary>
        public bool IsKeyDownUp()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyDownUp((PlayerIndex)i))
                    return true;
            }

            return false;
        }
        public bool IsKeyDownUp(PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Up) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Up));
        }

        /// <summary>
        /// A Check to see if the Left Key is being held down. --Christopher Wykel
        /// </summary>
        public bool IsKeyDownLeft()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyDownLeft((PlayerIndex)i))
                    return true;
            }

            return false;
        }
        public bool IsKeyDownLeft(PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Left) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Left));
        }
       
        /// <summary>
        /// A Check to see if the Right Key is being held down. --Christopher Wykel
        /// </summary>
        public bool IsKeyDownRight()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyDownRight((PlayerIndex)i))
                    return true;
            }

            return false;
        }
        public bool IsKeyDownRight(PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Right) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Right));
        }
    
        /// <summary>
        /// A Check to see if the Down Key is being held down. --Christopher Wykel
        /// </summary>
        public bool IsKeyDownDown()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyDownDown((PlayerIndex)i))
                    return true;
            }

            return false;
        }
        public bool IsKeyDownDown(PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Down) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Down));
        }

        #endregion

        /// <summary>
        /// A bunch of checks that I have placed into the InputState.cs file allowing
        /// us to check to see if a set of keys is pressed. some not used. --Christopher Wykel
        /// </summary>
        #region MenuChecks

        /// <summary>
        /// A Check to see if the LeftMenu Key is being held down. --Christopher Wykel
        /// </summary>
        public bool MenuLeft
        {
            get
            {
                return IsNewKeyPress(Keys.Left) ||
                       IsNewButtonPress(Buttons.DPadLeft) ||
                       IsNewButtonPress(Buttons.LeftThumbstickLeft);
            }
        }

        /// <summary>
        /// A Check to see if the RightMenu Key is being held down. --Christopher Wykel
        /// </summary>
        public bool MenuRight
        {
            get
            {
                return IsNewKeyPress(Keys.Right) ||
                       IsNewButtonPress(Buttons.DPadRight) ||
                       IsNewButtonPress(Buttons.LeftThumbstickRight);
            }
        }

        /// <summary>
        /// A Check to see if the UpMenu Key is being held down. --Christopher Wykel
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       IsNewButtonPress(Buttons.DPadUp) ||
                       IsNewButtonPress(Buttons.LeftThumbstickUp);
            }
        }

        /// <summary>
        /// A Check to see if the DownMenu Key is being held down. --Christopher Wykel
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       IsNewButtonPress(Buttons.DPadDown) ||
                       IsNewButtonPress(Buttons.LeftThumbstickDown);
            }
        }
        
        #endregion

        /// <summary>
        /// A bunch of checks that were already included in this document --Christopher Wykel
        /// </summary>
        #region Preset KeyChecks
        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       IsNewButtonPress(Buttons.A) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.B) ||
                       IsNewButtonPress(Buttons.Back);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.Back) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyPress(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonPress(button, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }


        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, playerIndex) ||
                   IsNewKeyPress(Keys.Enter, playerIndex) ||
                   IsNewButtonPress(Buttons.A, playerIndex) ||
                   IsNewButtonPress(Buttons.Start, playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action from the specified player.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, playerIndex) ||
                   IsNewButtonPress(Buttons.B, playerIndex) ||
                   IsNewButtonPress(Buttons.Back, playerIndex);
        }


        #endregion
    }
}
