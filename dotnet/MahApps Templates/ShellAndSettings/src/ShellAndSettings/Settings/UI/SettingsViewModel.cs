using Caliburn.Micro;
using ShellAndSettings.Events;

namespace ShellAndSettings.Settings.UI
{
    public class SettingsViewModel : Screen
    {
        readonly IEventAggregator _eventAggregator;

        public SettingsViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
        }

        public void Accept()
        {
        }

        public void HideSettings()
        {
            _eventAggregator.Publish(new SettingsChangedEvent());
            _eventAggregator.Publish(new SettingsCloseEvent());
        }
    }
}
