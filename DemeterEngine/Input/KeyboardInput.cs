#region License

// Copyright (c) 2015 FCDM
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region Header

/* Author: Michael Ala
 * Date of Creation: 6/12/2015
 * 
 * Description
 * ===========
 * The KeyboardInput object is a singleton that responds to keyboard input. It is like
 * MouseInput in that it is preferred over the builtin Monogame inputs because these
 * track more information.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#endregion

namespace DemeterEngine.Input
{
    public sealed class KeyboardInput : ChronometricObject
    {

        private static KeyboardInput Instance = new KeyboardInput();

        /// <summary>
        /// A dictionary mapping a Keys enumeration value to its appropriate
        /// index. This index is then used by framesSinceKeyPressed, 
        /// millisecondsSinceKeyPressed, framesSinceKeyUnpressed, and 
        /// millisecondsSinceKeyUnpressed.
        /// </summary>
        private Dictionary<Keys, int> keysToIndex = new Dictionary<Keys, int>();

        /// <summary>
        /// The number of frames each key has been pressed for.
        /// </summary>
        public static int[] FramesSinceKeyPressed
        {
            get
            {
                return Instance.framesSinceKeyPressed;
            }
        }
        private int[] framesSinceKeyPressed;

        /// <summary>
        /// The number of milliseconds each key has been pressed for.
        /// </summary>
        public static double[] MillisecondsSinceKeyPressed
        {
            get
            {
                return Instance.millisecondsSinceKeyPressed;
            }
        }
        private double[] millisecondsSinceKeyPressed;

        /// <summary>
        /// The number of frames each key has been unpressed for.
        /// </summary>
        public static int[] FramesSinceKeyUnpressed
        {
            get
            {
                return Instance.framesSinceKeyUnpressed;
            }
        }
        private int[] framesSinceKeyUnpressed;

        /// <summary>
        /// The number of milliseconds each key has been unpressed for.
        /// </summary>
        public static double[] MillisecondsSinceKeyUnpressed
        {
            get
            {
                return Instance.millisecondsSinceKeyUnpressed;
            }
        }
        private double[] millisecondsSinceKeyUnpressed;

        /// <summary>
        /// The current keyboard state.
        /// </summary>
        private KeyboardState currentKeyboardState;

        /// <summary>
        /// Prevent external instantiation, as this is a singleton.
        /// </summary>
        public KeyboardInput()
            : base(true)
        {
            var keyList = (Keys[])(Enum.GetValues(typeof(Keys)));
            for (int i = 0; i < keyList.Length; i++)
                keysToIndex[keyList[i]] = i;

            framesSinceKeyPressed = new int[keysToIndex.Count];
            framesSinceKeyUnpressed = new int[keysToIndex.Count];

            millisecondsSinceKeyPressed = new double[keysToIndex.Count];
            millisecondsSinceKeyUnpressed = new double[keysToIndex.Count];
        }

        /// <summary>
        /// Convert a key to an integer value representing it's index in the lists
        /// framesSinceKeyPressed, millisecondsSinceKeyPressed, framesSinceKeyUnpressed,
        /// and millisecondsSinceKeyUnpressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int KeyToInt(Keys key)
        {
            return Instance._KeyToInt(key);
        }
        private int _KeyToInt(Keys key)
        {
            return keysToIndex[key];
        }

        public static void Update()
        {
            Instance._Update();
        }
        private void _Update()
        {
            base.UpdateTime();
            currentKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            foreach (var pair in keysToIndex)
            {
                if (currentKeyboardState.IsKeyDown(pair.Key))
                {
                    framesSinceKeyPressed[pair.Value]++;
                    millisecondsSinceKeyPressed[pair.Value] += GlobalGameTimer.DeltaTime;

                    framesSinceKeyUnpressed[pair.Value] = 0;
                    millisecondsSinceKeyUnpressed[pair.Value] = 0;
                }
                else
                {
                    framesSinceKeyPressed[pair.Value] = 0;
                    millisecondsSinceKeyPressed[pair.Value] = 0;

                    framesSinceKeyUnpressed[pair.Value]++;
                    millisecondsSinceKeyUnpressed[pair.Value] += GlobalGameTimer.DeltaTime;
                }
            }
        }

        /// <summary>
        /// Check if a given key is pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsPressed(Keys key)
        {
            return Instance._IsPressed(key);
        }
        private bool _IsPressed(Keys key)
        {
            return framesSinceKeyPressed[keysToIndex[key]] > 0;
        }

        /// <summary>
        /// Check if a given key is unpressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsUnpressed(Keys key)
        {
            return Instance._IsUnpressed(key);
        }
        private bool _IsUnpressed(Keys key)
        {
            return framesSinceKeyUnpressed[keysToIndex[key]] > 0;
        }

        /// <summary>
        /// Check if a given key has been held down for the given number of frames or more.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsHeld(Keys key, int frames)
        {
            return Instance._IsHeld(key, frames);
        }
        private bool _IsHeld(Keys key, int frames)
        {
            return framesSinceKeyPressed[keysToIndex[key]] >= frames;
        }

        /// <summary>
        /// Check if a given key has been held down for the given number of milliseconds or more.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsHeld(Keys key, double milliseconds)
        {
            return Instance._IsHeld(key, milliseconds);
        }
        private bool _IsHeld(Keys key, double milliseconds)
        {
            return millisecondsSinceKeyPressed[keysToIndex[key]] >= milliseconds;
        }

        /// <summary>
        /// Check if a given key has been unheld for the given number of frames or more.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsUnheld(Keys key, int frames)
        {
            return Instance._IsUnheld(key, frames);
        }
        private bool _IsUnheld(Keys key, int frames)
        {
            return framesSinceKeyUnpressed[keysToIndex[key]] >= frames;
        }

        /// <summary>
        /// Check if a given key has been unheld for the given number of milliseconds or more.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsUnheld(Keys key, double milliseconds)
        {
            return Instance._IsUnheld(key, milliseconds);
        }
        private bool _IsUnheld(Keys key, double milliseconds)
        {
            return millisecondsSinceKeyUnpressed[keysToIndex[key]] >= milliseconds;
        }

        /// <summary>
        /// Check if a given key has been clicked (i.e. it has been held for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsClicked(Keys key)
        {
            return Instance._IsClicked(key);
        }
        private bool _IsClicked(Keys key)
        {
            return framesSinceKeyPressed[keysToIndex[key]] == 1;
        }

        /// <summary>
        /// Check if a given key has been released (i.e. it has been unheld for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsReleased(Keys key)
        {
            return Instance._IsReleased(key);
        }
        private bool _IsReleased(Keys key)
        {
            // Checking if LocalFrame != 1 ensures that IsReleased doesn't return true immediately
            // as soon as the game starts, which would normally occur unless the player was holding
            // down the key before the game began running.
            return framesSinceKeyUnpressed[keysToIndex[key]] == 1 && LocalFrame != 1;
        }

    }
}
