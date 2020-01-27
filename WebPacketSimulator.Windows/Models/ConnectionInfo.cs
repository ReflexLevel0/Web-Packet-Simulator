using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPacketSimulator.Wpf
{
    public struct ConnectionInfo
    {
        public RouterInfo SourceRouterInfo;
        public RouterInfo DestinationRouterInfo;

        public static IEnumerable<ConnectionInfo> ConnectionsToConnectionInfos(List<Connection> connections)
        {
            foreach (var connection in connections)
            {
                yield return new ConnectionInfo()
                {
                    SourceRouterInfo = RouterInfo.WpfRouterToRouterInfo(connection.SourceRouter),
                    DestinationRouterInfo = RouterInfo.WpfRouterToRouterInfo(connection.DestinationRouter)
                };
            }
        }

        public static bool AreCollectionsSame(IEnumerable<ConnectionInfo> collection1, IEnumerable<ConnectionInfo> collection2)
        {
            if(collection1.Count() != collection2.Count())
            {
                return false;
            }

            var dummyCollection1 = collection1.ToList();
            var dummyCollection2 = collection2.ToList();
            while(dummyCollection1.Count > 0)
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
    }
}