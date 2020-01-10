using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Linq;

namespace Common
{
    public class Router
    {
        public static List<Router> Routers = new List<Router>();
        public Point Location;
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
            while (true)
            {
                //Updating paths to routers
                Path pathToCurrentRouter = Path.GetPath(paths, this, currentRouter);
                foreach(var router in currentRouter.LinkedRouters)
                {
                    Path path = new Path() { 
                        SourceRouter = currentRouter, 
                        DestinationRouter = router, 
                        PathToDestination = pathToCurrentRouter.PathToDestination };
                    path.PathToDestination.Add(router);
                    paths.Add(path);
                }
                visitedRouters.Add(currentRouter);

                //Going to the next unvisited router
                bool hasUnvisitedRouters = false;
                foreach(var router in currentRouter.LinkedRouters)
                {
                    if(visitedRouters.Contains(router) == false)
                    {
                        hasUnvisitedRouters = true;
                        currentRouter = router;
                        break;
                    }
                }

                //If all routers around this router have been visited
                if(hasUnvisitedRouters == false)
                {
                    Router newRouter = (from router in routers
                                     where visitedRouters.Contains(router) == false
                                     select router).FirstOrDefault();
                    if(newRouter == null)
                    {
                        break;
                    }
                    currentRouter = newRouter;
                }
            }

            //Returning the result
            var result = Path.GetPath(paths, this, destination);
            if(result == null)
            {
                throw new Exception("Getting to destination router is impossible!");
            }
            return result.PathToDestination;
        }
    }
}