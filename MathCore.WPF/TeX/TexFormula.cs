using System.Diagnostics;

namespace MathCore.WPF.TeX;

/// <summary>
/// Представляет математическую формулу, которая может быть отрисована в WPF контексте
/// </summary>
/// <remarks>
/// Класс служит контейнером для иерархии атомов (Atom) и управляет процессом преобразования
/// формулы в боксы для рендеринга. Основной цикл жизни:
/// 1. Парсинг TeX строки создаёт TexFormula с корневым атомом
/// 2. Вызов GetRenderer() преобразует атомы в боксы
/// 3. Рендерер отрисовывает боксы в DrawingContext WPF
/// </remarks>
/// <example>
/// <code><![CDATA[
/// // Создание формулы из списка подформул
/// var formula = new TexFormula(new List<TexFormula> 
/// { 
///     subFormula1, 
///     subFormula2 
/// });
/// 
/// // Получение рендерера и отрисовка
/// var renderer = formula.GetRenderer(TexStyle.Display, scale: 20.0);
/// using (var drawingGroup = new DrawingGroup())
/// {
///     var dc = drawingGroup.Open();
///     renderer.Render(dc, 10, 10);
///     dc.Close();
/// }
/// ]]></code>
/// </example>
public sealed class TexFormula
{
    /// <summary>
    /// Инициализирует новый экземпляр TexFormula из списка подформул
    /// </summary>
    /// <param name="FormulaList">Список подформул для объединения в одну формулу</param>
    /// <remarks>
    /// Если в списке одна формула, она используется как есть. 
    /// Если несколько - они объединяются в RowAtom
    /// </remarks>
    public TexFormula(List<TexFormula> FormulaList)
    {
        Debug.Assert(FormulaList != null);

        if(FormulaList.Count == 1)
            Add(FormulaList[0]);
        else
            RootAtom = new RowAtom(FormulaList);
    }

    /// <summary>Текстовый стиль, применяемый к формуле</summary>
    public string TextStyle { get; set; }

    /// <summary>Корневой атом формулы (основная единица TeX формулы)</summary>
    internal Atom? RootAtom { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр TexFormula на основе другой формулы
    /// </summary>
    /// <param name="formula">Исходная формула для копирования</param>
    public TexFormula(TexFormula formula)
    {
        Debug.Assert(formula != null);

        Add(formula);
    }

    /// <summary>Инициализирует новый пустой экземпляр TexFormula</summary>
    public TexFormula() { }

    /// <summary>
    /// Получает рендерер для отрисовки формулы в WPF контексте
    /// </summary>
    /// <param name="style">Математический стиль (Display, Text, Script, ScriptScript)</param>
    /// <param name="scale">Коэффициент масштабирования (обычно размер шрифта в пиксельных точках)</param>
    /// <returns>Рендерер, готовый к отрисовке формулы</returns>
    /// <remarks>
    /// Этот метод преобразует иерархию атомов в иерархию боксов и создаёт рендерер,
    /// который может отрисовать формулу в DrawingContext WPF
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// var formula = new TexFormula();
    /// // ... добавить атомы в формулу ...
    /// 
    /// var renderer = formula.GetRenderer(TexStyle.Display, 20.0);
    /// renderer.Render(drawingContext, 10, 10);
    /// ]]></code>
    /// </example>
    public TexRenderer GetRenderer(TexStyle style, double scale)
    {
        var environment = new TexEnvironment(style, new DefaultTexFont(scale));
        return new(CreateBox(environment), scale);
    }

    /// <summary>
    /// Добавляет подформулу к текущей формуле
    /// </summary>
    /// <param name="formula">Подформула для добавления</param>
    /// <remarks>
    /// Если добавляемая формула содержит RowAtom, он оборачивается в новый RowAtom.
    /// Остальные атомы добавляются как есть.
    /// </remarks>
    public void Add(TexFormula formula)
    {
        Debug.Assert(formula != null);
        Debug.Assert(formula.RootAtom != null);

        Add(formula.RootAtom is RowAtom ? new RowAtom(formula.RootAtom) : formula.RootAtom);
    }

    /// <summary>Добавляет атом к корневому атому формулы</summary>
    /// <param name="atom">Атом для добавления</param>
    /// <remarks>
    /// Если корневой атом ещё не установлен, атом становится корневым.
    /// Если корневой атом уже существует и не является RowAtom, он оборачивается в RowAtom.
    /// Затем новый атом добавляется в RowAtom.
    /// </remarks>
    internal void Add(Atom atom)
    {
        Debug.Assert(atom != null);
        if(RootAtom is null)
            RootAtom = atom;
        else
        {
            if(RootAtom is not RowAtom)
                RootAtom = new RowAtom(RootAtom);
            ((RowAtom)RootAtom).Add(atom);
        }
    }

    ///<summary>
    /// Преобразует корневой атом в бокс, готовый к рендерингу
    /// </summary>
    /// <param name="environment">Среда рендеринга, содержащая стиль и информацию о шрифте</param>
    /// <returns>Бокс, который может быть отрисован в DrawingContext WPF</returns>
    /// <remarks>
    /// Если корневой атом не установлен, возвращается пустой StrutBox.
    /// Для каждого атома вызывается метод CreateBox для получения его визуального представления.
    /// </remarks>
    internal Box CreateBox(TexEnvironment environment)
    {
        var root = RootAtom;
        return root is null ? StrutBox.Empty : root.CreateBox(environment);
    }
}