namespace MathCore.WPF.Operations;

/// <summary>Делегат выполняемой операции</summary>
/// <typeparam name="T">Тип значения, передаваемого в операцию командой запуска</typeparam>
/// <typeparam name="TResult">Тип результата выполняемой операции</typeparam>
/// <param name="parameter">Параметр, передаваемый в операцию командой запуска</param>
/// <param name="progress">Объект извещения о изменении прогресса</param>
/// <param name="Cancel">Признак отмены</param>
/// <returns>Задача выполнения операции</returns>
public delegate Task<TResult> OperationFunc<in T, TResult>(T? parameter, IProgress<double> progress, CancellationToken Cancel);