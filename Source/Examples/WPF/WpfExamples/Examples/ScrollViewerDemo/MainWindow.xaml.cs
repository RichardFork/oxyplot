// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ScrollViewerDemo
{
    using System.Windows;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    using WpfExamples;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("Shows a plot inside a ScrollViewer.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var plotModel = new PlotModel { Title = "Plot in ScrollViewer" };
            plotModel.Axes.Add(new LinearAxis
                                   {
                                       Position = AxisPosition.Left,
                                       IsZoomEnabled = false
            });
            plotModel.Axes.Add(new LinearAxis
                                   {
                                       Position = AxisPosition.Bottom, Minimum = 0,
                                       Maximum = 100,
                                       AbsoluteMinimum = 00,
                                       AbsoluteMaximum = 200,
                                       

            });
            var controller = new PlotController();
            // controller.BindMouseWheel(PlotCommands.ZoomWheel);
            controller.BindMouseWheel(OxyModifierKeys.None,
                new DelegatePlotCommand<OxyMouseWheelEventArgs>(
                    (view, cont, args) =>
                        {
                            var currentAxis = view.ActualModel.DefaultXAxis;

                            var delta = args.Delta;
                            double change = (currentAxis.Transform(currentAxis.Maximum) - currentAxis.Transform(currentAxis.Minimum)) * delta / 400;
                            currentAxis.Pan(change);
                            view.InvalidatePlot(false);
                        }));
            
            this.Plot = plotModel;
            this.plotview.Controller = controller;
            this.DataContext = this;
        }

        public PlotModel Plot { get; set; }
    }
}
