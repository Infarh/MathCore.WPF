

// ReSharper disable ConvertToLocalFunction

// ReSharper disable ParameterHidesMember

namespace MathCore.WPF.Commands;

public abstract partial class Command
{
    /// <summary>Построитель команды</summary>
    public readonly ref struct Builder
    {
        /// <summary>Название команды</summary>
        public string? Name { get; init; }

        /// <summary>Описание команды</summary>
        public string? Description { get; init; }

        /// <summary>Выполняемый командой делегат</summary>
        public Action<object?> Execute { get; init; }

        /// <summary>Делегат проверки возможности выполнения команды</summary>
        public Func<object?, bool>? CanExecute { get; init; }

        public Builder(Action Execute, Func<bool>? CanExecute = null, string? Name = null, string? Description = null)
            : this(_ => Execute(), CanExecute is null ? null : _ => CanExecute(), Name, Description) { }

        public Builder(Action<object?> Execute, Func<object?, bool>? CanExecute = null, string? Name = null, string? Description = null)
        {
            this.Execute = Execute;
            this.CanExecute = CanExecute;
            this.Name = Name;
            this.Description = Description;
        }

        /// <summary>Добавить выполняемый делегат в конец</summary>
        /// <param name="Execute">Выполняемый командой делегат</param>
        /// <returns>Построитель команды</returns>
        public Builder Invoke(Action<object?> Execute) => this with { Execute = this.Execute + Execute };

        /// <summary>Добавить выполняемый делегат в начало</summary>
        /// <param name="Execute">Выполняемый командой делегат</param>
        /// <returns>Построитель команды</returns>
        public Builder InvokeBefore(Action<object?> Execute) => this with { Execute = Execute + this.Execute };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Использовать локальную функцию", Justification = "Структура не может быть захвачена")]
        public BuilderT<T> Invoke<T>(Action<T?> Execute)
        {
            var this_execute = Execute;
            Action<T?> action = p =>
            {
                this_execute(p);
                Execute(p);
            };

            return CanExecute is { } this_can_execute 
                ? new BuilderT<T>(action).When(p => this_can_execute(p)) 
                : new (action);
        }

        public Builder When(Func<object?, bool> CanExecute) => this with { CanExecute = this.CanExecute + CanExecute };
        public Builder WithName(string Name) => this with { Name = Name };
        public Builder WithDescription(string Description) => this with { Description = Description };

        public Command Build() => new LambdaCommand(Execute, CanExecute);

        public static implicit operator Command(Builder builder) => builder.Build();
    }

    public readonly ref struct BuilderAsync
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public Func<object?, Task> Execute { get; init; }
        public Func<object?, bool>? CanExecute { get; init; }

        public BuilderAsync(Func<Task> Execute, Func<bool>? CanExecute = null, string? Name = null, string? Description = null)
            : this(_ => Execute(), CanExecute is null ? null : _ => CanExecute(), Name, Description) { }

        public BuilderAsync(Func<object?, Task> Execute, Func<object?, bool>? CanExecute = null, string? Name = null, string? Description = null)
        {
            this.Execute = Execute;
            this.CanExecute = CanExecute;
            this.Name = Name;
            this.Description = Description;
        }

        //public Builder Invoke(Func<object?, Task> Execute) => this with { Execute = this.Execute + Execute };
        //public Builder InvokeBefore(Func<object?, Task> Execute) => this with { Execute = Execute + this.Execute };

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Использовать локальную функцию", Justification = "Структура не может быть захвачена")]
        //public BuilderT<T> Invoke<T>(Action<T?> Execute)
        //{
        //    var this_execute = Execute;
        //    Action<T?> action = p =>
        //    {
        //        this_execute(p);
        //        Execute(p);
        //    };

        //    return CanExecute is { } this_can_execute
        //        ? new BuilderT<T>(action).When(p => this_can_execute(p))
        //        : new(action);
        //}

        //public Builder When(Func<object?, bool> CanExecute) => this with { CanExecute = this.CanExecute + CanExecute };
        //public Builder WithName(string Name) => this with { Name = Name };
        //public Builder WithDescription(string Description) => this with { Description = Description };

        public Command Build() => new LambdaCommandAsync(Execute, CanExecute);

        public static implicit operator Command(BuilderAsync builder) => builder.Build();
    }

    public readonly ref struct BuilderT<T>
    {
        public Action<T?> Execute { get; init; }
        public Func<T?, bool>? CanExecute { get; init; }

        public BuilderT(Action<T?> Execute, Func<T?, bool>? CanExecute = null, string? Name = null, string? Description = null)
        {
            this.Execute = Execute;
            this.CanExecute = CanExecute;
        }

        public BuilderT<T> Invoke(Action<T?> Execute) => this with { Execute = this.Execute + Execute };
        public BuilderT<T> InvokeBefore(Action<T?> Execute) => this with { Execute = Execute + this.Execute };
        public BuilderT<T> When(Func<T?, bool> CanExecute) => this with { CanExecute = this.CanExecute + CanExecute };

        public Command Build() => new LambdaCommand<T>(Execute, CanExecute);

        public static implicit operator Command(BuilderT<T> builder) => builder.Build();
    }

    public static Builder New() => new();
    public static BuilderT<T> New<T>() => new();

    public static Builder Invoke(Action action) => new(action);
    public static Builder Invoke(Action<object?> action) => new(action);
    public static BuilderT<T> Invoke<T>(Action<T?> action) => new(action);
}