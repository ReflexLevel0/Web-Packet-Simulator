using System;
using Common;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Router a, b, c, d, e, f, g;
            a = new Router() { Name = "A" };
            b = new Router() { Name = "B" };
            c = new Router() { Name = "C" };
            d = new Router() { Name = "D" };
            e = new Router() { Name = "E" };
            f = new Router() { Name = "F" };
            g = new Router() { Name = "G" };

            Router.Routers.Add(a);
            Router.Routers.Add(b);
            Router.Routers.Add(c);
            Router.Routers.Add(d);
            Router.Routers.Add(e);
            Router.Routers.Add(f);
            Router.Routers.Add(g);

            //a.LinkedRouters.Add(b);
            //a.LinkedRouters.Add(c);

            //b.LinkedRouters.Add(a);
            //b.LinkedRouters.Add(d);

            //c.LinkedRouters.Add(a);
            //c.LinkedRouters.Add(d);

            //d.LinkedRouters.Add(b);
            //d.LinkedRouters.Add(c);
            //d.LinkedRouters.Add(e);

            //e.LinkedRouters.Add(d);

            a.LinkedRouters.Add(b);
            a.LinkedRouters.Add(e);
            b.LinkedRouters.Add(a);
            b.LinkedRouters.Add(c);
            c.LinkedRouters.Add(b);
            c.LinkedRouters.Add(d);
            d.LinkedRouters.Add(c);
            d.LinkedRouters.Add(g);
            e.LinkedRouters.Add(a);
            e.LinkedRouters.Add(f);
            f.LinkedRouters.Add(e);
            f.LinkedRouters.Add(g);
            g.LinkedRouters.Add(d);
            g.LinkedRouters.Add(f);

            foreach (var router in a.GetPathToRouter(Router.Routers, g))
            {
                Console.WriteLine(router.Name);
            }
        }
    }
}