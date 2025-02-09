using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace client.ViewModels
{
    internal class EditCreateHotelVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;

        public EditCreateHotelVM(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public EditCreateHotelVM(int id,HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
