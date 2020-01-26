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
        /// <param name="populateCanvas"> If true, canvas will be populated with loaded components </param>
        /// <returns></returns>
        public static (List<RouterInfo> RouterInfos, List<ConnectionInfo> ConnectionInfos) LoadFile(string fileName, bool populateCanvas)
        {
            if (File.Exists(fileName) == false)
            {
                throw new Exception("Error in opening file!");
            }

            List<RouterInfo> routerInfos = new List<RouterInfo>();
            List<ConnectionInfo> connectionInfos = new List<ConnectionInfo>();
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
                    routerInfos.Add(lineParts.ToRouterInfo());
                }
                else
                {
                    connectionInfos.Add(lineParts.ToConnectionInfo(routerInfos));
                }
            }
            if (populateCanvas)
            {
                PopulateCanvas(routerInfos, connectionInfos);
                MainWindow.CurrentFilePath = fileName;
            }
            return (routerInfos, connectionInfos);
        }

        /// <summary>
        /// This function populates a canvas with components
        /// </summary>
        /// <param name="routerInfos"> Info about routers which will be created </param>
        /// <param name="connectionInfos"> Info about connection which will be created </param>
        static void PopulateCanvas(List<RouterInfo> routerInfos, List<ConnectionInfo> connectionInfos)
        {
            //Deleting current data and loading data from the file
            while (WpfRouter.Routers.Count > 0)
            {
                WpfRouter.Routers[0].Delete();
            }
            while (Connection.Connections.Count > 0)
            {
                Connection.Connections[0].Delete();
            }

            //Populating canvas with loaded data
            foreach (var router in routerInfos)
            {
                WpfRouter.CreateRouter(router.Location, router.Address, router.Name);
            }
            foreach (var connection in connectionInfos)
            {
                WpfRouter.ConnectRouters(WpfRouter.Routers[routerInfos.IndexOf(connection.SourceRouterInfo)],
                                         WpfRouter.Routers[routerInfos.IndexOf(connection.DestinationRouterInfo)]);
            }
        }

        /// <summary>
        /// Gets router information from line parts
        /// </summary>
        /// <param name="lineParts"></param>
        /// <returns></returns>
        private static RouterInfo ToRouterInfo(this string[] lineParts)
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
                        address = string.IsNullOrEmpty(lineParts[2]) ? null : lineParts[2];
                        break;
                    case 3:
                        name = string.IsNullOrEmpty(lineParts[3]) ? null : lineParts[3];
                        break;
                }
                counter++;
            }
            return new RouterInfo() 
            {
                Location = location,
                Address = address,
                Name = name
            };
        }

        /// <summary>
        /// Gets connection information from line parts
        /// </summary>
        /// <param name="lineParts"></param>
        /// <param name="routerInfos"> Router information </param>
        /// <returns></returns>
        private static ConnectionInfo ToConnectionInfo(this string[] lineParts, List<RouterInfo> routerInfos)
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

            return new ConnectionInfo() 
            { 
                SourceRouterInfo = routerInfos[source], 
                DestinationRouterInfo = routerInfos[destination]
            };
        }
    }
}