
/* Model for an order */
public class Order
{
    public Order()
    {
        this.OrderId = Random.Shared.Next(1000, 9999);
        this.CustomerName = "Default Customer";
        this.DatePlaced = DateTime.Now;
        this.Items = new List<InventoryItem>();
    }

    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime DatePlaced { get; set; }
    public List<InventoryItem> Items { get; set; }

    public void AddItem(InventoryItem item) {
        Items.Add(item);
    }

    public void RemoveItem(int itemId) {
        Items.RemoveAll(i => i.ItemId == itemId);
    }

    public string GetOrderSummary() {
        return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced.ToShortDateString()}";
    }
}