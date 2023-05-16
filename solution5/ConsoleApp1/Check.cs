using ClassLibrary1;

namespace MyConsoleApp
{
    class Check
    {
        static void Main(string[] args)
        {
            FRaw fraw = x => x * x;
            RawData rdata1 = new RawData(0, 10, 20, true, fraw);
            rdata1.Save("rdata1.txt");
            SplineData sd1 = new SplineData(rdata1, 0, 2, 3);
            sd1.DoSplines();
            Console.WriteLine(sd1.Integral);
            for (int i = 0; i < sd1.NumNodes; i++)
            {
                Console.WriteLine(sd1.SplineDataItems[i]);
            }
        }
    }
}