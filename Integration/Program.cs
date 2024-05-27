using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        //Server 1
        var service = new ItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("c"));

        Thread.Sleep(5000);
        Console.WriteLine("Everything recorded Server 1:");

        //Server 2
        var service2 = new ItemIntegrationService();

        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("a"));
        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("d"));
        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service2.SaveDiffirentItems("e"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded Server 2:");

        service.GetAllItems().ForEach(Console.WriteLine);
        service2.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}