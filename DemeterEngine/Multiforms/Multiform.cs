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
 * A Multiform is a section of a game that defines its own behaviour and visual appearance. 
 * For example, a Menu and a Level would be separate Multiforms. They are called Multiforms 
 * because they are composed of multiple Forms. Each Form is a self-contained component that
 * serves to make their encompassing Multiform functional.
 */

#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using DemeterEngine.Extensions;

#endregion

namespace DemeterEngine.Multiforms
{
    public abstract class Multiform : ChronometricObject
    {

        private List<Form> anonymousForms = new List<Form>();

        private Dictionary<string, Form> namedForms = new Dictionary<string, Form>();

        /// <summary>
        /// The manager governing this Multiform.
        /// </summary>
        public MultiformManager Manager { get; private set; }

        public Action Updater { get; private set; }

        public Action Renderer { get; private set; }

        public Multiform()
            : base(true)
        {
            Manager = null;
        }

        /// <summary>
        /// Assign a manager to this multiform (only if one has not already been assigned).
        /// </summary>
        /// <param name="manager"></param>
        internal void SetManager(MultiformManager manager)
        {
            // We are only allowed to reset the manager if either the current manager is null or
            // the incoming manager is null.
            if (Manager != null && manager != null)
                throw new ArgumentException("A manager has already been assigned.");
            Manager = manager;
        }

        /// <summary>
        /// Set the current updater the multiform is using.
        /// </summary>
        /// <param name="updater"></param>
        protected void SetUpdater(Action updater)
        {
            Updater = updater;
        }

        /// <summary>
        /// Set the current renderer the multiform is using.
        /// </summary>
        /// <param name="renderer"></param>
        protected void SetRenderer(Action renderer)
        {
            Renderer = renderer;
        }

        /// <summary>
        /// Register a form.
        /// </summary>
        /// <param name="form"></param>
        protected void RegisterForm(Form form)
        {
            anonymousForms.Add(form);
            form.SetParent(this);
        }

        /// <summary>
        /// Register a form with a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="form"></param>
        /// <param name="serviceLocator"></param>
        protected void RegisterForm(string name, Form form)
        {
            namedForms[name] = form;
            form.SetParent(this);
        }

        /// <summary>
        /// Return a form by it's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Form GetForm(string name)
        {
            return namedForms[name];
        }

        /// <summary>
        /// Return a form by it's name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetForm<T>(string name) where T : Form
        {
            return (T)namedForms[name];
        }

        /// <summary>
        /// Clear all forms.
        /// </summary>
        protected void ClearForms()
        {
            namedForms.Clear();
            anonymousForms.Clear();
        }

        /// <summary>
        /// Remove a form with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveForm(string name)
        {
            namedForms.Remove(name);
        }

        /// <summary>
        /// Attempt to remove a form object.
        /// </summary>
        /// <param name="form"></param>
        public void RemoveForm(Form form)
        {
            var removed = anonymousForms.Remove(form);
            if (!removed)
            {
                var item = namedForms.First(i => i.Value == form);
                namedForms.Remove(item.Key);
            }
        }

        /// <summary>
        /// Return a list of all the forms in this multiform.
        /// </summary>
        /// <returns></returns>
        public Form[] AllForms()
        {
            return anonymousForms.ToArray().Concat(namedForms.Values.ToArray());
        }

        /// <summary>
        /// Update the form with the given name.
        /// </summary>
        /// <param name="name"></param>
        protected void UpdateForm(string name)
        {
            namedForms[name].Update();
        }

        /// <summary>
        /// Update the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        protected void UpdateForms(params string[] names)
        {
            foreach (var name in names)
                namedForms[name].Update();
        }

        /// <summary>
        /// Update all the forms attached to this multiform.
        /// </summary>
        /// <param name="serviceLocator"></param>
        protected void UpdateForms()
        {
            foreach (var form in anonymousForms)
                form.Update();
            foreach (var form in namedForms)
                form.Value.Update();
        }

		protected void UpdateFormsExcept(params string[] names)
		{
			foreach (var form in anonymousForms)
				form.Update();
			foreach (var form in namedForms)
            {
                if (!names.Contains(form.Key))
                    form.Value.Update();
            }
		}

        /// <summary>
        /// Render the form with the given name.
        /// </summary>
        /// <param name="name"></param>
        protected void RenderForm(string name)
        {
            namedForms[name].Render();
        }

        /// <summary>
        /// Render the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        protected void RenderForms(params string[] names)
        {
            foreach (var name in names)
                namedForms[name].Render();
        }

        protected void RenderFormsExcept(params string[] names)
        {
            foreach (var form in anonymousForms)
                form.Render();
            foreach (var form in namedForms)
            {
                if (!names.Contains(form.Key))
                    form.Value.Render();
            }
        }

        /// <summary>
        /// Render all the forms attached to this multiform.
        /// </summary>
        /// <param name="serviceLocator"></param>
        protected void RenderForms()
        {
            foreach (var form in anonymousForms)
                form.Render();
            foreach (var form in namedForms)
                form.Value.Render();
        }

        /// <summary>
        /// Construct the Multiform. This gets called every time the MultiformManager switches
        /// to using this Multiform. TransitionArguments from the previous Multiform are passed in.
        /// </summary>
        /// <param name="args"></param>
        public abstract void Construct(MultiformTransmissionData args);
    }
}
