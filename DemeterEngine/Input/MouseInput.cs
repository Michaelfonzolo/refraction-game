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
 * The MouseInput object is a singleton that tracks input from the mouse. It is
 * preferred over the builtin Monogame MouseInput because it records more than simply
 * the current state of the mouse (for example, it records the previous state of the mouse,
 * and the amount of time for which a button has been held down).
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DemeterEngine.Input
{

    public sealed class MouseInput : ChronometricObject
    {

        private static MouseInput Instance = new MouseInput();

        /// <summary>
        /// The number of frames since a mouse button was pressed. The
        /// first, second, and third elements of the array correspond to
        /// the left, middle, and right mouse buttons respectively.
        /// </summary>
        public static int[] FramesSinceMousePressed
        {
            get
            {
                return Instance.framesSinceMousePressed;
            }
        }
        private int[] framesSinceMousePressed;

        /// <summary>
        /// The number of milliseconds since a mouse button was pressed. The
        /// first, second, and third elements of the array correspond to the
        /// left, middle, and right mouse buttons respectively.
        /// </summary>
        public static double[] MillisecondsSinceMousePressed
        {
            get
            {
                return Instance.millisecondsSinceMousePressed;
            }
        }
        private double[] millisecondsSinceMousePressed;

        /// <summary>
        /// The number of frames since a mouse button was pressed. The
        /// first, second, and third elements of the array correspond to
        /// the left, middle, and right mouse buttons respectively.
        /// </summary>
        public static int[] FramesSinceMouseUnpressed
        {
            get
            {
                return Instance.framesSinceMouseUnpressed;
            }
        }
        private int[] framesSinceMouseUnpressed;

        /// <summary>
        /// The number of milliseconds since a mouse button was upressed. The
        /// first, second, and third elements of the array correspond to the
        /// left, middle, and right mouse buttons respectively.
        /// </summary>
        public static double[] MillisecondsSinceMouseUnpressed
        {
            get
            {
                return Instance.millisecondsSinceMouseUnpressed;
            }
        }
        private double[] millisecondsSinceMouseUnpressed;

        /// <summary>
        /// The current mouse state (type Microsoft.Xna.Framework.Input.MouseState).
        /// </summary>
        private MouseState currentMouseState;

        /// <summary>
        /// The stack of previous mouse positions. This stores the last second of
        /// mouse positions.
        /// </summary>
        public static Stack<Point> PrevMousePositions
        {
            get
            {
                return Instance.prevMousePositions;
            }
        }
        private Stack<Point> prevMousePositions;

        /// <summary>
        /// The number of frames since the last time the mouse moved.
        /// </summary>
        public static double FramesSinceMouseMovement
        {
            get
            {
                return Instance.framesSinceMouseMovement;
            }
        }
        private int framesSinceMouseMovement;

        /// <summary>
        /// The number of milliseconds since the last time the mouse moved.
        /// </summary>
        public static double MillisecondsSinceMouseMovement
        {
            get
            {
                return Instance.millisecondsSinceMouseMovement;
            }
        }
        private double millisecondsSinceMouseMovement;

        /// <summary>
        /// The previous number of frames since mouse movement.
        /// </summary>
        public static double PrevFramesSinceMouseMovement
        {
            get
            {
                return Instance.prevFramesSinceMouseMovement;
            }
        }
        private int prevFramesSinceMouseMovement;

        /// <summary>
        /// The previous number of milliseconds since mouse movement.
        /// </summary>
        public static double PrevMillisecondsSinceMouseMovement 
        { 
            get 
            { 
                return Instance.prevMillisecondsSinceMouseMovement; 
            } 
        }
        private double prevMillisecondsSinceMouseMovement;

        /// <summary>
        /// The position of the mouse.
        /// </summary>
        public static Point MousePosition { get { return Instance.mousePosition; } }
        private Point mousePosition;

        public static Vector2 MouseVector
        {
            get
            {
                return new Vector2(MousePosition.X, MousePosition.Y);
            }
        }

        /// <summary>
        /// The previous cumulative scroll wheel value.
        /// </summary>
        public static int PrevScrollWheelVal { get { return Instance.prevScrollWheelVal; } }
        private int prevScrollWheelVal;

        /// <summary>
        /// The cumulative scroll wheel value since the game began.
        /// </summary>
        public static int ScrollWheelValue { get { return Instance.scrollWheelValue; } }
        private int scrollWheelValue;

        /// <summary>
        /// Return the change in the scroll wheel value since the last call to Update.
        /// </summary>
        public static int DeltaScrollWheelValue { get { return ScrollWheelValue - PrevScrollWheelVal; } }

        public static double MouseSpeed
        {
            get
            {
                var _v = Instance.prevMousePositions.Peek();
                var v1 = new Vector2(_v.X, _v.Y);
                var v2 = MouseVector;
                return Vector2.Distance(v1, v2);
            }
        }

        /// <summary>
        /// Return the velocity of the mouse movement in
        /// </summary>
        public static Vector2 MouseVelocity
        {
            get
            {
                var _v = Instance.prevMousePositions.Peek();
                var v1 = new Vector2(_v.X, _v.Y);
                var v2 = MouseVector;
                return v2 - v1;
            }
        }

        /// <summary>
        /// Prevent external instantiation, as this is a singleton.
        /// </summary>
        public MouseInput()
            : base(true)
        {
            framesSinceMousePressed = new int[] { 0, 0, 0 };
            framesSinceMouseUnpressed = new int[] { 0, 0, 0 };
            millisecondsSinceMousePressed = new double[] { 0, 0, 0 };
            millisecondsSinceMouseUnpressed = new double[] { 0, 0, 0 };

            framesSinceMouseMovement = 0;
            millisecondsSinceMouseMovement = 0;
            prevFramesSinceMouseMovement = 0;
            prevMillisecondsSinceMouseMovement = 0;

            prevMousePositions = new Stack<Point>(60);

            prevScrollWheelVal = 0;
            scrollWheelValue = 0;
        }

        /// <summary>
        /// Convert a MouseButton into it's appropriate index in the list
        /// framesSinceMousePressed.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static int ButtonToIndex(MouseButtons button)
        {
            return Instance._ButtonToIndex(button);
        }
        private int _ButtonToIndex(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left: return 0;
                case MouseButtons.Middle: return 1;
                case MouseButtons.Right: return 2;
                default: return 0; // Should never occur.
            }
        }

        /// <summary>
        /// Update the mouse state to retrieve new input.
        /// </summary>
        public static void Update()
        {
            Instance._Update();
        }
        private void _Update()
        {
            base.UpdateTime();
            currentMouseState = Mouse.GetState();

            prevMousePositions.Push(mousePosition);
            mousePosition = currentMouseState.Position;

            if (mousePosition == prevMousePositions.First())
            {
                framesSinceMouseMovement++;
                millisecondsSinceMouseMovement += GlobalGameTimer.DeltaTime;
            }
            else
            {
                framesSinceMouseMovement = 0;
                millisecondsSinceMouseMovement = 0;
            }

            prevScrollWheelVal = scrollWheelValue;
            scrollWheelValue = currentMouseState.ScrollWheelValue;

            UpdateButton(MouseButtons.Left);
            UpdateButton(MouseButtons.Middle);
            UpdateButton(MouseButtons.Right);
        }

        /// <summary>
        /// Update an individual mouse button.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="position"></param>
        private void UpdateButton(MouseButtons button)
        {
            int pos = ButtonToIndex(button);
            if (IsPressed(button))
            {
                framesSinceMousePressed[pos]++;
                millisecondsSinceMousePressed[pos] += GlobalGameTimer.DeltaTime;

                framesSinceMouseUnpressed[pos] = 0;
                millisecondsSinceMouseUnpressed[pos] = 0;
            }
            else
            {
                framesSinceMousePressed[pos] = 0;
                millisecondsSinceMousePressed[pos] = 0;

                framesSinceMouseUnpressed[pos]++;
                millisecondsSinceMouseUnpressed[pos] += GlobalGameTimer.DeltaTime;
            }
        }

        /// <summary>
        /// Check if a given mouse button is pressed or not. A button is pressed
        /// iff it has been held for greater than or equal to 1 frame.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsPressed(MouseButtons button)
        {
            return Instance._IsPressed(button);
        }
        private bool _IsPressed(MouseButtons button)
        {
            // We can't just check if framesSinceMousePressed[ButtonToPosition(button)] > 0
            // because that would break UpdateButton.
            switch (button)
            {
                case MouseButtons.Left: return currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.Middle: return currentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Right: return currentMouseState.RightButton == ButtonState.Pressed;
                default: return false;
            }
        }

        /// <summary>
        /// Check if a mouse button has been held for a given number of frames or more.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsHeld(MouseButtons button, int frames)
        {
            return Instance._IsHeld(button, frames);
        }
        private bool _IsHeld(MouseButtons button, int frames)
        {
            return framesSinceMousePressed[ButtonToIndex(button)] >= frames;
        }

        /// <summary>
        /// Check if a mouse button has been held for a given number of milliseconds or more.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsHeld(MouseButtons button, double milliseconds)
        {
            return Instance._IsHeld(button, milliseconds);
        }
        private bool _IsHeld(MouseButtons button, double milliseconds)
        {
            return millisecondsSinceMousePressed[ButtonToIndex(button)] >= milliseconds;
        }

        /// <summary>
        /// Check if a mouse button has been unheld for a given number of frames or more.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsUnheld(MouseButtons button, int frames)
        {
            return Instance._IsUnheld(button, frames);
        }
        private bool _IsUnheld(MouseButtons button, int frames)
        {
            return framesSinceMouseUnpressed[ButtonToIndex(button)] >= frames;
        }

        /// <summary>
        /// Check if a mouse button has been unheld for a given number of milliseconds or more.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsUnheld(MouseButtons button, double milliseconds)
        {
            return Instance._IsUnheld(button, milliseconds);
        }
        private bool _IsUnheld(MouseButtons button, double milliseconds)
        {
            return millisecondsSinceMouseUnpressed[ButtonToIndex(button)] >= milliseconds;
        }

        /// <summary>
        /// Check if a mouse button has been clicked (i.e. it has been held for exactly one frame).
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsClicked(MouseButtons button)
        {
            return Instance._IsClicked(button);
        }
        private bool _IsClicked(MouseButtons button)
        {
            return framesSinceMousePressed[ButtonToIndex(button)] == 1;
        }

        /// <summary>
        /// Check if a mouse button has been released (i.e. it has been unpressed for exactly one frame).
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsReleased(MouseButtons button)
        {
            return Instance._IsReleased(button);
        }
        private bool _IsReleased(MouseButtons button)
        {
            // Checking if LocalFrame != 1 ensures that this doesn't return true immediately as
            // soon as the game begins, which would normally occur unless the player was pressing
            // the button before the game began.
            return framesSinceMouseUnpressed[ButtonToIndex(button)] == 1 && LocalFrame != 1;
        }

        /// <summary>
        /// Check if the mouse has not moved since the last call to Update.
        /// </summary>
        /// <returns></returns>
        public static bool IsMouseStill()
        {
            return Instance._IsMouseStill();
        }
        private bool _IsMouseStill()
        {
            return framesSinceMouseMovement > 0;
        }

        /// <summary>
        /// Check if the mouse has not moved for the given number of frames.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsMouseStill(int frames)
        {
            return Instance._IsMouseStill(frames);
        }
        private bool _IsMouseStill(int frames)
        {
            return framesSinceMouseMovement >= frames;
        }

        /// <summary>
        /// Check if the mouse has not moved for the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsMouseStill(double milliseconds)
        {
            return Instance._IsMouseStill(milliseconds);
        }
        private bool _IsMouseStill(double milliseconds)
        {
            return millisecondsSinceMouseMovement >= milliseconds;
        }

        /// <summary>
        /// Check if the mouse just began moving after being still for any variable period of time.
        /// </summary>
        /// <returns></returns>
        public static bool MouseJustMoved()
        {
            return Instance._MouseJustMoved();
        }
        private bool _MouseJustMoved()
        {
            return framesSinceMouseMovement == 1 &&
                   prevFramesSinceMouseMovement > 0;
        }

        /// <summary>
        /// Check if the mouse began moving after being still for the given number of frames or more.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool MouseMovedAfter(int frames)
        {
            return Instance._MouseMovedAfter(frames);
        }
        private bool _MouseMovedAfter(int frames)
        {
            return framesSinceMouseMovement == 0 &&
                   prevFramesSinceMouseMovement >= frames;
        }

        /// <summary>
        /// Check if the mouse began moving after being still for the given number of milliseconds or more.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool MouseMovedAfter(double milliseconds)
        {
            return Instance._MouseMovedAfter(milliseconds);
        }
        private bool _MouseMovedAfter(double milliseconds)
        {
            return millisecondsSinceMouseMovement == 0 &&
                   prevMillisecondsSinceMouseMovement >= milliseconds;
        }

        /// <summary>
        /// Check if the mouse has been idle for at least the given number of frames.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static bool IsIdle(int frames)
        {
            return Instance._IsIdle(frames);
        }
        private bool _IsIdle(int frames)
        {
            bool idle = framesSinceMouseMovement >= frames;
            foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
            {
                idle &= framesSinceMouseUnpressed[_ButtonToIndex(button)] >= frames;
            }
            return idle;
        }

        /// <summary>
        /// Check if the mouse has been idle for at least the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static bool IsIdle(double milliseconds)
        {
            return Instance._IsIdle(milliseconds);
        }
        private bool _IsIdle(double milliseconds)
        {
            bool idle = millisecondsSinceMouseMovement >= milliseconds;
            foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
            {
                idle &= millisecondsSinceMouseUnpressed[_ButtonToIndex(button)] >= milliseconds;
            }
            return idle;
        }
    }
}
