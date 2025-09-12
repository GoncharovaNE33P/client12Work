using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using client.Models;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.ComponentModel;

namespace client.ViewModels
{
    internal class EditCreateHotelVM : ViewModelBase
    {
        private readonly HttpClient _httpClient;

        public ICommand AddEditCommand { get; }

        string _nameBT;
        public string NameBT { get => _nameBT; set => this.RaiseAndSetIfChanged(ref _nameBT, value); }

        public void ToHotelsPage()
        {
            MainWindowViewModel.Instance.PageContent = new HotelsPage();
        }

        Hotel? _newHotel;
        public Hotel? NewHotel { get => _newHotel; set => this.RaiseAndSetIfChanged(ref _newHotel, value); }

        Country? _selectedCountry = null;

        public List<Country> CountryList { get; private set; }

        public Country? SelectedCountry
        {
            get
            {
                if (_selectedCountry == null)
                    return CountryList?.FirstOrDefault();
                else return _selectedCountry;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCountry, value);
            }
        }        

        string _countStars;
        public string CountStars { get => _countStars; set => this.RaiseAndSetIfChanged(ref _countStars, value); }

        bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public EditCreateHotelVM(HttpClient httpClient)
        {
            _nameBT = "Добавить отель";
            _httpClient = httpClient;
            NewHotel = new Hotel();
            AddEditCommand = new AsyncRelayCommand(async _ => await AddHotel());
            InitializeCountryListAsync();
        }

        public EditCreateHotelVM(int id,HttpClient httpClient)
        {
            _nameBT = "Сохранить изменения";
            _httpClient = httpClient;
            LoadDataAsync(id);
            AddEditCommand = new AsyncRelayCommand(async _ => await UpdateHotel());
        }

        private async Task LoadDataAsync(int id)
        {
            await InitializeCountryListAsync();
            await EditHotel(id);
        }

        async Task EditHotel(int id)
        {
            HttpResponseMessage messageMusicartist = await _httpClient.GetAsync($"/OneHotel/{id}");
            NewHotel = JsonConvert.DeserializeObject<Hotel>(await messageMusicartist.Content.ReadAsStringAsync());
            CountStars = NewHotel.CountOfStars.ToString();
            SelectedCountry = CountryList?.FirstOrDefault(p => p.CountryCode == NewHotel.CountryCodeNavigation.CountryCode);
        }

        private async Task InitializeCountryListAsync()
        {
            CountryList = await GetCountryList(_httpClient);
            this.RaisePropertyChanged(nameof(CountryList));
            SelectedCountry = CountryList?.FirstOrDefault();
        }

        async Task<List<Country>> GetCountryList(HttpClient client)
        {
            try
            {
                HttpResponseMessage message = await client.GetAsync("/CountryList");
                string buf = await message.Content.ReadAsStringAsync();
                var fetchedList = JsonConvert.DeserializeObject<List<Country>>(buf);
                fetchedList.OrderBy(x => x.CountryCode);
                var combinedList = new List<Country>
                {
                    new Country { CountryCode = "", NameCountry = "Выберите страну" }
                };

                if (fetchedList != null)
                    combinedList.AddRange(fetchedList.OrderBy(x => x.CountryCode));

                return combinedList;
            }
            catch (Exception ex)
            {
                await MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", ex.Message + "\n" + ex.StackTrace,
                    MsBox.Avalonia.Enums.ButtonEnum.Ok
                ).ShowAsync();

                return new List<Country>
                {
                    new Country { CountryCode = "", NameCountry = "Выберите страну" }
                };
            }
        }

        async Task AddHotel()
        {
            try
            {
                if (NewHotel.NameHotel != null && !string.IsNullOrEmpty(NewHotel.Description) && _countStars != "" && _selectedCountry.CountryCode != "")
                {
                    if (!IsAllDigits(_countStars))
                    {
                        string Messege = "Некорректные данные в поле с количеством звёзд. Необходимо ввести цифру от 0 до 5!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    else if (int.Parse(_countStars) < 0 || int.Parse(_countStars) > 5)
                    {
                        string Messege = "Некорректные данные в поле с количеством звёзд. Необходимо ввести цифру от 0 до 5!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    else
                    {
                        NewHotel.CountryCode = _selectedCountry.CountryCode;
                        NewHotel.CountOfStars = int.Parse(_countStars);
                        JsonContent contentCreate = JsonContent.Create(NewHotel);
                        HttpResponseMessage message = await _httpClient.PostAsync($"/HotelCreate", contentCreate);
                        string buf = await message.Content.ReadAsStringAsync();
                        NewHotel = JsonConvert.DeserializeObject<Hotel>(buf);
                        MainWindowViewModel.Instance.PageContent = new HotelsPage();
                        string Messege = "Отель добавлен!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о добавлении!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                }
                else
                {
                    if (NewHotel.NameHotel == null)
                    {
                        string Messege = "Наименование отеля не заполнено!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    if (string.IsNullOrEmpty(NewHotel.Description))
                    {
                        string Messege = "Описание отеля не заполнено!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }                    
                    if (_countStars == "")
                    {
                        string Messege = "Не указано количество звёзд у отеля!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    if (_selectedCountry.CountryCode == "")
                    {
                        string Messege = "Не выбрана страна, в которой находится отель!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Внимание", ex.Message + "\n" + ex.StackTrace, MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
            }
        }

        async Task UpdateHotel()
        {
            try
            {
                if (NewHotel.NameHotel != null && !string.IsNullOrEmpty(NewHotel.Description) && _countStars != "" && _selectedCountry.CountryCode != "")
                {
                    if (!IsAllDigits(_countStars))
                    {
                        string Messege = "Некорректные данные в поле с количеством звёзд. Необходимо ввести цифру от 0 до 5!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    else if (int.Parse(_countStars) < 0 || int.Parse(_countStars) > 5)
                    {
                        string Messege = "Некорректные данные в поле с количеством звёзд. Необходимо ввести цифру от 0 до 5!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    else
                    {
                        NewHotel.CountryCode = _selectedCountry.CountryCode;
                        NewHotel.CountryCodeNavigation = _selectedCountry;
                        NewHotel.CountOfStars = int.Parse(_countStars);
                        JsonContent contentUpdate = JsonContent.Create(NewHotel);
                        HttpResponseMessage updateMusicartist = await _httpClient.PutAsync("/HotelUpdate", contentUpdate);
                        MainWindowViewModel.Instance.PageContent = new HotelsPage();
                        string Messege = "Отель изменён!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением об изменениях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                }
                else
                {
                    if (NewHotel.NameHotel == null)
                    {
                        string Messege = "Наименование отеля не заполнено!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    if (string.IsNullOrEmpty(NewHotel.Description))
                    {
                        string Messege = "Описание отеля не заполнено!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                    if (_countStars == "")
                    {
                        string Messege = "Не указано количество звёзд у отеля!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }                    
                    if (_selectedCountry.CountryCode == "")
                    {
                        string Messege = "Не выбрана страна, в которой находится отель!";
                        ButtonResult result = await MessageBoxManager.GetMessageBoxStandard("Сообщение с уведомлением о незаполненных полях!", Messege, ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("Внимание", ex.Message + "\n" + ex.StackTrace, MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
            }
        }
    }
}
