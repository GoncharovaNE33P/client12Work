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

        public HotelsPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);

            LoadDataCommand.Execute().Subscribe();
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении списка отелей: {ex.Message}");
            }
        }        
    }
}
