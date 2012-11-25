using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ShellAndSettings.Events;
using ShellAndSettings.Settings.UI;

namespace ShellAndSettings.Shell
{
    public class ShellViewModel : Conductor<IScreen>, IShell, IHandle<SettingsCloseEvent>, INotifyPropertyChanged
    {
        const string SHOW_SETTINGS_STATE = "ShowSettings";
        const string HIDE_SETTINGS_STATE = "HideSettings";

        readonly IWindowManager _windowManager;
        readonly IEventAggregator _eventAggregator;

        public SettingsViewModel Settings { get; private set; }
        public string CurrentState { get; set; }

        [ImportingConstructor]
        public ShellViewModel(
            IWindowManager windowManager,
            SettingsViewModel settingsViewModel,
            IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            Settings = settingsViewModel;
            _eventAggregator = eventAggregator;

            Settings.Initialize();
        }

        public override string DisplayName
        {
            get { return "MarkPad style Shell and Settings"; }
            set { }
        }

        public override void CanClose(Action<bool> callback)
        {
            App.Current.Shutdown();
        }

        public void ShowSettings()
        {
            CurrentState = SHOW_SETTINGS_STATE;
        }

        public void ShowHelp()
        {
            MessageBox.Show("Not implemented yet");
        }

        public void Handle(SettingsCloseEvent message)
        {
            CurrentState = HIDE_SETTINGS_STATE;
        }
    }
}
