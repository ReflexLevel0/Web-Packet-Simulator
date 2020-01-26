using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WebPacketSimulator.Wpf
{
    class LineMarginToRouterCenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = (Thickness)value;
            var param = parameter as LineMarginToRouterConverterParameter;
            var point = param.Router.RouterImage.TransformToAncestor(param.Router.RouterCanvas).Transform(new Point(0, 0));

            //X position of the router's center will be returned if the passed parameter is true
            if (param.Left)
            {
                return margin.Left + point.X + WpfRouter.RouterImageWidth / 2;
            }

            //Y position of the router's position will be returned if the passed parameter is false
            else
            {
                return margin.Top + point.Y + WpfRouter.RouterImageHeight / 2;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}