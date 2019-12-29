using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.StringConverters
{
	[MarkupExtensionReturnType(typeof(ToUpper)), ValueConversion(typeof(string), typeof(string))]
	public class ToUpper : ValueConverter
	{
		protected override object Convert(object v, Type t, object p, CultureInfo c) => ((string)v).ToUpper();
	}
}