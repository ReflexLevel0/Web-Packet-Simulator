using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace WebPacketSimulator.Wpf
{
    static class FileHandler
    {
        static Exception invalidFileException = new Exception("This file is invalid!");
        public static string FileExtension = "webps";
        public static string FileDialogFilter = string.Format("Web Packet Simulator file (*.{0})|*.{0}", FileHandler.FileExtension);

        public static void SaveFile(IEnumerable<WpfRouter> routers, IEnumerable<Connection> connections, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            //Making text and writing it into a file
            StringBuilder text = new StringBuilder(1024);
            foreach (var router in routers)
            {
                text.AppendLine(string.Format("{0} {1} {2} {3}",
                                          router.RouterCanvas.Margin.Left,
                                          router.RouterCanvas.Margin.Top,
                                          router.Router.Address,
                                          router.Router.Name));
            }
            text.AppendLine();
            foreach (var connection in connections)
            {
                text.AppendLine(string.Format("{0} {1}",
                                routers.ToList().IndexOf(connection.SourceRouter),
                                routers.ToList().IndexOf(connection.DestinationRouter)));
            }
            File.WriteAllText(filePath, text.ToString());
        }

        /// <summary>
        /// This function loads the chosen file
        /// </summary>
        /// <param name="fileName"> Path to the file to be opened </param>
        /// <returns></returns>
        public static void LoadFile(string fileName)
        {
            var text = File.ReadAllText(fileName).Replace("\r", "");
            var lines = text.Split('\n');
            bool firstPart = true;
            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    firstPart = false;
                    continue;
                }
                var lineParts = line.Split(' ');
                if (firstPart)
                {
                    var routerInfo = lineParts.ToRouterInfo();
                    WpfRouter.CreateRouter(routerInfo.location, routerInfo.address, routerInfo.name);
                }
                else
                {
                    var connectionInfo = lineParts.ToConnectionInfo();
                    WpfRouter.ConnectRouters(WpfRouter.Routers[connectionInfo.sourceRouterIndex],
                                             WpfRouter.Routers[connectionInfo.destinationRouterIndex]);
                }
            }
        }

        /// <summary>
        /// Gets router information from line parts
        /// </summary>
        /// <param name="lineParts"></param>
        /// <returns></returns>
        private static (Point location, string address, string name) ToRouterInfo(this string[] lineParts)
        {
            if (lineParts.Length < 2)
            {
                throw invalidFileException;
            }

            var location = new Point();
            string address = null, name = null;
            int counter = 0;
            while (counter != lineParts.Length)
            {
                switch (counter)
                {
                    case 0:
                        try
                        {
                            location.X = int.Parse(lineParts[0]);
                        }
                        catch
                        {
                            throw invalidFileException;
                        }
                        break;
                    case 1:
                        try
                        {
                            location.Y = int.Parse(lineParts[1]);
                        }
                        catch
                        {
                            throw invalidFileException;
                        }
                        break;
                    case 2:
                        address = lineParts[2];
                        break;
                    case 3:
                        name = lineParts[3];
                        break;
                }
                counter++;
            }
            return (location, address, name);
        }

        /// <summary>
        /// Gets connection information from line parts
        /// </summary>
        /// <param name="lineParts"></param>
        /// <returns></returns>
        private static (int sourceRouterIndex, int destinationRouterIndex) ToConnectionInfo(this string[] lineParts)
        {
            if (lineParts.Length != 2)
            {
                throw invalidFileException;
            }

            int source, destination;
            try
            {
                source = int.Parse(lineParts[0]);
                destination = int.Parse(lineParts[1]);
            }
            catch
            {
                throw invalidFileException;
            }

            return (source, destination);
        }
    }
}