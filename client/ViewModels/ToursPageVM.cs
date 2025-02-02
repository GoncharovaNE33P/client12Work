using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using client.Models;
using ReactiveUI;
using Newtonsoft.Json;
using System.Reactive.Subjects;
using System.Reactive;

namespace client.ViewModels
{
    internal class ToursPageVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        List<Tour> _alltours = new();

        List<Tour> _tours;
        public List<Tour> ToursList { get => _tours; set => this.RaiseAndSetIfChanged(ref _tours, value); }

        string _search;
        public string Search { get => _search; set { _search = this.RaiseAndSetIfChanged(ref _search, value); filters(); } }

        List<Models.Type> _types = new();
        public List<Models.Type> TypesList { get => _types; set { _types = this.RaiseAndSetIfChanged(ref _types, value); filters(); } }

        Models.Type _selectType = null;
        public Models.Type SelectType
        {
            get => _selectType ?? _types.FirstOrDefault(); // Безопасный доступ
            set { this.RaiseAndSetIfChanged(ref _selectType, value); filters(); }
        }

        public ToursPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadDataCommand.Execute().Subscribe();
        }

        private async Task LoadDataAsync()
        {
            await getToursList(_httpClient);
            await getTypesList(_httpClient);
            if (TypesList != null && TypesList.Count > 0)
            {
                SelectType = TypesList[0];
            }

            TotalToursCount = _alltours.Count;
            FilteredToursCount = TotalToursCount;
        }

        async Task getToursList(HttpClient client)
        {
            try
            {
                HttpResponseMessage message = await client.GetAsync("/ToursList");
                message.EnsureSuccessStatusCode(); // Проверяем успешный статус ответа (200-299)

                string buf = await message.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(buf))
                {
                    Console.WriteLine("Ответ от сервера пустой.");
                    return;
                }

                _alltours = JsonConvert.DeserializeObject<List<Tour>>(buf) ?? new List<Tour>();
                ToursList = new List<Tour>(_alltours);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка туров: {ex.Message}");
            }
        }               
                
        async Task getTypesList(HttpClient client)
        {
            try
            {
                HttpResponseMessage message = await client.GetAsync("/TypesList");
                message.EnsureSuccessStatusCode(); // Проверяем успешный статус ответа (200-299)

                string buf = await message.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(buf))
                {
                    Console.WriteLine("Ответ от сервера пустой.");
                    return;
                }

                TypesList = JsonConvert.DeserializeObject<List<Models.Type>>(buf);
                TypesList = new List<Models.Type> { new Models.Type { Id = 0, Type1 = "Все типы" } }.Concat(TypesList).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка туров: {ex.Message}");
            }            
        }

        bool _checkActual = false;
        public bool CheckActual { get => _checkActual; set { _checkActual = value; filters(); } }

        private int _totalToursCount;
        public int TotalToursCount
        {
            get => _totalToursCount;
            set => this.RaiseAndSetIfChanged(ref _totalToursCount, value);
        }

        private int _filteredToursCount;
        public int FilteredToursCount
        {
            get => _filteredToursCount;
            set => this.RaiseAndSetIfChanged(ref _filteredToursCount, value);
        }

        private bool _noResults;
        public bool NoResults
        {
            get => _noResults;
            set => this.RaiseAndSetIfChanged(ref _noResults, value);
        }

        public void filters()
        {
            ToursList = new List<Tour>(_alltours);

            if (!string.IsNullOrEmpty(_search)) 
            {                
                ToursList = ToursList.Where(x => x.Name.ToLower().Contains(_search.ToLower()) ||
                x.Description.ToLower().Contains(_search.ToLower())).ToList();
            }

            if (SelectType != null && SelectType.Id != 0)
            {
                ToursList = ToursList.Where(x => x.ToursTypes.Any(y => y.TypeId == SelectType.Id)).ToList();
            }

            if (_checkActual)
            {
                ToursList = ToursList.Where(x => x.IsActual == 1).ToList();
            }

            FilteredToursCount = ToursList.Count;
            NoResults = FilteredToursCount == 0;
        }
    }
}
