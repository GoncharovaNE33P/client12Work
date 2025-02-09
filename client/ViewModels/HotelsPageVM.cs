using client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace client.ViewModels
{
    internal class HotelsPageVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }       

        private List<Hotel> _hotels = new();
        public List<Hotel> HotelsList
        {
            get => _hotels;
            set => this.RaiseAndSetIfChanged(ref _hotels, value);
        }

        private ObservableCollection<Hotel> _pagedHotels = new();
        private int _currentPage = 1;
        private int _itemsPerPage = 10;

        public ObservableCollection<Hotel> PagedHotels
        {
            get => _pagedHotels;
            set => this.RaiseAndSetIfChanged(ref _pagedHotels, value);
        }

        public ObservableCollection<int> PageSizes { get; } = new() { 5, 10, 20, 50 };

        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                this.RaiseAndSetIfChanged(ref _itemsPerPage, value);
                _currentPage = 1;
                UpdatePagedHotels();
            }
        }

        public string CurrentPageText => $"Страница {_currentPage} из {TotalPages}";

        public bool CanGoFirst => _currentPage > 1;
        public bool CanGoPrevious => _currentPage > 1;
        public bool CanGoNext => _currentPage < TotalPages;
        public bool CanGoLast => _currentPage < TotalPages;

        private int TotalPages => (int)Math.Ceiling((double)_hotels.Count / _itemsPerPage);

        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

        public void ToToursPage()
        {
            MainWindowViewModel.Instance.PageContent = new ToursPage();
        }

        public void ToEditCreateHotelPage()
        {
            MainWindowViewModel.Instance.PageContent = new EditCreateHotelPage();
        }

        public void ToEditCreateHotelPage(int id)
        {
            MainWindowViewModel.Instance.PageContent = new EditCreateHotelPage(id);
        }

        public HotelsPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;

            FirstPageCommand = ReactiveCommand.Create(() => { _currentPage = 1; UpdatePagedHotels(); });
            PreviousPageCommand = ReactiveCommand.Create(() => { if (_currentPage > 1) _currentPage--; UpdatePagedHotels(); });
            NextPageCommand = ReactiveCommand.Create(() => { if (_currentPage < TotalPages) _currentPage++; UpdatePagedHotels(); });
            LastPageCommand = ReactiveCommand.Create(() => { _currentPage = TotalPages; UpdatePagedHotels(); });

            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadDataCommand.Execute().Subscribe();
        }

        private void UpdatePagedHotels()
        {
            var pagedData = _hotels.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
            PagedHotels = new ObservableCollection<Hotel>(pagedData);

            this.RaisePropertyChanged(nameof(CurrentPageText));
            this.RaisePropertyChanged(nameof(CanGoFirst));
            this.RaisePropertyChanged(nameof(CanGoPrevious));
            this.RaisePropertyChanged(nameof(CanGoNext));
            this.RaisePropertyChanged(nameof(CanGoLast));
        }

        private async Task LoadDataAsync()
        {
            await GetHotelsList();
        }

        private async Task GetHotelsList()
        {
            try
            {
                HttpResponseMessage message = await _httpClient.GetAsync("/HotelsList");
                message.EnsureSuccessStatusCode();

                string buf = await message.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(buf))
                {
                    Debug.WriteLine("Ответ от сервера пустой.");
                    return;
                }

                HotelsList = JsonConvert.DeserializeObject<List<Hotel>>(buf);
                UpdatePagedHotels();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении списка отелей: {ex.Message}");
            }
        }        
    }
}
