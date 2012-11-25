using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ShellAndSettings
{
    public partial class App : Application
    {
        readonly AppBootstrapper _bootstrapper;

        public App()
        {
            InitializeComponent();

            _bootstrapper = new AppBootstrapper();
        }
    }
}
