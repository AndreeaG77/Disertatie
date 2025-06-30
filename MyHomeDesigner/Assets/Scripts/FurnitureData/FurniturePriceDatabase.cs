using System.Collections.Generic;
using UnityEngine;
public static class FurniturePriceDatabase
{
    public static Dictionary<string, string> dictDoors = new Dictionary<string, string>
    {
        { "Door1", "30€" },
        { "Door2", "30€" },
        { "Door3", "55€" },
        { "Door4", "55€" },
        { "Door5", "25€" },
        { "Door6", "25€" },
        { "Door7", "60€" },
        { "Door8", "30€" },
        { "Door9", "55€" },
    };

    public static Dictionary<string, string> dictWindows = new Dictionary<string, string>
    {
        { "Window1", "200€" },
        { "Window2", "190€" },
        { "Window3", "210€" },
        { "Window4", "170€" },
        { "Window5", "150€" },
        { "Window6", "180€" },
        { "Window7", "175€" },
        { "Window8", "165€" },
        { "Window9", "150€" },
    };
    public static Dictionary<string, string> dictKitchen = new Dictionary<string, string>
    {
        { "coffee_machine_001", "200€" },
        { "fridge_001", "400€" },
        { "kitchen_chair_001", "50€" },
        { "kitchen_sink_001", "400€" },
        { "kitchen_table_001", "2000€" },
        { "microwave_oven_001", "100€" },
        { "Cabinet_Base_Corner_01", "555€" },
        { "Cabinet_Base_DD_01", "220€" },
        { "Cabinet_Base_SD_01", "175€" },
        { "Cabinet_Base_Sink_01", "410€" },
        { "Fridge_01", "650€" },
        { "Stove_01", "840€" }
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
        { "tv_wall_001", "3500€" },
        { "Shelf_Apt_01", "100€" },
        { "Chair_Apt_01", "50€" },
        { "Sofa_Apt_01", "500€" },
        { "Sofa_Apt_02", "520€" },
        { "Table_Coffee_01", "45€" },
        { "Table_Computer_01", "230€" },
        { "Table_Computer_01_Setup", "1950€" },
        { "Table_Dining_Apt_01", "50€" },
        { "Table_Dining_Apt_01_Setup Variant", "250€" },
        { "Table_Media_01", "50€" },
        { "Table_Side_Apt_01", "20€" }
    };
    
    public static Dictionary<string, string> dictBedroom = new Dictionary<string, string>
    {
        { "bed_001", "300€" },
        { "closet_001", "270€" },
        { "closet_002", "90€" },
        { "dresser_001", "70€" },
        { "Bed_Apt_01_01", "280€" },
        { "Bed_Apt_01_02", "220€" },
        { "Bed_Apt_02_01", "405€" },
        { "Dresser Closet_01", "170€" },
        { "Dresser_Apt_01", "190€" },
        { "Table_End_01", "60€" },
        { "Table_End_02", "35€" }
    };

    public static Dictionary<string, string> dictBathroom = new Dictionary<string, string>
    {
        { "bathroom_item_001", "15€" },
        { "washing_machine_001", "310€" },
        { "BathTub_Base_01", "430€"},
        { "BR_Vanity_01", "75€" },
        { "Hamper_01", "5€" },
        { "Shower_Base_01", "290€" },
        { "Toilet_Apt_01", "110€" },
        { "basien", "85€" },
        { "Bathroom_Props_Set02", "345€" },
        { "bathtub", "220€" },
        { "washbasine", "305€" }
    };

    public static Dictionary<string, string> dictMiscellaneous = new Dictionary<string, string>
    {
        { "camera_001", "300€" },
        { "dumbbell_001", "10€" },
        { "dumbbell_002", "20€" },
        { "flower_001", "5€" },
        { "lamp_001", "15€" },
        { "training_item_001", "105€" },
        { "training_item_002", "165€" }
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
            "Miscellaneous" => dictMiscellaneous,
            _ => null
        };

        if (dict != null && dict.TryGetValue(itemName, out string price))
            return price;

        return "0€";
    }
}
