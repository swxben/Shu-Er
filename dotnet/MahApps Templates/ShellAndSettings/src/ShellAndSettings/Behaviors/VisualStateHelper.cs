using System;
using System.Windows;
using System.Windows.Controls;

namespace ShellAndSettings.Behaviors
{
    public class VisualStateHelper : DependencyObject
    {
        static readonly DependencyProperty VisualStateNameProperty = DependencyProperty.RegisterAttached("VisualStateName", typeof(string), typeof(VisualStateHelper), new PropertyMetadata(OnVisualStateNameChanged));

        public static string GetVisualStateName(DependencyObject target)
        {
            return (string)target.GetValue(VisualStateNameProperty);
        }

        public static void SetVisualStateName(DependencyObject target, string visualStateName)
        {
            target.SetValue(VisualStateNameProperty, visualStateName);
        }

        static void OnVisualStateNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is Grid)) throw new InvalidOperationException("This attached property only supports types derived from Control");
            VisualStateManager.GoToElementState(sender as Grid, (string)args.NewValue, true);
        }
    }
}
