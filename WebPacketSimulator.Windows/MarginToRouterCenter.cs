using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WebPacketSimulator.Wpf
{
    class MarginToRouterCenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = (Thickness)(value as Thickness?);
            
            //X position of the router's center will be returned if the passed parameter is true
            if((parameter as bool?) == true)
            {
                return margin.Left + WpfRouter.RouterImageWidth / 2;
            }

            //Y position of the router's position will be returned if the passed parameter is false
            else
            {
                return margin.Top + WpfRouter.RouterImageHeight / 2;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
