using System.Text;

using Microsoft.CodeAnalysis.Text;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class StringBuilderEx
{
    public static IEnumerable<string> EnumLines(this StringBuilder builder) => builder.ToString().EnumLines();

    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, T> selector) => builder.ToString().EnumLines(selector);
    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, int, T> selector) => builder.ToString().EnumLines(selector);

    public static SourceText ToSource(this StringBuilder builder) => SourceText.From(builder.ToString(), Encoding.UTF8);

    public static string ToNumeratedLinesString(this StringBuilder builder) => builder.EnumLines(static (s, i) => $"{i + 1,3}|{s}").JoinStringLN();

    public static StringBuilder Using(this StringBuilder builder, string NameSpace) => builder
        .Append("using ")
        .Append(NameSpace)
        .AppendLine(";");

    public static StringBuilder Namespace(this StringBuilder builder, string Namespace) => builder
        .Append("namespace ")
        .Append(Namespace)
        .AppendLine(";");

    public static StringBuilder Nullable(this StringBuilder builder, bool Enable = true)
    {
        //if (builder is not [.., '\n'])
        if (builder.Length < 2 || builder[builder.Length - 1] != '\n')
            builder.LN();
        return builder.AppendLine(Enable ? "#nullable enable" : "#nullable disable");
    }

    public static StringWriter CreateWriter(this StringBuilder builder) => new(builder);

    public readonly ref struct RegionBuilder
    {
        private readonly int _Ident;
        private readonly bool _FreeLineOffset;

        public StringBuilder Source { get; }

        public RegionBuilder(StringBuilder source, string RegionName, int Ident = 1, bool FreeLineOffset = true)
        {
            Source = source;
            _Ident = Ident;
            _FreeLineOffset = FreeLineOffset;
            source.Ident(Ident).Append("# region {0}", RegionName).LN();
            if (FreeLineOffset)
                source.LN();
        }

        public void Dispose()
        {
            var source = Source;
            if (_FreeLineOffset)
                source.LN();
            source.Ident(_Ident).Append("#endregion").LN();
        }
    }

    public static RegionBuilder Region(this StringBuilder builder, string RegionName, int Ident = 1, bool FreeLineOffset = true) =>
        new(builder, RegionName, Ident, FreeLineOffset);

    public static StringBuilder AddProperty(this StringBuilder source, string Type, string FieldName, string PropertyName) => source
        .Append("    public {0} {1}", Type, PropertyName).LN()
        .Append("    {").LN()
        .Append("        get => {0};", FieldName).LN()
        .Append("        set => {0} = value;", FieldName).LN()
        .Append("    }").LN();

    public static StringBuilder AddNotifyProperty(
        this StringBuilder source,
        string Type,
        string FieldName,
        string PropertyName,
        bool GenerateEvent = false,
        string? Comment = null)
    {
        if (Comment is { Length: > 0 })
        {
            if (!Comment.StartsWith("    "))
                source.Append("    ");
            source.Append(Comment.TrimEnd()).LN();
        }
        source.Append("    public {0} {1}", Type, PropertyName).LN();
        source.Append("    {").LN();
        source.Append("        get => {0};", FieldName).LN();

        if (GenerateEvent)
        {
            source.Append("        set").LN();
            source.Append("        {").LN();
            source.Append("            if(Equals(value, {0})) return;", FieldName).LN();
            source.Append("            {0} = value;", FieldName).LN();
            source.Append("            {0}Changed?.Invoke(this, System.EventArgs.Empty);", PropertyName).LN();
            source.Append("        }").LN();
            source.Append("    }").LN();
            source.AppendLine();
            source.Append("    public System.EventHandler {0}Changed = null;", PropertyName).LN();
        }
        else
        {
            source.Append("        set => Set(ref {0}, value);", FieldName).LN();
            source.Append("    }").LN();
        }

        return source;
    }

    public static StringBuilder AddDependencyProperty(this StringBuilder source, string PropertyName, string PropertyTypeName)
    {
        source.Append("// dp:").Append(PropertyName).Append(" of type: ").Append(PropertyTypeName).LN();

        return source;
    }


    public static string ToStringWithLineNumbers(this StringBuilder builder, char Separator = '|') =>
        builder
            .ToString()
            .Split(["\r\n"], StringSplitOptions.None)
            .Select((s, i) => $"{i + 1,3}{Separator}{s}")
            .JoinStringLN();

    // [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0) => builder.AppendFormat(Format, arg0);

    // [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1);

    // [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2);

    // [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, params object[] args) => builder.AppendFormat(Format, args);

    // [StringFormatMethod("Format")]
    public static StringBuilder AppendLine(this StringBuilder builder, string Format, object arg0) => builder.AppendFormat(Format, arg0);

    // [StringFormatMethod("Format")]
    public static StringBuilder AppendLine(this StringBuilder builder, string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1);

    // [StringFormatMethod("Format")]
    public static StringBuilder AppendLine(this StringBuilder builder, string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2);

    // [StringFormatMethod("Format")]
    public static StringBuilder AppendLine(this StringBuilder builder, string Format, params object[] args) => builder.AppendFormat(Format, args);

    public static StringBuilder LN(this StringBuilder builder) => builder.AppendLine();
    public static StringBuilder LN(this StringBuilder builder, string str) => builder.AppendLine(str);
    // [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, string Format, object arg0) => builder.Append(Format, arg0).LN();
    // [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, string Format, object arg0, object arg1) => builder.Append(Format, arg0, arg1).LN();
    // [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, string Format, object arg0, object arg1, object arg2) => builder.Append(Format, arg0, arg1, arg2).LN();
    // [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, string Format, params object[] args) => builder.Append(Format, args).LN();

    public static StringBuilder Ident(this StringBuilder builder, int Level = 1, string Ident = "    ")
    {
        while (Level-- > 0)
            builder.Append(Ident);
        return builder;
    }
}