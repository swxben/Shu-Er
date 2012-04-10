using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace meftest
{
    [InheritedExport]
    public interface Foo { string Name { get; } }
    public class TallFoo : Foo { public string Name { get { return "I am a tall foo"; } } }
    public class ShortFoo : Foo { public string Name { get { return "I am a short foo"; } } }

    class Program
    {
        [ImportMany]
        IEnumerable<Foo> foos;

        void Run()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(this.GetType().Assembly));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            Console.WriteLine("Foos:");
            foreach (var foo in foos) Console.WriteLine(foo.Name);
            Console.ReadKey();
        }



        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}