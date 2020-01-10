using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Path
    {
        public Router SourceRouter;
        public Router DestinationRouter;
        public List<Router> PathToDestination = new List<Router>();

        /// <summary>
        /// This function returns the path (from the list of paths) which matches the given parameters
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="sourceRouter"></param>
        /// <param name="destinationRouter"></param>
        /// <returns></returns>
        public static Path GetPath(List<Path> paths, Router sourceRouter, Router destinationRouter) =>
            (from path in paths
             where path.SourceRouter == sourceRouter
             select path).FirstOrDefault();
    }
}
