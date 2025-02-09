using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using client.ViewModels;
using System.Net.Http;
using System;

namespace client;

public partial class EditCreateHotelPage : UserControl
{
    public EditCreateHotelPage()
    {
        InitializeComponent();
        DataContext = new EditCreateHotelVM(new HttpClient { BaseAddress = new Uri("http://localhost:5046/") });
    }

    public EditCreateHotelPage(int id)
    {
        InitializeComponent();
        DataContext = new EditCreateHotelVM(id,new HttpClient { BaseAddress = new Uri("http://localhost:5046/") });
    }
}