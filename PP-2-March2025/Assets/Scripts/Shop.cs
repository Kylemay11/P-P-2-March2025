
using System.Collections.Generic;
using UnityEngine;



public class Shop : MonoBehaviour
{
    [Range(1, 1000)][SerializeField] int ItemPrice;
    public class Item
    {
        public string Name;
        public ItemType itemType;
    }

    public enum ItemType
    {
        Health,
        Ammo,
        Weapons,
    }

    private List<Item> itemsForSale = new List<Item>();

    void Start()
    {
        itemsForSale.Add(new Item { Name = "Small Health Pack", itemType = ItemType.Health });
        itemsForSale.Add(new Item { Name = "Health Pack", itemType = ItemType.Health });
        itemsForSale.Add(new Item { Name = "Large Health Pack", itemType = ItemType.Health });


        itemsForSale.Add(new Item { Name = "Spare Pistol Ammo",  itemType = ItemType.Ammo });
        itemsForSale.Add(new Item { Name = "Spare SMG Ammo", itemType = ItemType.Ammo });
        itemsForSale.Add(new Item { Name = "Spare Shotgun Shells", itemType = ItemType.Ammo });
        itemsForSale.Add(new Item { Name = "Spare SPECIAL Ammo", itemType = ItemType.Ammo });


        itemsForSale.Add(new Item { Name = "SMG",  itemType = ItemType.Weapons });
        itemsForSale.Add(new Item { Name = "ShotGun", itemType = ItemType.Weapons });
        itemsForSale.Add(new Item { Name = "B.F.G", itemType = ItemType.Weapons });
        itemsForSale.Add(new Item { Name = "R.Y.N.O", itemType = ItemType.Weapons });
        itemsForSale.Add(new Item { Name = "Wood Bat", itemType = ItemType.Weapons });
    }

    

}
