using Integration.Service;

public abstract class Server2
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 1"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 2"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 3"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 4"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 5"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("Item Content 6"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}