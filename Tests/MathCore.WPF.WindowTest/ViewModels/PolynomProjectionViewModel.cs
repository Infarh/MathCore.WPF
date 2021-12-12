using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using XPlot.Plotly;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(PolynomProjectionViewModel))]
public class PolynomProjectionViewModel : TitledViewModel
{
    public PolynomProjectionViewModel()
    {
        Title = "Проверка";
        _Points = new()
        {
            new(0,0),
            new(1, 65535)
        };

        var x = Enumerable.Range(1, 10);
        var y = Enumerable.Range(1, 10);

        var chart = Chart.Plot(
            new Scatter
            {
                x = x,
                y = y,
                mode = "lines+markers"
            }
        );

        var chart1_layout = new Layout.Layout
        {
            title = "Open Price",
            xaxis = new Xaxis
            {
                title = "Date"
            },
            yaxis = new Yaxis
            {
                title = "Price (USD)"
            }
        };
        chart.WithLayout(chart1_layout);
        var html = chart.GetHtml();
        var web_browser = new WebBrowser{ Source = new("http://yandex.ru")};
        var window = new Window
        {
            Content = web_browser
        };
        window.Loaded += (_, _) => web_browser.NavigateToString(html);
        window.Show();
    }

    private ObservableCollection<Point> _Points;

    public IEnumerable<Point> Points => _Points;


}