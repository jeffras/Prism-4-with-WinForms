//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Prism.Interactivity.Tests
{
    [TestClass]
    public class ApplicationBarButtonCommandFixture
    {
        [TestMethod]
        public void CommandBehaviorMonitorsCanExecuteToSetEnabledWhenCanExecuteIsTrue()
        {
            var page = new PhoneApplicationPage();
            var bar = new ApplicationBar();
            var button = new ApplicationBarIconButton(new Uri("/foo.png", UriKind.Relative));
            button.Text = "Foo";
            bar.Buttons.Add(button);
            page.ApplicationBar = bar;
            var command = new ApplicationBarButtonCommand();
            command.ButtonText = "Foo";
            var bindingSource = new MyCommandHolder();
            var binding = new Binding("Command") { Source = bindingSource, Mode = BindingMode.OneWay };
            command.CommandBinding = binding;
            Interaction.GetBehaviors(page).Add(command);

            bindingSource.CanExecute = true;

            Assert.IsTrue(button.IsEnabled);
        }

        [TestMethod]
        public void CommandBehaviorMonitorsCanExecuteChangedToUpdateEnabledWhenCanExecuteChangedIsRaised()
        {
            var page = new PhoneApplicationPage();
            var bar = new ApplicationBar();
            var button = new ApplicationBarIconButton(new Uri("/foo.png", UriKind.Relative));
            button.Text = "Foo";
            bar.Buttons.Add(button);
            page.ApplicationBar = bar;
            var command = new ApplicationBarButtonCommand();
            command.ButtonText = "Foo";
            var bindingSource = new MyCommandHolder();
            var binding = new Binding("Command") { Source = bindingSource, Mode = BindingMode.OneWay };
            command.CommandBinding = binding;
            Interaction.GetBehaviors(page).Add(command);

            bindingSource.CanExecute = true;

            var initialState = button.IsEnabled;

            bindingSource.CanExecute = false;

            var finalState = button.IsEnabled;

            Assert.IsTrue(initialState);
            Assert.IsFalse(finalState);
        }

        public class MyCommandHolder
        {
            private bool canExecute;

            public MyCommandHolder()
            {
                this.Command = new MockCommand()
                                   {
                                       CanExecuteCallback = () => this.canExecute
                                   };
            }

            public bool CanExecute
            {
                get { return this.canExecute; }
                set
                {
                    this.canExecute = value;
                    ((MockCommand)this.Command).RaiseCanExecuteChanged();
                }
            }

            public ICommand Command { get; set; }
        }

        private class MockCommand : ICommand
        {
            public Func<bool> CanExecuteCallback = () => { return true; };

            public bool CanExecute(object parameter)
            {
                return this.CanExecuteCallback();
            }

            public void Execute(object parameter)
            {

            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                var handlers = this.CanExecuteChanged;
                if (handlers != null)
                {
                    handlers(this, EventArgs.Empty);
                }

            }
        }
    }
}