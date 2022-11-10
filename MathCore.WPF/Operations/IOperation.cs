using System.Windows.Input;

namespace MathCore.WPF.Operations;

public interface IOperation : ICommand
{
    /// <summary>Запуск операции</summary>
    ICommand Start { get; }

    /// <summary>Прогресс операции</summary>
    double Progress { get; }

    /// <summary>Отмена операции</summary>
    ICommand Cancel { get; }

    /// <summary>Ошибка</summary>
    Exception? Error { get; set; }

    /// <summary>Операция завершена с ошибкой</summary>
    bool HasError { get; }

    /// <summary>Сообщение об ошибке в последней операции</summary>
    string? ErrorMessage { get; }

    /// <summary>Операция в процессе выполнения</summary>
    bool InProgress { get; }

    /// <summary>Прошедшее в процессе операции время от момента запуска</summary>
    TimeSpan ElapsedTime { get; }

    /// <summary>Оставшееся время до достижения прогресса значения 1</summary>
    TimeSpan RemainingTime { get; }

    /// <summary>Скорость выполнения прогресса - число процентов в секунду</summary>
    double PercentPerSecond { get; }
}