using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using System;
using System.Threading;

namespace QuanLySanPham.ViewModel
{
    public partial class WaitPopupViewModel : ObservableObject
    {
        private Popup _popup;
        private readonly Action _cancelAction;

        // Constructor accepts a cancellation callback
        public WaitPopupViewModel(Action cancelAction)
        {
            _cancelAction = cancelAction;
        }

        public void SetPopup(Popup popup)
        {
            _popup = popup;
        }

        [RelayCommand]
        private void Cancel()
        {
            _cancelAction?.Invoke(); // Trigger the cancellation
            _popup?.Close(null);     // Close the popup
        }
    }
}
