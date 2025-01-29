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

        List<Tour> _tours;
        public List<Tour> ToursList { get => _tours; set => this.RaiseAndSetIfChanged(ref _tours, value); }

        public ToursPageVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
            getToursList(_httpClient);
        }

        async Task getToursList(HttpClient client)
        {
            HttpResponseMessage message = await client.GetAsync("/ToursList");
            string buf = await message.Content.ReadAsStringAsync();
            ToursList = JsonConvert.DeserializeObject<List<Tour>>(buf);
        }

        string _search;
        public string Search { get => _search; set { _search = this.RaiseAndSetIfChanged(ref _search, value);  filters(); } }

        public void filters()
        {
            if (!string.IsNullOrEmpty(_search)) 
            {                
                ToursList = ToursList.Where(x => x.Name.ToLower().Contains(_search.ToLower()) ||
                x.Description.ToLower().Contains(_search.ToLower())).ToList();
            }
            else getToursList(_httpClient);
        }

    }
}
