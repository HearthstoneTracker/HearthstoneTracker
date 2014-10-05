// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="GridViewColumnResize.cs">
//   
// </copyright>
// <summary>
//   Static class used to attach to wpf control
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Static class used to attach to wpf control
    /// </summary>
    public static class GridViewColumnResize
    {
        #region DependencyProperties

        /// <summary>
        /// The width property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(string), typeof(GridViewColumnResize), 
                                                new PropertyMetadata(OnSetWidthCallback));

        /// <summary>
        /// The grid view column resize behavior property.
        /// </summary>
        public static readonly DependencyProperty GridViewColumnResizeBehaviorProperty =
            DependencyProperty.RegisterAttached("GridViewColumnResizeBehavior", 
                                                typeof(GridViewColumnResizeBehavior), typeof(GridViewColumnResize), 
                                                null);

        /// <summary>
        /// The enabled property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(GridViewColumnResize), 
                                                new PropertyMetadata(OnSetEnabledCallback));

        /// <summary>
        /// The list view resize behavior property.
        /// </summary>
        public static readonly DependencyProperty ListViewResizeBehaviorProperty =
            DependencyProperty.RegisterAttached("ListViewResizeBehaviorProperty", 
                                                typeof(ListViewResizeBehavior), typeof(GridViewColumnResize), null);

        #endregion

        /// <summary>
        /// The get width.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetWidth(DependencyObject obj)
        {
            return (string)obj.GetValue(WidthProperty);
        }

        /// <summary>
        /// The set width.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetWidth(DependencyObject obj, string value)
        {
            obj.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// The get enabled.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        /// <summary>
        /// The set enabled.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        #region CallBack

        /// <summary>
        /// The on set width callback.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnSetWidthCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as GridViewColumn;
            if (element != null)
            {
                GridViewColumnResizeBehavior behavior = GetOrCreateBehavior(element);
                behavior.Width = e.NewValue as string;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type GridViewColumn but found " +
                                        dependencyObject.GetType().Name);
            }
        }

        /// <summary>
        /// The on set enabled callback.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnSetEnabledCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as ListView;
            if (element != null)
            {
                ListViewResizeBehavior behavior = GetOrCreateBehavior(element);
                behavior.Enabled = (bool)e.NewValue;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type ListView but found " + dependencyObject.GetType().Name);
            }
        }

        /// <summary>
        /// The get or create behavior.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="ListViewResizeBehavior"/>.
        /// </returns>
        private static ListViewResizeBehavior GetOrCreateBehavior(ListView element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as ListViewResizeBehavior;
            if (behavior == null)
            {
                behavior = new ListViewResizeBehavior(element);
                element.SetValue(ListViewResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        /// <summary>
        /// The get or create behavior.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="GridViewColumnResizeBehavior"/>.
        /// </returns>
        private static GridViewColumnResizeBehavior GetOrCreateBehavior(GridViewColumn element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior;
            if (behavior == null)
            {
                behavior = new GridViewColumnResizeBehavior(element);
                element.SetValue(GridViewColumnResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        #endregion

        #region Nested type: GridViewColumnResizeBehavior

        /// <summary>
        /// GridViewColumn class that gets attached to the GridViewColumn control
        /// </summary>
        public class GridViewColumnResizeBehavior
        {
            /// <summary>
            /// The _element.
            /// </summary>
            private readonly GridViewColumn _element;

            /// <summary>
            /// Initializes a new instance of the <see cref="GridViewColumnResizeBehavior"/> class.
            /// </summary>
            /// <param name="element">
            /// The element.
            /// </param>
            public GridViewColumnResizeBehavior(GridViewColumn element)
            {
                this._element = element;
            }

            /// <summary>
            /// Gets or sets the width.
            /// </summary>
            public string Width { get; set; }

            /// <summary>
            /// Gets a value indicating whether is static.
            /// </summary>
            public bool IsStatic
            {
                get { return this.StaticWidth >= 0; }
            }

            /// <summary>
            /// Gets the static width.
            /// </summary>
            public double StaticWidth
            {
                get
                {
                    double result;
                    return double.TryParse(this.Width, out result) ? result : -1;
                }
            }

            /// <summary>
            /// Gets the percentage.
            /// </summary>
            public double Percentage
            {
                get
                {
                    if (!this.IsStatic)
                    {
                        return this.Mulitplier * 100;
                    }

                    return 0;
                }
            }

            /// <summary>
            /// Gets the mulitplier.
            /// </summary>
            public double Mulitplier
            {
                get
                {
                    if (this.Width == "*" || this.Width == "1*") return 1;
                    if (this.Width.EndsWith("*"))
                    {
                        double perc;
                        if (double.TryParse(this.Width.Substring(0, this.Width.Length - 1), out perc))
                        {
                            return perc;
                        }
                    }

                    return 1;
                }
            }

            /// <summary>
            /// The set width.
            /// </summary>
            /// <param name="allowedSpace">
            /// The allowed space.
            /// </param>
            /// <param name="totalPercentage">
            /// The total percentage.
            /// </param>
            public void SetWidth(double allowedSpace, double totalPercentage)
            {
                if (this.IsStatic)
                {
                    this._element.Width = this.StaticWidth;
                }
                else
                {
                    double width = allowedSpace * (this.Percentage / totalPercentage);
                    this._element.Width = width;
                }
            }
        }

        #endregion

        #region Nested type: ListViewResizeBehavior

        /// <summary>
        /// ListViewResizeBehavior class that gets attached to the ListView control
        /// </summary>
        public class ListViewResizeBehavior
        {
            /// <summary>
            /// The margin.
            /// </summary>
            private const int Margin = 25;

            /// <summary>
            /// The refresh time.
            /// </summary>
            private const long RefreshTime = Timeout.Infinite;

            /// <summary>
            /// The delay.
            /// </summary>
            private const long Delay = 500;

            /// <summary>
            /// The _element.
            /// </summary>
            private readonly ListView _element;

            /// <summary>
            /// The _timer.
            /// </summary>
            private readonly Timer _timer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ListViewResizeBehavior"/> class.
            /// </summary>
            /// <param name="element">
            /// The element.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// </exception>
            public ListViewResizeBehavior(ListView element)
            {
                if (element == null) throw new ArgumentNullException("element");
                this._element = element;
                element.Loaded += this.OnLoaded;

                // Action for resizing and re-enable the size lookup
                // This stops the columns from constantly resizing to improve performance
                Action resizeAndEnableSize = () =>
                {
                    this.Resize();
                    this._element.SizeChanged += this.OnSizeChanged;
                };
                this._timer = new Timer(x => Application.Current.Dispatcher.BeginInvoke(resizeAndEnableSize), null, Delay, 
                                   RefreshTime);
            }

            /// <summary>
            /// Gets or sets a value indicating whether enabled.
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// The on loaded.
            /// </summary>
            /// <param name="sender">
            /// The sender.
            /// </param>
            /// <param name="e">
            /// The e.
            /// </param>
            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                this._element.SizeChanged += this.OnSizeChanged;
            }

            /// <summary>
            /// The on size changed.
            /// </summary>
            /// <param name="sender">
            /// The sender.
            /// </param>
            /// <param name="e">
            /// The e.
            /// </param>
            private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                if (e.WidthChanged)
                {
                    this._element.SizeChanged -= this.OnSizeChanged;
                    this._timer.Change(Delay, RefreshTime);
                }
            }

            /// <summary>
            /// The resize.
            /// </summary>
            private void Resize()
            {
                if (this.Enabled)
                {
                    double totalWidth = this._element.ActualWidth;
                    var gv = this._element.View as GridView;
                    if (gv != null)
                    {
                        double allowedSpace = totalWidth - GetAllocatedSpace(gv);
                        allowedSpace = allowedSpace - Margin;
                        double totalPercentage = GridViewColumnResizeBehaviors(gv).Sum(x => x.Percentage);
                        foreach (GridViewColumnResizeBehavior behavior in GridViewColumnResizeBehaviors(gv))
                        {
                            behavior.SetWidth(allowedSpace, totalPercentage);
                        }
                    }
                }
            }

            /// <summary>
            /// The grid view column resize behaviors.
            /// </summary>
            /// <param name="gv">
            /// The gv.
            /// </param>
            /// <returns>
            /// The <see cref="IEnumerable"/>.
            /// </returns>
            private static IEnumerable<GridViewColumnResizeBehavior> GridViewColumnResizeBehaviors(GridView gv)
            {
                foreach (GridViewColumn t in gv.Columns)
                {
                    var gridViewColumnResizeBehavior =
                        t.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior;
                    if (gridViewColumnResizeBehavior != null)
                    {
                        yield return gridViewColumnResizeBehavior;
                    }
                }
            }

            /// <summary>
            /// The get allocated space.
            /// </summary>
            /// <param name="gv">
            /// The gv.
            /// </param>
            /// <returns>
            /// The <see cref="double"/>.
            /// </returns>
            private static double GetAllocatedSpace(GridView gv)
            {
                double totalWidth = 0;
                foreach (GridViewColumn t in gv.Columns)
                {
                    var gridViewColumnResizeBehavior =
                        t.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior;
                    if (gridViewColumnResizeBehavior != null)
                    {
                        if (gridViewColumnResizeBehavior.IsStatic)
                        {
                            totalWidth += gridViewColumnResizeBehavior.StaticWidth;
                        }
                    }
                    else
                    {
                        totalWidth += t.ActualWidth;
                    }
                }

                return totalWidth;
            }
        }

        #endregion
    }
}