using System.Collections.Concurrent;
using System.Text.Json;
using Integration.Common;

namespace Integration.Backend;

public sealed class ItemOperationBackend
{
    private ConcurrentBag<Item> SavedItems { get; set; } = new();
    private int _identitySequence;
    public static readonly string filePath = "..\\..\\..\\SavedItems.txt";
    private static readonly object fileLock = new object();
    public Item SaveItem(string itemContent)
    {
        // This simulates how long it takes to save
        // the item content. Forty seconds, give or take.
        Thread.Sleep(2_000);
        
        var item = new Item();
        item.Content = itemContent;
        item.Id = GetNextIdentity();
        SavedItems.Add(item);
        return item;
    }
    public Item SaveItemForDistributedSystems(string itemContent)
    {
        // This simulates how long it takes to save
        // the item content. Forty seconds, give or take.
        Thread.Sleep(2_000);

        var item = new Item();
        item.Content = itemContent;
        item.Id = GetNextIdentity();

        string fileContent = $"Id: {item.Id}\nContent: {item.Content}\n---\n";

        lock (fileLock)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, fileContent);
            }
            else
            {
                File.AppendAllText(filePath, fileContent);
            }

        }
        return item;
    }
    public List<Item> FindItemsWithContent(string itemContent)
    {
        return SavedItems.Where(x => x.Content == itemContent).ToList();
    }
    public List<Item> FindItemsWithContentDistributedSystems(string itemContent)
    {
        var items = new List<Item>();

        if (!File.Exists(filePath))
        {
            return items;
        }

        var fileContent = File.ReadAllText(filePath);

        // Her öğeyi ayırmak için "---" ayırıcısını kullan
        var itemEntries = fileContent.Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var entry in itemEntries)
        {
            var lines = entry.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var item = new Item();

            foreach (var line in lines)
            {
                if (line.StartsWith("Id:"))
                {
                    item.Id = int.Parse(line.Substring(3).Trim());
                }
                else if (line.StartsWith("Content:"))
                {
                    item.Content = line.Substring(8).Trim();
                }
            }

            if (item.Content == itemContent)
            {
                items.Add(item);
            }
        }

        return items;
    }

    private int GetNextIdentity()
    {
        return Interlocked.Increment(ref _identitySequence);
    }

    public List<Item> GetAllItems()
    {
        return SavedItems.ToList();
    }
}