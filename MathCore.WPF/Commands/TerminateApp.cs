namespace MathCore.WPF.Commands;

/// <summary>Команда для завершения всего приложения принудительно</summary>
public class TerminateApp : CloseApp
{
    /// <summary>Выполняет команду завершения всего приложения</summary>
    public override void Execute(object? p)
    {
        var code = p switch
        {
            int c => c,
            string s when int.TryParse(s, out var v) => v,
            _ => ExitCode
        };
        if (code is not null)
            Environment.Exit(code.Value);
        else
            Environment.Exit(0);
    }
}