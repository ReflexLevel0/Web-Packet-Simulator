using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Linq;

namespace WebPacketSimulator.Common
{
    public class Router
    {
        public List<Router> LinkedRouters = new List<Router>();
        public string Name { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// This function returns the shortest path from this router to the chosen router
        /// </summary>
        /// <param name="routers"> All routers </param>
        /// <param name="destination"> Destination router </param>
        /// <returns></returns>
        public List<Router> GetPathToRouter(List<Router> routers, Router destination)
        {
            List<Path> paths = new List<Path>();
            List<Router> visitedRouters = new List<Router>();
            Router currentRouter = new Router();

            //Going trough all routers
            currentRouter = this;
            paths.Add(new Path()
            {
                SourceRouter = this,
                DestinationRouter = this,
                PathToDestination = new List<Router>() { this }
            });
            while (true)
            {
                //Updating paths to routers
                Path pathToCurrentRouter = Path.GetPath(paths, this, currentRouter);
                foreach (var router in currentRouter.LinkedRouters)
                {
                    Path path = Path.GetPath(paths, this, router);

                    //Not updating path to router
                    //if current path is longer than the shortest path to the current router
                    if (path != null && pathToCurrentRouter != null &&
                       path.PathToDestination.Count <= pathToCurrentRouter.PathToDestination.Count)
                    {
                        continue;
                    }

                    //Updating path to router
                    if (path == null)
                    {
                        Path newPath = new Path()
                        {
                            SourceRouter = this,
                            DestinationRouter = router,
                            PathToDestination = (pathToCurrentRouter == null) ?
                                                 new List<Router>() :
                                                 pathToCurrentRouter.PathToDestination.ToArray().ToList()
                        };
                        newPath.PathToDestination.Add(router);
                        paths.Add(newPath);
                    }
                    else
                    {
                        path.PathToDestination = pathToCurrentRouter.PathToDestination
                                                 .ToArray().ToList();
                        path.PathToDestination.Add(router);
                    }
                    visitedRouters.Remove(router);
                }
                visitedRouters.Add(currentRouter);

                //Going to the next unvisited router
                bool hasUnvisitedRouters = false;
                foreach (var router in currentRouter.LinkedRouters)
                {
                    if (visitedRouters.Contains(router) == false)
                    {
                        hasUnvisitedRouters = true;
                        currentRouter = router;
                        break;
                    }
                }

                //If all routers around this router have been visited
                if (hasUnvisitedRouters == false)
                {
                    Router newRouter = (from router in routers
                                        where visitedRouters.Contains(router) == false
                                        select router).FirstOrDefault();
                    if (newRouter == null)
                    {
                        break;
                    }
                    currentRouter = newRouter;
                }
            }

            //Returning the result
            var result = Path.GetPath(paths, this, destination);
            if (result == null)
            {
                throw new Exception("Getting to destination router is impossible!");
            }
            return result.PathToDestination;
        }
    }
}