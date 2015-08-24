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
 * 
 * Description
 * ===========
 * A form representing an arbitrary button that can be interacted with by the mouse.
 * It contains information about whether or not the mouse is colliding with it, whether
 * or not it is pressed, and whether or not it is just released (i.e. released for a
 * single frame).
 */

#endregion

#region Using Statements

using DemeterEngine.Collision;
using DemeterEngine.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DemeterEngine.Multiforms.Forms
{
    public class ButtonForm : Form
    {

        private Collidable _collider;

		/// <summary>
		/// The collider representing the area the mouse can click.
		/// </summary>
        public Collidable Collider 
        {
            get { return _collider; } 
            set 
            {
                if (Begun)
                    throw new ArgumentException("Cannot set collider after form has begun updating.");
                _collider = value;
            } 
        }

		/// <summary>
		/// Whether or not the mouse is colliding with this button.
		/// </summary>
        public bool CollidingWithMouse { get; internal set; }

        /// <summary>
        /// This flag is true for the first frame that the mouse enters the button's collider.
        /// </summary>
        public bool MouseEntered { get { return MouseCollidingFor == 1; } }

        /// <summary>
        /// How many frames the mouse has been colliding with this button for.
        /// </summary>
        public int MouseCollidingFor { get; private set; }

        /// <summary>
        /// Whether or not interaction with this button is locked.
        /// </summary>
        public bool Locked { get; private set; }

		/// <summary>
		/// A dictionary representing the held down states of each mouse button. For example, if the
		/// user is holding down the Left mouse button whilst colliding with this button, then the
		/// item in this dictionary with key MouseButtons.Left will have value true.
		/// </summary>
        private Dictionary<MouseButtons, bool> _HeldDownButtons = new Dictionary<MouseButtons, bool>()
        {
            {MouseButtons.Left,   false},
            {MouseButtons.Middle, false},
            {MouseButtons.Right,  false}
        };

		/// <summary>
		/// A dictionary representing the released states of each mouse button. For example, if the
		/// user has released the Left mouse button whilst colliding with this button, then the
		/// item in this dictionary with key MouseButtons.Left will have value true.
		/// </summary>
        private Dictionary<MouseButtons, bool> _ReleasedButtons = new Dictionary<MouseButtons, bool>()
        {
            {MouseButtons.Left,   false},
            {MouseButtons.Middle, false},
            {MouseButtons.Right,  false}
        };

        public ButtonForm(Collidable collider, bool keepTime = true)
            : base(keepTime)
        {
            Collider = collider;
            CollidingWithMouse = false;
            MouseCollidingFor = 0;
        }

        public void LockInteraction()
        {
            Locked = true;
            CollidingWithMouse = false;
        }

        public void UnlockInteraction()
        {
            Locked = false;
        }

        private void UpdateButton(MouseButtons button)
        {
            _ReleasedButtons[button] = false;

            if (Locked)
                return;

            if (CollidingWithMouse)
            {
                if (MouseInput.IsClicked(button))
                    _HeldDownButtons[button] = true;
                if (_HeldDownButtons[button] && MouseInput.IsReleased(button))
                {
                    _HeldDownButtons[button] = false;
                    _ReleasedButtons[button] = true;
                }
            }
            else
            {
                _HeldDownButtons[button] = false;
            }
        }

        public bool IsHeldDown(MouseButtons button)
        {
            return _HeldDownButtons[button];
        }

        public bool IsReleased(MouseButtons button)
        {
            return _ReleasedButtons[button];
        }

        public override void Update()
        {
            base.Update();

			if (Collider == null)
				return;

            if (!Locked)
            {
                var response =
                    Collider.CollidingWith(
                        new PointCollider(MouseInput.MousePosition));
                CollidingWithMouse = response == null ? false : response.Colliding;
                if (CollidingWithMouse)
                {
                    MouseCollidingFor++;
                }
                else
                {
                    MouseCollidingFor = 0;
                }
            }

            foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
                UpdateButton(button);
        }

    }
}
