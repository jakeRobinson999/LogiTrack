public class InventoryItem {
    public InventoryItem()
    {
        ItemId = 0;
        Name = "Default Item";
        Quantity = 0;
        Location = "Default Location";
    }

    public int ItemId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }

    public void DisplayInfo() {
        Console.WriteLine($"Item: {this.Name} | Quantity: {this.Quantity} | Location: {this.Location}");
    } 

 }
