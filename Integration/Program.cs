using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        //Single Server Scenerio
        Console.WriteLine("Single Server Scenerio Start");
        var service = new ItemIntegrationService();

        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveDiffirentItems("c"));

        Thread.Sleep(5000);
        Console.WriteLine("Everything recorded");
        Console.WriteLine("Single Server Scenerio Start");
        service.GetAllItems().ForEach(Console.WriteLine);
        //Multiple Sever Scenerio
        //Server 1
        Console.WriteLine("Multiple Server Scenerio Start");
        var multipleServerService = new ItemIntegrationService();
        
        service.SaveItemForDistributedSystems("a");
        service.SaveItemForDistributedSystems("b");
        service.SaveItemForDistributedSystems("c");



        service.SaveItemForDistributedSystems("a");
        service.SaveItemForDistributedSystems("b");
        service.SaveItemForDistributedSystems("c");


        Console.WriteLine("Everything recorded Server 1:");

        //Server 2
        var multipleServerService2 = new ItemIntegrationService();

        service.SaveItemForDistributedSystems("a");
        service.SaveItemForDistributedSystems("b");
        service.SaveItemForDistributedSystems("c");

 

        service.SaveItemForDistributedSystems("b");
        service.SaveItemForDistributedSystems("d");
        service.SaveItemForDistributedSystems("e");



        Console.WriteLine("Everything recorded Server 2:");

        multipleServerService.GetAllItemsForDistributedSystems().ForEach(Console.WriteLine);
        multipleServerService.GetAllItemsForDistributedSystems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}