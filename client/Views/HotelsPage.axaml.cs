using System.Net.Http;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using client.ViewModels;

namespace client;

public partial class HotelsPage : UserControl
{
    public HotelsPage()
    {
        InitializeComponent();
        DataContext = new ToursPageVM(new HttpClient { BaseAddress = new Uri("http://localhost:5046/") });
    }
}