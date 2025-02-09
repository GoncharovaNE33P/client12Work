using Avalonia.Controls;
using ReactiveUI;

namespace client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance;

        public MainWindowViewModel()
        {
            Instance = this;
        }

        UserControl pageContent = new ToursPage();

        public UserControl PageContent { get => pageContent; set => this.RaiseAndSetIfChanged(ref pageContent, value); }
    }
}
