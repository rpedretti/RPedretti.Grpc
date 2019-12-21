using System;

using RPedretti.Grpc.Uwp.Client.ViewModels;

using Windows.UI.Xaml.Controls;

namespace RPedretti.Grpc.Uwp.Client.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
