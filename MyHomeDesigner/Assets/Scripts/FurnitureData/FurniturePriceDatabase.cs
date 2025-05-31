using System.Collections.Generic;
using UnityEngine;
public static class FurniturePriceDatabase
{
    public static Dictionary<string, string> dictDoors = new Dictionary<string, string>
    {
        { "Door1", "30€" }
    };

    public static Dictionary<string, string> dictWindows = new Dictionary<string, string>
    {
        { "Window1", "200€" }
    };
    public static Dictionary<string, string> dictKitchen = new Dictionary<string, string>
    {
        { "coffee_machine_001", "200€" },
        { "fridge_001", "400€" },
        { "kitchen_chair_001", "50€" },
        { "kitchen_sink_001", "400€" },
        { "kitchen_table_001", "2000€" },
        { "microwave_oven_001", "100€" }
    };

    public static Dictionary<string, string> dictLivingRoom = new Dictionary<string, string>
    {
        { "air_hockey_001", "350€" },
        { "coffee_table_001", "50€" },
        { "lamp_002", "30€" },
        { "lounge_chair_001", "60€" },
        { "musical_instrument_001", "2000€" },
        { "office_table_001", "250€" },
        { "scratching_post_001", "20€" },
        { "sofa_001", "1300€" },
        { "tv_wall_001", "3500€" }
    };
    
    public static Dictionary<string, string> dictBedroom = new Dictionary<string, string>
    {
        { "bed_001", "300€" },
        { "closet_001", "270€" },
        { "closet_002", "90€" },
        { "dresser_001", "70€" },
    };

    public static Dictionary<string, string> dictBathroom = new Dictionary<string, string>
    {
        { "bathroom_item_001", "15€" },
        { "Dulap", "20€" },
        { "washing_machine_001", "310€" }
    };

    public static Dictionary<string, string> dictMiscellaneos = new Dictionary<string, string>
    {
        
    };

    public static string GetPrice(string category, string itemName)
    {
        Dictionary<string, string> dict = category switch
        {
            "Doors" => dictDoors,
            "Windows" => dictWindows,
            "Kitchen" => dictKitchen,
            "LivingRoom" => dictLivingRoom,
            "Bedroom" => dictBedroom,
            "Bathroom" => dictBathroom,
            "Miscellaneos" => dictMiscellaneos,
            _ => null
        };

        if (dict != null && dict.TryGetValue(itemName, out string price))
            return price;

        return "0€";
    }
}
