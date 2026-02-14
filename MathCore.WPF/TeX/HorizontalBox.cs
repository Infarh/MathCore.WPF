using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>
/// Бокс, содержащий горизонтальный стек дочерних боксов
/// </summary>
/// <remarks>
/// HorizontalBox используется для размещения боксов в горизонтальном направлении.
/// Поддерживает выравнивание (влево, по центру, вправо).
/// </remarks>
internal sealed class HorizontalBox : Box
{
    /// <summary>Общая ширина всех дочерних боксов</summary>
    private double _ChildBoxesTotalWidth;

    /// <summary>
    /// Инициализирует новый бокс с одним элементом и заданным выравниванием
    /// </summary>
    /// <param name="box">Элемент для размещения</param>
    /// <param name="width">Требуемая ширина контейнера</param>
    /// <param name="alignment">Выравнивание (Center, Left, Right)</param>
    public HorizontalBox(Box box, double width, TexAlignment alignment)
        : this()
    {
        var extra_width = width - box.Width;
        switch(alignment)
        {
            case TexAlignment.Center:
                var strut_box = new StrutBox(extra_width / 2, 0, 0, 0);
                Add(strut_box);
                Add(box);
                Add(strut_box);
                break;
            case TexAlignment.Left:
                Add(box);
                Add(new StrutBox(extra_width, 0, 0, 0));
                break;
            case TexAlignment.Right:
                Add(new StrutBox(extra_width, 0, 0, 0));
                Add(box);
                break;
        }
    }

    /// <summary>Инициализирует новый горизонтальный бокс с одним элементом</summary>
    /// <param name="box">Элемент для добавления</param>
    public HorizontalBox(Box box) : this() => Add(box);

    /// <summary>Инициализирует новый горизонтальный бокс с кистями для отрисовки</summary>
    /// <param name="foreground">Кисть переднего плана</param>
    /// <param name="background">Кисть фона</param>
    public HorizontalBox(Brush foreground, Brush background) : base(foreground, background) { }

    /// <summary>Инициализирует новый пустой горизонтальный бокс</summary>
    public HorizontalBox() { }

    /// <summary>Добавляет дочерний бокс и обновляет общую ширину</summary>
    /// <param name="box">Бокс для добавления</param>
    public override void Add(Box box)
    {
        base.Add(box);

        _ChildBoxesTotalWidth += box.Width;
        Width                =  Math.Max(Width, _ChildBoxesTotalWidth);
        Height               =  Math.Max(Children.Count == 0 ? double.NegativeInfinity : Height, box.Height - box.Shift);
        Depth                =  Math.Max(Children.Count == 0 ? double.NegativeInfinity : Depth, box.Depth + box.Shift);
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        base.Draw(Context, scale, x, y);

        var cur_x = x;
        foreach(var box in Children)
        {
            box.Draw(Context, scale, cur_x, y + box.Shift);
            cur_x += box.Width;
        }
    }

    public override int GetLastFontId()
    {
        var font_id = TexFontUtilities.NoFontId;
        foreach(var child in Children)
        {
            font_id = child.GetLastFontId();
            if(font_id == TexFontUtilities.NoFontId)
                break;
        }
        return font_id;
    }
}