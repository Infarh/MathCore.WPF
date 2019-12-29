using System.Windows;

namespace MathCore.WPF
{
    public static class DataFlowing
    {
        #region DataPipes (Attached DependencyProperty)

        public static readonly DependencyProperty DataFlowsProperty = DependencyProperty.RegisterAttached(
            "ShadowDataFlows",
            typeof(DataFlowCollection),
            typeof(DataFlowing),
            new FrameworkPropertyMetadata(null));

        public static void SetDataFlows(DependencyObject d, DataFlowCollection v) => d.SetValue(DataFlowsProperty, v);

        public static DataFlowCollection GetDataFlows(DependencyObject d)
        {
            var data_flow_collection = (DataFlowCollection)d.GetValue(DataFlowsProperty);
            if (data_flow_collection is null)
                SetDataFlows(d, data_flow_collection = new DataFlowCollection());
            return data_flow_collection;
        }

        #endregion
    }

    public class DataFlowCollection : FreezableCollection<DataFlow> { }

    public class DataFlow : Freezable
    {
        #region Source (DependencyProperty)

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(object),
            typeof(DataFlow),
            new FrameworkPropertyMetadata(null, (d, e) => ((DataFlow)d).Target = e.NewValue));

        public object Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        #endregion

        #region Target (DependencyProperty)

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            nameof(Target),
            typeof(object),
            typeof(DataFlow),
            new FrameworkPropertyMetadata(null));

        public object Target { get => GetValue(TargetProperty); set => SetValue(TargetProperty, value); }

        #endregion

        protected override Freezable CreateInstanceCore() => new DataFlow();
    }
}
