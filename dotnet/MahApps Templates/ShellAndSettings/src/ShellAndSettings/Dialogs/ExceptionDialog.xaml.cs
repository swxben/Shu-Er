using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ShellAndSettings.Dialogs
{
    public partial class ExceptionDialog : Window
    {
        public Exception Exception { get; set; }
        string _details;
        string _message;

        void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.MiddleButton == MouseButtonState.Pressed) return;
            DragMove();
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                messageBox.Text = value;
            }
        }

        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                detailsBox.Text = value;
            }
        }

        public ExceptionDialog()
        {
            InitializeComponent();
        }

        void TryClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            SetData(DataFormats.Text, this.Details);
        }

        void SetData(string format, object data)
        {
            if (!data.GetType().IsSerializable)
            {
                throw new NotSupportedException("The object must be serializable");
            }

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetData(format, data);
                    return;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e, "ERROR");
                }
                Thread.Sleep(100);
            }
        }
    }
}
