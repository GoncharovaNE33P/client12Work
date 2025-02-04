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

namespace client.ViewModels
{
    internal class HotelsPageVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        List<Hotel> _hotels;
        public List<Hotel> HotelsList { get => _hotels; set => this.RaiseAndSetIfChanged(ref _hotels, value); }

        public HotelsPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            LoadDataCommand.Execute().Subscribe();
        }

        private async Task LoadDataAsync()
        {
            await getHotelsList(_httpClient);
        }

        async Task getHotelsList(HttpClient client)
        {
            try
            {
                HttpResponseMessage message = await client.GetAsync("/HotelsList");
                message.EnsureSuccessStatusCode(); 

                string buf = await message.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(buf))
                {
                    Console.WriteLine("Ответ от сервера пустой.");
                    return;
                }

                HotelsList = JsonConvert.DeserializeObject<List<Hotel>>(buf);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка туров: {ex.Message}");
            }
        }
    }
}
