// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotViewTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="PlotView" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using ExampleLibrary;

    using NUnit.Framework;

    using OxyPlot.Axes;
    using OxyPlot.Series;

    using PlotCommands = OxyPlot.PlotCommands;

    /// <summary>
    /// Provides unit tests for the <see cref="PlotView" /> class.
    /// </summary>
    [TestFixture]
    public class PlotViewTests
    {
        /// <summary>
        /// Provides unit tests for the <see cref="PlotView.ActualModel" /> property.
        /// </summary>
        public class ActualModel
        {
            /// <summary>
            /// Gets the actual model when model is not set.
            /// </summary>
            // [Test, Ignore("")] // TODO: add ignore reason.
            [Test, Apartment(ApartmentState.STA)]
            public void GetDefault()
            {
                var w = new Window();
                var plotView = new PlotView();
                w.Content = plotView;
                // var model = new PlotModel();
                // CreateHeatMapCategory(model);
                // var model = TrackerExamples.NoInterpolation();
                // var model = TrackerExamples.TrackerChangedEvent();
                var example = TrackerExamples.TrackerFiresDistance();
                var model = example.Model;
                var controller = example.Controller;
                
                // create a new plot controller with default bindings
                // var controller = new PlotController();

                // add a tracker command to the mouse enter event
                
                // 마우스 클릭을 해야만 Tracking 수행
                // controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);
                
                // 무조건 Tracking 
                // controller.BindMouseEnter(PlotCommands.HoverTrack);
                plotView.Controller = controller;
                // var model = ImageAnnotationExamples.ImageAnnotation();
                plotView.Model = model;

                w.Closed += (sender, args) =>
                    {
                        // Stop the Dispatcher when the window is closed.
                        Dispatcher.CurrentDispatcher.InvokeShutdown();
                    };

                w.Show();

                // Start the dispatcher to make it interactive.
                Dispatcher.Run();
                // Assert.IsNotNull(plotView.ActualModel);
            }

            

            /// <summary>
            /// Gets the actual model from the same thread that created the <see cref="PlotView" />.
            /// </summary>
            [Test]
            public void GetFromSameThread()
            {
                var model = new PlotModel();
                var plotView = new PlotView { Model = model };
                Assert.AreEqual(model, plotView.ActualModel);
            }

            /// <summary>
            /// Gets the actual model from a thread different from the one that created the <see cref="PlotView" />.
            /// </summary>
            [Test]
            public void GetFromOtherThread()
            {
                var model = new PlotModel();
                var plotView = new PlotView { Model = model };
                PlotModel actualModel = null;
                Task.Factory.StartNew(() => actualModel = plotView.ActualModel).Wait();
                Assert.AreEqual(model, actualModel);
            }
        }

        /// <summary>
        /// Provides unit tests for the <see cref="PlotView.InvalidatePlot" /> method.
        /// </summary>
        public class InvalidatePlot
        {
            /// <summary>
            /// Invalidates the plotView from the same thread that created the <see cref="PlotView" />.
            /// </summary>
            [Test]
            public void InvalidateFromSameThread()
            {
                var model = new PlotModel();
                var plotView = new PlotView { Model = model };
                plotView.InvalidatePlot();
            }

            /// <summary>
            /// Invalidates the plotView from a thread different from the one that created the <see cref="PlotView" />.
            /// </summary>
            [Test]
            public void InvalidateFromOtherThread()
            {
                var model = new PlotModel();
                var plotView = new PlotView { Model = model };
                Task.Factory.StartNew(() => plotView.InvalidatePlot()).Wait();
            }
        }

        /// <summary>
        /// Provides unit tests for the default values of the <see cref="PlotView" /> class.
        /// </summary>
        public class DefaultValues
        {
            /// <summary>
            /// Asserts that the default values are equal to the default values in the <see cref="PlotModel" />.
            /// </summary>
            [Test]
            public void PlotModelVsPlot()
            {
                var model = new PlotModel();
                var view = new PlotView();
                OxyAssert.PropertiesAreEqual(model, view);
            }
        }

        /// <summary>
        /// Make sure that the axis transforms has been initialized when showing a PlotView from a unit test.
        /// In this case, the Loaded event is not fired.
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public async Task PlotInifityPolyline()
        {
            var model = new PlotModel();
            var series = new OxyPlot.Series.LineSeries();
            series.Points.Add(new DataPoint(0, 0));
            series.Points.Add(new DataPoint(1, -1e40));
            model.Series.Add(series);

            var view = new PlotView { Model = model };
            var window = new Window { Height = 350, Width = 500, Content = view };

            Assert.DoesNotThrow(() => window.Show());
            Assert.IsNull(model.GetLastPlotException());
            await Task.Delay(5000);
            window.Close();

        }

        /// <summary>
        /// Make sure the PlotView does not throw an exception if it is invalidated while not in the visual tree.
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void InvalidateDisconnected()
        {
            var model = new PlotModel();

            var view = new PlotView { Model = model };
            var window = new Window { Height = 350, Width = 500, Content = view };

            Assert.DoesNotThrow(() => window.Show());
            Assert.DoesNotThrow(() => view.InvalidatePlot());
            window.Content = null;
            Assert.DoesNotThrow(() => view.InvalidatePlot());
        }


        public static void CreateHeatMapCategory(PlotModel model)
        {
            // Weekday axis (horizontal)
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,

                // Key used for specifying this axis in the HeatMapSeries
                Key = "WeekdayAxis",

                // Array of Categories (see above), mapped to one of the coordinates of the 2D-data array
                ItemsSource = new[]
                                                     {
                                                             "Monday",
                                                             "Tuesday",
                                                             "Wednesday",
                                                             "Thursday",
                                                             "Friday",
                                                             "Saturday",
                                                             "Sunday"
                                                         }
            });

            // Cake type axis (vertical)
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = "CakeAxis",
                ItemsSource = new[]
                                                     {
                                                             "Apple cake",
                                                             "Baumkuchen",
                                                             "Bundt cake",
                                                             "Chocolate cake",
                                                             "Carrot cake"
                                                         }
            });

            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
            });

            var rand = new Random();
            var data = new double[7, 5];
            for (int x = 0; x < 5; ++x)
            {
                for (int y = 0; y < 7; ++y)
                {
                    data[y, x] = rand.Next(0, 200) * (0.13 * (y + 1));
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = 6,
                Y0 = 0,
                Y1 = 4,
                XAxisKey = "WeekdayAxis",
                YAxisKey = "CakeAxis",
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                // TrackerFormatString = "X: {0}\nY: {1}\nValue: {2:0.00}",
                Data = data
            };
            
            model.Series.Add(heatMapSeries);
        }

    }
}
