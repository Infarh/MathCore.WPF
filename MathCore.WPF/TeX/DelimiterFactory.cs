// Creates boxes containing delimeter symbol that exists in different sizes.
namespace MathCore.WPF.TeX;

internal static class DelimiterFactory
{
    public static Box CreateBox(string symbol, double MinHeight, TexEnvironment environment)
    {
        var tex_font  = environment.TexFont;
        var style    = environment.Style;
        var char_info = tex_font.GetCharInfo(symbol, style);

        // Find first version of character that has at least minimum height.
        var metrics     = char_info.Metrics;
        var total_height = metrics.Height + metrics.Depth;
        while(total_height < MinHeight && tex_font.HasNextLarger(char_info))
        {
            char_info    = tex_font.GetNextLargerCharInfo(char_info, style);
            metrics     = char_info.Metrics;
            total_height = metrics.Height + metrics.Depth;
        }

        if(total_height >= MinHeight)
        {
            // Character of sufficient height was found.
            return new CharBox(environment, char_info);
        }
        if(tex_font.IsExtensionChar(char_info))
        {
            var result_box = new VerticalBox();

            // Construct box from extension character.
            var extension = tex_font.GetExtension(char_info, style);
            if(extension.Top != null)
                result_box.Add(new CharBox(environment, extension.Top));
            if(extension.Middle != null)
                result_box.Add(new CharBox(environment, extension.Middle));
            if(extension.Bottom != null)
                result_box.Add(new CharBox(environment, extension.Bottom));

            // Insert repeatable part multiple times until box is high enough.
            var repeat_box = new CharBox(environment, extension.Repeat);
            do
            {
                if(extension.Top != null && extension.Bottom != null)
                {
                    result_box.Add(1, repeat_box);
                    if(extension.Middle != null)
                        result_box.Add(result_box.Children.Count - 1, repeat_box);
                }
                else if(extension.Bottom != null)
                {
                    result_box.Add(0, repeat_box);
                }
                else
                {
                    result_box.Add(repeat_box);
                }
            } while(result_box.Height + result_box.Depth < MinHeight);

            return result_box;
        }
        // No extensions available, so use tallest available version of character.
        return new CharBox(environment, char_info);
    }
}