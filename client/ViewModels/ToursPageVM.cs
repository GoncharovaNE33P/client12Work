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

namespace client.ViewModels
{
    internal class ToursPageVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;

        List<Tour> _alltours = new();

        List<Tour> _tours;
        public List<Tour> ToursList { get => _tours; set => this.RaiseAndSetIfChanged(ref _tours, value); }

        public ToursPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
            getToursList(_httpClient);
            getTypesList(_httpClient);
        }

        async Task getToursList(HttpClient client)
        {
            HttpResponseMessage message = await client.GetAsync("/ToursList");
            string buf = await message.Content.ReadAsStringAsync();
            _alltours = JsonConvert.DeserializeObject<List<Tour>>(buf);
            ToursList = new List<Tour>(_alltours);
        }       

        string _search;
        public string Search { get => _search; set { _search = this.RaiseAndSetIfChanged(ref _search, value);  filters(); } }

        List<Models.Type> _types;
        public List<Models.Type> TypesList { get => _types; set => this.RaiseAndSetIfChanged(ref _types, value); }

        async Task getTypesList(HttpClient client)
        {
            HttpResponseMessage message = await client.GetAsync("/TypesList");
            string buf = await message.Content.ReadAsStringAsync();
            TypesList = JsonConvert.DeserializeObject<List<Models.Type>>(buf);
            TypesList = new List<Models.Type> { new Models.Type { Id = 0, Type1 = "Все типы" } }.Concat(TypesList).ToList();
        }
        
        public void filters()
        {
            ToursList = new List<Tour>(_alltours);

            if (!string.IsNullOrEmpty(_search)) 
            {                
                ToursList = ToursList.Where(x => x.Name.ToLower().Contains(_search.ToLower()) ||
                x.Description.ToLower().Contains(_search.ToLower())).ToList();
            }
        }

    }
}
