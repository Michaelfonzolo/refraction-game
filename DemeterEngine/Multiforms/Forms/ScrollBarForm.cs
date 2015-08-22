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
 * A form representing an arbitrary orthogonal scroll bar.
 */

#endregion

#region Using Statements

using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Maths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

#endregion

namespace DemeterEngine.Multiforms.Forms
{
	public class ScrollBarForm : Form
    {

        #region Default Values

        public static readonly MouseButtons DefaultButtonToPress        = MouseButtons.Left;
        public static readonly Keys? DefaultVerticalScrollActivateKey   = null;
        public static readonly Keys? DefaultHorizontalScrollActivateKey = Keys.LeftShift;
        
        public static readonly Keys? DefaultVerticalScrollPlusKey       = Keys.Down;
        public static readonly Keys? DefaultVerticalScrollMinusKey      = Keys.Up;
        public static readonly Keys? DefaultHorizontalScrollPlusKey     = Keys.Right;
        public static readonly Keys? DefaultHorizontalScrollMinusKey    = Keys.Left;

        public static readonly double DefaultJumpAmount             = 0.25;
        public static readonly double DefaultMouseWheelScrollAmount = 0.01;
        public static readonly double DefaultKeyboardScrollAmount   = 0.05;
        public static readonly double DefaultScrollWheelDamper      = 30;

        #endregion

        /// <summary>
		/// The collider for the entire scroll bar.
		/// </summary>
		public RectCollider ScrollBarCollider { get; private set; }

		/// <summary>
		/// The collider for the thumb of the scroll bar.
		/// </summary>
		public RectCollider ScrollThumbCollider { get; private set; }

		/// <summary>
		/// Whether the scroll bar is vertical (true) or horizontal (false).
		/// </summary>
		public bool Vertical { get; private set; }

		/// <summary>
		/// The "scroll value" is a value between 0 and 1 representing the thumb's
		/// position on the scroll bar. A value of 0 means the thumb is at one side
		/// of the scroll bar and a value of 1 means the thumb is at the opposite side.
		/// 
		/// If the scroll bar is vertical, scroll value 0 is the top of the scroll bar.
		/// If the scroll bar is horizontal, scroll value 0 is the left side of the scroll
		/// bar.
		/// </summary>
		public double ScrollValue
		{
			get
			{
				if (Vertical)
				{
					return (ScrollThumbCollider.Y - ScrollBarCollider.Y)
						 / (ScrollBarCollider.H - ScrollThumbCollider.H);
				}
				else
				{
					return (ScrollThumbCollider.X - ScrollBarCollider.X)
						/ (ScrollBarCollider.W - ScrollThumbCollider.W);
				}
			}
		}

        /// <summary>
        /// The length of the area the scroll bar can scroll through. This is NOT
        /// the length of the scroll bar itself.
        /// </summary>
        public double Span { get; private set; }

		/// <summary>
		/// The amount of room the thumb has to move around in the scroll bar.
		/// </summary>
		public double Range
		{
			get
			{
				return Vertical ? ScrollBarCollider.H - ScrollThumbCollider.H
								: ScrollBarCollider.W - ScrollThumbCollider.W;
			}
		}

        /// <summary>
        /// The value of Span * ScrollValue.
        /// </summary>
        public double SpanScrollValue
        {
            get
            {
                return ScrollValue * Span;
            }
        }

        /// <summary>
        /// The value of Range * ScrollValue.
        /// </summary>
        public double RangeScrollValue
        {
            get
            {
                return ScrollValue * Range;
            }
        }

        private double prevScrollValue;

        /// <summary>
        /// The amount by which ScrollValue has changed since the last call to update.
        /// </summary>
        public double DeltaScrollValue
        {
            get
            {
                return ScrollValue - prevScrollValue;
            }
        }

        /// <summary>
        /// The value of Span * DeltaScrollValue.
        /// </summary>
        public double DeltaSpanScrollValue
        {
            get
            {
                return DeltaScrollValue * Span;
            }
        }

