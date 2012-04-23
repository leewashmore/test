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
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    public partial class ViewAssetAllocation : ViewBaseUserControl
    {
        #region PropertyDeclaration

        private ViewModelAssetAllocation _dataContextAssetAllocation;
        public ViewModelAssetAllocation DataContextAssetAllocation
        {
            get
            {
                return _dataContextAssetAllocation;
            }
            set
            {
                _dataContextAssetAllocation = value;
            }
        }


        #endregion

        #region Constructor

        public ViewAssetAllocation(ViewModelAssetAllocation dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextAssetAllocation = dataContextSource;
        }

        #endregion

        #region EventUnsubscribe

        public override void Dispose()
        {
            this.DataContextAssetAllocation.Dispose();
            this.DataContextAssetAllocation = null;
            this.DataContext = null;
        }

        #endregion
    }
}
