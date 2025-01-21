using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using client.ViewModels;

namespace client;

public partial class ToursPage : UserControl
{
    public ToursPage()
    {
        InitializeComponent();
        DataContext = new ToursPageVM(new HttpClient { BaseAddress = new Uri("http://localhost:5046/") });
    }
}