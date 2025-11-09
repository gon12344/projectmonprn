using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        // Convenience read-only property for bindings
        public bool IsNotBusy => !IsBusy;

        // Notify dependent property IsNotBusy when IsBusy changes
        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }
}
