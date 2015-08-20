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
		public MouseButtons Button { get; private set; }

		/// <summary>
		/// The keyboard key the user has to be holding down in order to scroll with the
		/// mouse wheel. This is so that you can implement horizontal scroll bars which
		/// require the user to press "shift" to use.
		/// </summary>
		public Keys? ScrollLockKey { get; private set; }

		/// <summary>
		/// The amount by which the scroll thumb jumps when the user clicks on the scroll
		/// bar shaft.
		/// </summary>
		public double JumpAmount { get; private set; }

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
			int scrollBarWidth, bool vertical, float initialScrollValue = 0, 
			MouseButtons respondTo = MouseButtons.Left, Keys? scrollLock = null,
			double jumpAmount = 0.25, bool keepsTime = true)
			: base(keepsTime)
		{
			Vertical = vertical;

			// Let Sl = scrollBarLength, St = scrollThumbLength, and T = spanLength.
			// Then St/Sl = Sl/T, so St = Sl^2/T.
			var scrollThumbLength = scrollBarLength * scrollBarLength / spanLength;

			// Clamp it so that the scroll thumb can only be as long as the scroll bar.
			scrollThumbLength = Math.Min(scrollThumbLength, scrollBarLength);

			if (vertical)
			{
				ScrollBarCollider   = new RectCollider(topLeft, scrollBarWidth, scrollBarLength);
				ScrollThumbCollider = new RectCollider(topLeft, scrollBarWidth, scrollThumbLength);
				ScrollThumbCollider.Translate(0, initialScrollValue * Range);
			}
			else
			{
				ScrollBarCollider   = new RectCollider(topLeft, scrollBarLength, scrollBarWidth);
				ScrollThumbCollider = new RectCollider(topLeft, scrollThumbLength, scrollBarWidth);
				ScrollThumbCollider.Translate(initialScrollValue * Range, 0);
			}

			ClampThumbPosition();

			Button = respondTo;
			if (!scrollLock.HasValue)
			{
				ScrollLockKey = vertical ? null : (Nullable<Keys>)Keys.LeftShift;
			}
			else
			{
				ScrollLockKey = scrollLock;
			}

			JumpAmount = jumpAmount;

			Locked = false;
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
		public void Scroll(
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
		public void ScrollTo(
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

		protected virtual void DragThumb(Point mpos)
		{
			if (Vertical)
			{
				Scroll(mpos.Y - prevDragPos);
			}
			else
			{
				Scroll(mpos.X - prevDragPos);
			}
		}

		public override void Update()
		{
			base.Update();

			if (Locked)
				return;

			var mpos = MouseInput.MousePosition;

			ThumbCollidingWithMouse = ScrollThumbCollider.CollidingWith(MouseInput.MousePosition).Colliding;
			CollidingWithMouse = ScrollBarCollider.CollidingWith(mpos).Colliding;

			if (MouseInput.IsClicked(Button))
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
			else if (MouseInput.IsPressed(Button))
			{
				if (Dragging)
				{
					DragThumb(mpos);
					prevDragPos = mpos.Y;
				}
			}
			else if (MouseInput.IsUnheld(Button, 0))
			{
				Dragging = false;
				prevDragPos = 0;
			}
		}
	}
}
