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

        private List<Hotel> _pagehotels = new();
        public List<Hotel> HotelsList
        {
            get => _pagehotels;
            set => this.RaiseAndSetIfChanged(ref _pagehotels, value);
        }      

        public List<int> PageSizes { get; } = new() { 5, 10, 20, 50 };

        private int _itemsPage = 10;
        private int _currentPage = 1;
        private int _totalHotels;
        private int _totalPages;
        public string CurrentPageText => $"Страница {_currentPage} из {TotalPages}";

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value < 1) value = 1;
                if (value > TotalPages) value = TotalPages;
                this.RaiseAndSetIfChanged(ref _currentPage, value);
                UpdatePagedHotels();
                this.RaisePropertyChanged(nameof(PageInfo));
            }
        }

        public string PageInfo => $"Страница {_currentPage} из {TotalPages}";
        public int TotalHotels
        {
            get => _totalHotels;
            private set => this.RaiseAndSetIfChanged(ref _totalHotels, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => this.RaiseAndSetIfChanged(ref _totalPages, value);
        }

        public int ItemsPage
        {
            get => _itemsPage;
            set
            {
                if (value < 1) value = 1;
                this.RaiseAndSetIfChanged(ref _itemsPage, value);
                UpdatePagination();
            }
        }

        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
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
            
            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadDataCommand.Execute().Subscribe();

            UpdatePagination();

            NextPageCommand = ReactiveCommand.Create(() => CurrentPage++);
            PrevPageCommand = ReactiveCommand.Create(() => CurrentPage--);
            FirstPageCommand = ReactiveCommand.Create(() => CurrentPage = 1);
            LastPageCommand = ReactiveCommand.Create(() => CurrentPage = TotalPages);
        }

        private async Task LoadDataAsync()
        {
            await GetHotelsList();
            UpdatePagination();
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

                _hotels = JsonConvert.DeserializeObject<List<Hotel>>(buf);                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении списка отелей: {ex.Message}");
            }
        }

        private void UpdatePagination()
        {
            TotalHotels = _hotels.Count;
            TotalPages = (int)Math.Ceiling((double)TotalHotels / ItemsPage);
            CurrentPage = 1; // Переключаем на первую страницу при изменении количества элементов
            UpdatePagedHotels();
        }

        private void UpdatePagedHotels()
        {
            if (TotalHotels == 0)
            {
                HotelsList = new List<Hotel>();
                return;
            }

            HotelsList = new List<Hotel>(
                _hotels.Skip((CurrentPage - 1) * ItemsPage).Take(ItemsPage)
            );
        }
    }
}