        /// <summary>
        /// The value of Range * DeltaScrollValue.
        /// </summary>
        public double DeltaRangeScrollValue
        {
            get
            {
                return DeltaScrollValue * Range;
            }
        }

		/// <summary>
		/// Whether or not the scroll thumb is colliding with the mouse.
		/// </summary>
		public bool ThumbCollidingWithMouse { get; private set; }

		/// <summary>
		/// Whether or not the entire scroll bar is colliding with the mouse.
		/// </summary>
		public bool CollidingWithMouse { get; private set; }

		/// <summary>
		/// Whether or not the scroll shaft is colliding with the mouse.
		/// </summary>
		public bool ShaftCollidingWithMouse
		{
			get
			{
				return CollidingWithMouse && !ThumbCollidingWithMouse;
			}
		}

		/// <summary>
		/// The mouse button the user has to be pressing in order to use the scroll bar.
		/// </summary>
		public MouseButtons ButtonToPress { get; set; }

        /// <summary>
        /// The key the user has to press to increase the scroll value (or null).
        /// </summary>
        public Keys? ScrollPlusKey { get; set; }

        /// <summary>
        /// The key the user has to press to decrease the scroll value (or null).
        /// </summary>
        public Keys? ScrollMinusKey { get; set; }

		/// <summary>
		/// The keyboard key the user has to be holding down in order to scroll with the
		/// mouse wheel. This is so that you can implement horizontal scroll bars which
		/// require the user to press "shift" to use.
		/// </summary>
		public Keys? ScrollActivateKey { get; set; }

		/// <summary>
		/// The amount by which the scroll thumb jumps when the user clicks on the scroll
		/// bar shaft.
		/// </summary>
		public double JumpAmount { get; set; }

		/// <summary>
		/// The amount by which scrolling the mouse wheel will scroll the thumb.
		/// </summary>
		public double MouseWheelScrollAmount { get; set; }

        /// <summary>
        /// The amount by which to divide the delta scroll wheel value.
        /// </summary>
        public double ScrollWheelDampener { get; set; }

        /// <summary>
        /// The amount by which using the scroll plus/minus keys scrolls the thumb.
        /// </summary>
        public double KeyboardScrollAmount { get; set; }

        /// <summary>
        /// The initial value of the scroll thumb. The value will be clamped between 0 and 1.
        /// </summary>
        public double InitialScrollValue
        {
            set
            {
                if (Begun)
                {
                    throw new ArgumentException("Cannot set initial scroll value; form has already begun updating.");
                }
                if (Vertical)
                {
                    ScrollThumbCollider.Translate(0, value * Range);
                }
                else
                {
                    ScrollThumbCollider.Translate(value * Range, 0);
                }
                ClampThumbPosition();
            }
        }

        /// <summary>
        /// The initial value of the scroll thumb unclamped.
        /// </summary>
        public double InitialScrollValueUnclamped
        {
            set
            {
                if (Begun)
                {
                    throw new ArgumentException("Cannot set initial scroll value; form has already begun updating.");
                }
                if (Vertical)
                {
                    ScrollThumbCollider.Translate(0, value * Range);
                }
                else
                {
                    ScrollThumbCollider.Translate(value * Range, 0);
                }
            }
        }

		/// <summary>
		/// Whether or not the scroll bar is locked. If it is locked, the user cannot interact
		/// with it, otherwise they can.
		/// </summary>
		public bool Locked { get; private set; }

		/// <summary>
		/// Whether or not the scroll thumb is in the middle of being dragged by the mouse.
		/// </summary>
		public bool Dragging { get; private set; }

		private double prevDragPos = 0;

