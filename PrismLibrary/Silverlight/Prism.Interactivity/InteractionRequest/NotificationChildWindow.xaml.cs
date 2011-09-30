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
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Practices.Prism.Interactivity.InteractionRequest
{
    ///<summary>
    /// The notification <see cref="ChildWindow"/> to use when displaying <see cref="Notification"/> messages.
    ///</summary>
    public partial class NotificationChildWindow : ChildWindow
    {
        ///<summary>
        /// The <see cref="DataTemplate"/> to apply when displaying <see cref="Notification"/> data.
        ///</summary>
        public static readonly DependencyProperty NotificationTemplateProperty =
            DependencyProperty.Register(
                "NotificationTemplate",
                typeof(DataTemplate),
                typeof(NotificationChildWindow),
                new PropertyMetadata(null));

        /// <summary>
        /// Creates a new instance of <see cref="NotificationChildWindow"/>
        /// </summary>
        public NotificationChildWindow()
        {
            InitializeComponent();
        }

        ///<summary>
        /// The <see cref="DataTemplate"/> to apply when displaying <see cref="Notification"/> data.
        ///</summary>
        public DataTemplate NotificationTemplate
        {
            get { return (DataTemplate)GetValue(NotificationTemplateProperty); }
            set { SetValue(NotificationTemplateProperty, value); }
        }
    }
}

