using System.Windows;

namespace MathCore.WPF.Commands;

/// <summary>Команда для закрытия всего приложения</summary>
public class CloseApp : Command
{
    /// <summary>Код выхода из приложения</summary>
    public int? ExitCode { get; set; }

    /// <summary>Выполняет команду закрытия всего приложения</summary>
    public override void Execute(object? p)
    {
        var code = p switch
        {
            int c => c,
            string s when int.TryParse(s, out var v) => v,
            _ => ExitCode
        };

        if (code is not null)
            Application.Current.Shutdown(code.Value);
        else
            Application.Current.Shutdown();
    }
}