        public ScrollBarForm(
            Vector2 topLeft, int scrollBarLength, int spanLength, 
            int scrollBarWidth, bool vertical, bool keepsTime = true)
            : base(keepsTime)
		{
            Locked = false;
			Vertical = vertical;
            Span = spanLength - scrollBarLength;

			// Let Sl = scrollBarLength, St = scrollThumbLength, and T = spanLength.
			// Then St/Sl = Sl/T, so St = Sl^2/T.
			var scrollThumbLength = scrollBarLength * scrollBarLength / spanLength;

			// Clamp it so that the scroll thumb can only be as long as the scroll bar.
			scrollThumbLength = Math.Min(scrollThumbLength, scrollBarLength);

			if (vertical)
			{
				ScrollBarCollider   = new RectCollider(topLeft, scrollBarWidth, scrollBarLength);
				ScrollThumbCollider = new RectCollider(topLeft, scrollBarWidth, scrollThumbLength);
			}
			else
			{
				ScrollBarCollider   = new RectCollider(topLeft, scrollBarLength, scrollBarWidth);
				ScrollThumbCollider = new RectCollider(topLeft, scrollThumbLength, scrollBarWidth);
			}

			ClampThumbPosition();

            // Initialize the default values.

            ButtonToPress          = DefaultButtonToPress;
            ScrollActivateKey      = Vertical ? DefaultVerticalScrollActivateKey 
                                              : DefaultHorizontalScrollActivateKey;

			JumpAmount             = DefaultJumpAmount;
			MouseWheelScrollAmount = DefaultMouseWheelScrollAmount;
            KeyboardScrollAmount = DefaultKeyboardScrollAmount;
            ScrollWheelDampener    = DefaultScrollWheelDamper;

            ScrollPlusKey          = Vertical ? DefaultVerticalScrollPlusKey
                                              : DefaultHorizontalScrollPlusKey;

            ScrollMinusKey         = Vertical ? DefaultVerticalScrollMinusKey
                                              : DefaultHorizontalScrollMinusKey;
		}

		/// <summary>
		/// Clamp the thumb position so that it doesn't extend past the 
		/// </summary>
		public void ClampThumbPosition()
		{
			double clampedVal;
			if (Vertical)
			{
				clampedVal = MathUtils.Clamp(ScrollThumbCollider.Y, ScrollBarCollider.Y, ScrollBarCollider.Y + Range);
				ScrollThumbCollider.SetPosition(ScrollThumbCollider.X, clampedVal);
			}
			else
			{
				clampedVal = MathUtils.Clamp(ScrollThumbCollider.X, ScrollBarCollider.X, ScrollBarCollider.X + Range);
				ScrollThumbCollider.SetPosition(clampedVal, ScrollThumbCollider.Y);
			}
		}

		/// <summary>
		/// Lock scroll bar interaction.
		/// </summary>
		public void Lock()
		{
			Locked = true;
		}

		/// <summary>
		/// Unlock scroll bar interaction.
		/// </summary>
		public void Unlock()
		{
			Locked = false;
		}

		/// <summary>
		/// Scroll the thumb by the given amount. If `clamp` is true, then the thumb will be clamped 
		/// between the boundaries of the scroll bar. If `absolute` is false, then the given amount 
		/// is interpretted as a percentage.
		/// </summary>
		public virtual void Scroll(
			double amount, bool absolute = false, bool clamp = true, bool overrideLock = false)
		{
			if (Locked && !overrideLock)
			{
				return;
			}
			double scrollAmount = absolute ? amount
										   : amount * Range;
				
			if (Vertical)
			{
				ScrollThumbCollider.Translate(0, scrollAmount);
			}
			else
			{
				ScrollThumbCollider.Translate(scrollAmount, 0);
			}
			if (clamp)
			{
				ClampThumbPosition();
			}
		}

