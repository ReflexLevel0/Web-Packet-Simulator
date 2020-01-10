using System;
using Common;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Router a, b;
            a = new Router() { Name = "A" };
            b = new Router() { Name = "B" };
            Router.Routers.Add(a);
            Router.Routers.Add(b);
            a.LinkedRouters.Add(b);
            b.LinkedRouters.Add(a);
            foreach(var router in a.GetPathToRouter(b))
            {
                Console.WriteLine(router.Name);
            }
        }
    }
}
