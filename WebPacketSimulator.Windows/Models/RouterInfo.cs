using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WebPacketSimulator.Wpf
{
    public struct RouterInfo
    {
        public string Address;
        public string Name;
        public Point Location;

        /// <summary>
        /// This function compares two <see cref="RouterInfo"/> collections
        /// </summary>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns></returns>
        public static bool AreCollectionsSame(IEnumerable<RouterInfo> collection1, IEnumerable<RouterInfo> collection2)
        {
            if (collection1.Count() != collection2.Count())
            {
                return false;
            }

            var dummyCollection1 = collection1.ToList();
            var dummyCollection2 = collection2.ToList();
            while (dummyCollection1.Count > 0)
            {
                int index = dummyCollection2.IndexOf(dummyCollection1[0]);
                if(index == -1)
                {
                    return false;
                }
                dummyCollection1.RemoveAt(0);
                dummyCollection2.RemoveAt(index);
            }
            return true;
        }

        public static RouterInfo WpfRouterToRouterInfo(WpfRouter router)
        {
            var margin = router.RouterCanvas.Margin;
            return new RouterInfo()
            {
                Address = router.Router.Address,
                Name = router.Router.Name,
                Location = new Point(margin.Left, margin.Top)
            };
        }

        public static List<RouterInfo> WpfRoutersToRouterInfos(IEnumerable<WpfRouter> routers)
        {
            var routerInfos = new List<RouterInfo>();
            foreach(var router in routers)
            {
                routerInfos.Add(WpfRouterToRouterInfo(router));
            }
            return routerInfos;
        }
    }
}