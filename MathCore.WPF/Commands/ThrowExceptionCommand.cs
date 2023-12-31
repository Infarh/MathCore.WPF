namespace MathCore.WPF.Commands;

public class ThrowExceptionCommand : Command
{
    public override void Execute(object? parameter)
    {
        switch (parameter)
        {
            default: throw new ApplicationException();
            case Exception e: throw e;
            case Type type when type.IsSubclassOf(typeof(Exception)) && Activator.CreateInstance(type) is Exception exception:
                throw exception;
            case string message:
                throw new ApplicationException(message);
        }
    }
}
