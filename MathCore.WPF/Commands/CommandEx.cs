namespace MathCore.WPF.Commands;

public static class CommandEx
{
    public static TCommand CatchException<TCommand>(this TCommand Command, Action<Exception> Handler) 
        where TCommand : Command
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e) => Handler(e.Argument);

        Command.Error += Catch;
        return Command;
    }

    public static TCommand CatchException<TCommand, TException>(this TCommand Command, Action<TException> Handler) 
        where TCommand : Command
        where TException : Exception
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e)
        {
            if (e.Argument is TException error)
                Handler(error);
        }

        Command.Error += Catch;
        return Command;
    }

    public static TCommand CatchException<TCommand>(this TCommand Command, Func<Exception, bool> Handler) 
        where TCommand : Command
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e)
        {
            if (Handler(e.Argument))
                e.IsHandled = true;
        }

        Command.Error += Catch;
        return Command;
    }

    public static TCommand CatchException<TCommand, TException>(this TCommand Command, Func<TException, bool> Handler) 
        where TCommand : Command
        where TException : Exception
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e)
        {
            if (e.Argument is TException error && Handler(error))
                e.IsHandled = true;
        }

        Command.Error += Catch;
        return Command;
    }

    public static TCommand IgnoreExceptions<TCommand>(this TCommand Command)
        where TCommand : Command
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e) => e.IsHandled = true;

        Command.Error += Catch;
        return Command;
    }

    public static TCommand CatchCancellation<TCommand>(this TCommand Command)
        where TCommand : Command
    {
        void Catch(object sender, ExceptionEventHandlerArgs<Exception> e)
        {
            if (e.Argument is OperationCanceledException)
                e.IsHandled = true;
        }

        Command.Error += Catch;
        return Command;
    }
}
