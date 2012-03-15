﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.LoginModule.ViewModel;

namespace GreenField.LoginModule.Views
{
    [Export]
    public partial class ViewPasswordResetForm : UserControl
    {
        public ViewPasswordResetForm()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelPasswordResetForm DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