		/// <summary>
		/// Scroll the thumb to the given value. If `clamp` is true, then the thumb will be clamped
		/// between the boundaries of the scroll bar. If `absolute` is false, then the given amount 
		/// is interpretted as a percentage.
		/// </summary>
		public virtual void ScrollTo(
			double value, bool absolute = false, bool clamp = true, bool overrideLock = false)
		{
			if (Locked && !overrideLock)
			{
				return;
			}
			double scrollAmount = absolute ? (value - ScrollValue * Range)
										   : (value - ScrollValue) * Range;

			if (Vertical)
			{
				ScrollThumbCollider.Translate(0, scrollAmount);
			}
			else
			{
				ScrollThumbCollider.Translate(scrollAmount, 0);
			}
			if (clamp)
			{
				ClampThumbPosition();
			}
		}

		/// <summary>
		/// Jump the thumb along the shaft in response to a click on the shaft.
		/// </summary>
		/// <param name="mpos"></param>
		protected virtual void JumpThumb(Point mpos)
		{
			bool positive = false;
			if (Vertical && mpos.Y > ScrollThumbCollider.Bottom
				|| !Vertical && mpos.X > ScrollThumbCollider.Right)
			{
				positive = true;
			}
			Scroll(positive ? JumpAmount : -JumpAmount);
		}

		/// <summary>
		/// Drag the thumb according to a change in the mouse position.
		/// </summary>
		/// <param name="mpos"></param>
		protected virtual void DragThumb(Point mpos)
		{
			if (Vertical)
			{
				Scroll(mpos.Y - prevDragPos, true);
			}
			else
			{
				Scroll(mpos.X - prevDragPos, true);
			}
			prevDragPos = mpos.Y;
		}

		/// <summary>
		/// Scroll the thumb in response to a change in the mouse scroll wheel value.
		/// 
		/// This is called if MouseInput.DeltaScrollWheelValue != 0, and the ScrollLockKey is
		/// null or pressed.
		/// </summary>
		protected virtual void MouseWheelScroll()
		{
            var multiplier = Vertical ? -1 : 1;
			Scroll(multiplier * MouseWheelScrollAmount / ScrollWheelDampener * MouseInput.DeltaScrollWheelValue);
		}

        protected virtual bool ValidKeyPressedState(Keys key)
        {
            return KeyboardInput.IsClicked(key) ||
                  (KeyboardInput.IsHeld(key, 400d) &&
                  (KeyboardInput.FramesSinceKeyPressed[KeyboardInput.KeyToInt(key)] - 400d) % 2 == 0);
        }

		public override void Update()
		{
			base.Update();

            prevScrollValue = ScrollValue;

			if (Locked)
				return;

			var mpos = MouseInput.MousePosition;

			ThumbCollidingWithMouse = ScrollThumbCollider.CollidingWith(MouseInput.MousePosition).Colliding;
			CollidingWithMouse = ScrollBarCollider.CollidingWith(mpos).Colliding;

			if (MouseInput.IsClicked(ButtonToPress))
			{
				if (ShaftCollidingWithMouse)
				{
					JumpThumb(mpos);
				}
				else if (ThumbCollidingWithMouse)
				{
					Dragging = true;
					prevDragPos = Vertical ? mpos.Y : mpos.X;
				}
			}
			else if (MouseInput.IsPressed(ButtonToPress))
			{
				if (Dragging)
				{
					DragThumb(mpos);
				}
			}
			else if (MouseInput.IsUnheld(ButtonToPress, 0))
			{
				Dragging = false;
				prevDragPos = 0;
			}

			if (!ScrollActivateKey.HasValue || KeyboardInput.IsPressed(ScrollActivateKey.Value))
			{
				if (MouseInput.DeltaScrollWheelValue != 0)
				{
					MouseWheelScroll();
				}
			}

            if (ScrollPlusKey.HasValue && ValidKeyPressedState(ScrollPlusKey.Value))
            {
                Scroll(KeyboardScrollAmount);
            }
            else if (ScrollMinusKey.HasValue && ValidKeyPressedState(ScrollMinusKey.Value))
            {
                Scroll(-KeyboardScrollAmount);
            }
		}
	}
}
