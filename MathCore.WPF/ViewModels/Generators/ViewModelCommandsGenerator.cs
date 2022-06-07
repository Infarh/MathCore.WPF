using System.Collections;
using System.Collections.Immutable;
using System.Text;

using MathCore.WPF.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MathCore.WPF.ViewModels.Generators;

[Generator]
public class ViewModelCommandsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new ViewModelCommandsSyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not ViewModelCommandsSyntaxReceiver receiver)
            return;

        foreach (var (@class, method, name) in receiver)
        {
            var file = $"{@class.Name}.cs";

        }
    }
}

internal class ViewModelCommandsSyntaxReceiver : ISyntaxContextReceiver, IEnumerable<ViewModelCommandInfo>
{
    private readonly List<ViewModelCommandInfo> _Commands = new();

    public List<string> Log { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        try
        {
            if (context.Node is MethodDeclarationSyntax method_syntax)
            {
                var command_method = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
                var container_class = command_method.ContainingType;

                foreach (var attribute in command_method.GetAttributes())
                    foreach (var constructor_argument in attribute.ConstructorArguments)
                        if (constructor_argument.Value is INamedTypeSymbol named_argument)
                            foreach (var member in named_argument.GetMembers())
                                if (member is IPropertySymbol property)
                                {

                                }

                var command_attribute = command_method
                   .GetAttributes()
                   .FirstOrDefault(att => att.AttributeClass.Name == typeof(CommandAttribute).FullName);
                if (command_attribute is not null)
                {
                    foreach (var assembly in command_method.ContainingModule.ReferencedAssemblies)
                    {

                    }

                    var command_name = (INamedTypeSymbol?)command_attribute.ConstructorArguments[0].Value;

                    _Commands.Add(new(container_class, command_method, command_name));
                }
            }
        }
        catch (Exception e)
        {
            Log.Add(e.Message);
        }
    }

    public IEnumerator<ViewModelCommandInfo> GetEnumerator() => _Commands.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Commands).GetEnumerator();
}

internal record ViewModelCommandInfo(INamedTypeSymbol Class, INamedTypeSymbol CommandMethod, INamedTypeSymbol? CommandName);

internal class ViewModelCommandCodeWriter
{
    private readonly StringBuilder _Content = new();

    public int IndentLevel { get; set; }

    public ViewModelCommandCodeWriter Append(string line)
    {
        _Content.Append(line);
        return this;
    }

    public ViewModelCommandCodeWriter AppendLine(string line)
    {
        Append(new string(' ', IndentLevel * 4));
        _Content.AppendLine(line);
        return this;
    }

    public ViewModelCommandCodeWriter AppendLine()
    {
        _Content.AppendLine();
        return this;
    }

    #region Scope

    public IDisposable BeginScope(string line)
    {
        AppendLine(line);
        return BeginScope();
    }

    public IDisposable BeginScope()
    {
        _Content.Append(new string(' ', IndentLevel * 4)).AppendLine("{");
        IndentLevel++;
        return new ScopeTracker(this);
    }

    private class ScopeTracker : IDisposable
    {
        private readonly ViewModelCommandCodeWriter _Writer;

        public ScopeTracker(ViewModelCommandCodeWriter Writer) => _Writer = Writer;

        public void Dispose()
        {
            _Writer.IndentLevel--;
            _Writer._Content.Append(new string(' ', _Writer.IndentLevel * 4)).AppendLine("}");
        }
    } 

    #endregion
}