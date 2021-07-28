using DLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DLS.WPF.Helpers
{
    class LabelingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            var data = item as ViewModel.Wrapper.LabelingDataWrapper;

            FrameworkElement frameworkElement = container as FrameworkElement;
            DataTemplate dataTemplate = null;

            switch (data.LabelingType)
            {
                case Model.Enums.LabelingType.Rect:
                    dataTemplate = (DataTemplate)frameworkElement.FindResource("RectDataTemplate");
                    break;
                case Model.Enums.LabelingType.Polygon:
                    dataTemplate = (DataTemplate)frameworkElement.FindResource("PolygonDataTemplate");
                    break;
            }

            return dataTemplate;
        }
    }

    class SelectedLabelingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            var data = item as ViewModel.Wrapper.LabelingDataWrapper;

            FrameworkElement frameworkElement = container as FrameworkElement;
            DataTemplate dataTemplate = null;

            switch (data.LabelingType)
            {
                case Model.Enums.LabelingType.Rect:
                    dataTemplate = (DataTemplate)frameworkElement.FindResource("SelectedRectDataTemplate");
                    break;
                case Model.Enums.LabelingType.Polygon:
                    dataTemplate = (DataTemplate)frameworkElement.FindResource("SelectedPolygonDataTemplate");
                    break;
            }

            return dataTemplate;
        }
    }
}
