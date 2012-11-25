using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace ShellAndSettings.Shell
{
    public partial class ShellView : Window
    {
        bool _ignoreNextMouseMove;

        public ShellView()
        {
            InitializeComponent();
        }

        void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            //if (DocumentIsOpen && !header.IsMouseOver) return;

            if (WindowState == WindowState.Maximized && e.ClickCount != 2) return;

            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                _ignoreNextMouseMove = true;
                return;
            }

            DragMove();
        }

        void MouseMoveWindow(object sender, MouseEventArgs e)
        {
            if (_ignoreNextMouseMove)
            {
                _ignoreNextMouseMove = false;
                return;
            }

            if (WindowState != WindowState.Maximized) return;

            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!header.IsMouseOver) return;

            // Calculate correct left coordinate for multi-screen system
            var mouseX = PointToScreen(Mouse.GetPosition(this)).X;
            var width = RestoreBounds.Width;
            var left = mouseX - width / 2;
            if (left < 0) left = 0;

            // Align left edge to fit the screen
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (left + width > virtualScreenWidth) left = virtualScreenWidth - width;

            Top = 0;
            Left = left;

            WindowState = WindowState.Normal;

            DragMove();
        }

        void ToggleMaximized()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        void ShellViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //if (DocumentIsOpen && !header.IsMouseOver) return;
            ToggleMaximized();
        }

        void ButtonMinimiseOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void ButtonMaxRestoreOnClick(object sender, RoutedEventArgs e)
        {
            ToggleMaximized();
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            RefreshMaximiseIconState();
            base.OnStateChanged(e);
        }


        void RefreshMaximiseIconState()
        {
            if (WindowState == WindowState.Normal)
            {
                maxRestore.Content = "1";
                maxRestore.SetResourceReference(ToolTipProperty, "WindowCommandsMaximiseToolTip");
            }
            else
            {
                maxRestore.Content = "2";
                maxRestore.SetResourceReference(ToolTipProperty, "WindowCommandsRestoreToolTip");
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }



    }
}
