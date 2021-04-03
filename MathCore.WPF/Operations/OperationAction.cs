using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.WPF.Operations
{
    /// <summary>Делегат выполняемой операции</summary>
    /// <param name="parameter">Параметр, передаваемый в операцию командой запуска</param>
    /// <param name="progress">Объект извещения о изменении прогресса</param>
    /// <param name="Cancel">Признак отмены</param>
    /// <returns>Задача выполнения операции</returns>
    public delegate Task OperationAction(object? parameter, IProgress<double> progress, CancellationToken Cancel);

    /// <summary>Делегат выполняемой операции</summary>
    /// <typeparam name="T">Тип значения, передаваемого в операцию командой запуска</typeparam>
    /// <param name="parameter">Параметр, передаваемый в операцию командой запуска</param>
    /// <param name="progress">Объект извещения о изменении прогресса</param>
    /// <param name="Cancel">Признак отмены</param>
    /// <returns>Задача выполнения операции</returns>
    public delegate Task OperationAction<in T>(T? parameter, IProgress<double> progress, CancellationToken Cancel);
}