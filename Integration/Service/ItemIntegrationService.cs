using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

    // This is called externally and can be called multithreaded, in parallel.
    // More than one item with the same content should not be saved. However,
    // calling this with different contents at the same time is OK, and should
    // be allowed for performance reasons.
    private static readonly ConcurrentDictionary<string, object> InProgressSaves = new();

    public Result SaveItem(string itemContent)
    {
        // Check the backend to see if the content is already saved.
        if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
        {
            return new Result(false, $"Duplicate item received with content {itemContent}.");
        }

        var item = ItemIntegrationBackend.SaveItem(itemContent);

        return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
    }

    public void SaveDiffirentItems(string itemContent)
    {
        // Lock object for the specific content to ensure thread safety
        var lockObject = InProgressSaves.GetOrAdd(itemContent, new object());

        lock (lockObject)
        {
            try
            { 
                SaveItem(itemContent);
            }
            finally
            {
            // Clean up the lock object if no other threads are waiting on it
                InProgressSaves.TryRemove(itemContent, out _);
            }
        }
    }
    public Result SaveItemForDistributedSystems(string itemContent)
    {
        // Check the backend to see if the content is already saved.
        if (ItemIntegrationBackend.FindItemsWithContentDistributedSystems(itemContent).Count != 0)
        {
            return new Result(false, $"Duplicate item received with content {itemContent}.");
        }

        var item = ItemIntegrationBackend.SaveItemForDistributedSystems(itemContent);

        return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
    public List<Item> GetAllItemsForDistributedSystems()
    {
        var items = new List<Item>();

        if (!File.Exists(ItemOperationBackend.filePath))
        {
            return items;
        }

        var fileContent = File.ReadAllText(ItemOperationBackend.filePath);

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

            items.Add(item);
        }

        return items;
    }
}