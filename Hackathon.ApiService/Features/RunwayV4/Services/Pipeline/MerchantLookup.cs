using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IMerchantLookup
{
    MerchantMatch? Match(string normDesc);
}

public class MerchantLookup : IMerchantLookup
{
    private static readonly List<(string Pattern, string MerchantName, CategoryKey Category)> Rules =
    [
        // Food & Dining
        ("JOLLIBEE", "Jollibee", CategoryKey.FoodDining),
        ("MCDONALDS", "McDonald's", CategoryKey.FoodDining),
        ("MCDONALD", "McDonald's", CategoryKey.FoodDining),
        ("GRAB FOOD", "GrabFood", CategoryKey.FoodDining),
        ("GRABFOOD", "GrabFood", CategoryKey.FoodDining),
        ("FOODPANDA", "Foodpanda", CategoryKey.FoodDining),
        ("CHOWKING", "Chowking", CategoryKey.FoodDining),
        ("KFC", "KFC", CategoryKey.FoodDining),
        ("STARBUCKS", "Starbucks", CategoryKey.FoodDining),
        ("SBUX", "Starbucks", CategoryKey.FoodDining),
        ("GREENWICH", "Greenwich", CategoryKey.FoodDining),
        ("YELLOW CAB", "Yellow Cab", CategoryKey.FoodDining),
        ("SHAKEYS", "Shakey's", CategoryKey.FoodDining),
        ("PIZZA HUT", "Pizza Hut", CategoryKey.FoodDining),
        ("BONCHON", "Bonchon", CategoryKey.FoodDining),
        ("ARMY NAVY", "Army Navy", CategoryKey.FoodDining),
        ("TIM HORTONS", "Tim Hortons", CategoryKey.FoodDining),
        ("DUNKIN", "Dunkin'", CategoryKey.FoodDining),

        // Groceries
        ("SM SUPERMARKET", "SM Supermarket", CategoryKey.Groceries),
        ("SM MARKET", "SM Supermarket", CategoryKey.Groceries),
        ("ROBINSONS SUPERMARKET", "Robinsons Supermarket", CategoryKey.Groceries),
        ("PUREGOLD", "Puregold", CategoryKey.Groceries),
        ("LANDERS", "Landers", CategoryKey.Groceries),
        ("S&R", "S&R", CategoryKey.Groceries),
        ("METRO SUPERMARKET", "Metro Supermarket", CategoryKey.Groceries),
        ("WALTERMART", "Walter Mart", CategoryKey.Groceries),
        ("SHOPWISE", "Shopwise", CategoryKey.Groceries),
        ("LANDMARK", "Landmark", CategoryKey.Groceries),
        ("EVER SUPERMARKET", "Ever Supermarket", CategoryKey.Groceries),
        ("7 ELEVEN", "7-Eleven", CategoryKey.Groceries),
        ("7-ELEVEN", "7-Eleven", CategoryKey.Groceries),
        ("MINISTOP", "Ministop", CategoryKey.Groceries),
        ("FAMILYMART", "FamilyMart", CategoryKey.Groceries),
        ("LAWSON", "Lawson", CategoryKey.Groceries),

        // Transport
        ("GRAB", "Grab", CategoryKey.Transport),
        ("ANGKAS", "Angkas", CategoryKey.Transport),
        ("BEEP", "Beep Card", CategoryKey.Transport),
        ("AUTOSWEEP", "Autosweep", CategoryKey.Transport),
        ("EASYTRIP", "Easytrip", CategoryKey.Transport),
        ("SHELL", "Shell", CategoryKey.Transport),
        ("PETRON", "Petron", CategoryKey.Transport),
        ("CALTEX", "Caltex", CategoryKey.Transport),
        ("JOYRIDE", "JoyRide", CategoryKey.Transport),

        // Shopping
        ("LAZADA", "Lazada", CategoryKey.Shopping),
        ("SHOPEE", "Shopee", CategoryKey.Shopping),
        ("ZALORA", "Zalora", CategoryKey.Shopping),
        ("SM STORE", "SM Store", CategoryKey.Shopping),
        ("UNIQLO", "Uniqlo", CategoryKey.Shopping),
        ("BENCH", "Bench", CategoryKey.Shopping),
        ("NIKE", "Nike", CategoryKey.Shopping),
        ("ADIDAS", "Adidas", CategoryKey.Shopping),
        ("SHEIN", "Shein", CategoryKey.Shopping),
        ("TIKTOK SHOP", "TikTok Shop", CategoryKey.Shopping),

        // Bills & Utilities
        ("MERALCO", "Meralco", CategoryKey.BillsUtilities),
        ("MANILA WATER", "Manila Water", CategoryKey.BillsUtilities),
        ("MAYNILAD", "Maynilad", CategoryKey.BillsUtilities),
        ("PLDT", "PLDT", CategoryKey.BillsUtilities),
        ("GLOBE", "Globe", CategoryKey.BillsUtilities),
        ("SMART", "Smart", CategoryKey.BillsUtilities),
        ("CONVERGE", "Converge", CategoryKey.BillsUtilities),
        ("SKY CABLE", "Sky Cable", CategoryKey.BillsUtilities),
        ("CIGNAL", "Cignal", CategoryKey.BillsUtilities),

        // Health & Wellness
        ("MERCURY DRUG", "Mercury Drug", CategoryKey.HealthWellness),
        ("WATSONS", "Watsons", CategoryKey.HealthWellness),
        ("SOUTH STAR DRUG", "South Star Drug", CategoryKey.HealthWellness),
        ("GENERIKA", "Generika", CategoryKey.HealthWellness),
        ("ANYTIME FITNESS", "Anytime Fitness", CategoryKey.HealthWellness),
        ("GOLD GYM", "Gold's Gym", CategoryKey.HealthWellness),

        // Entertainment & Subscriptions
        ("NETFLIX", "Netflix", CategoryKey.EntertainmentSubs),
        ("SPOTIFY", "Spotify", CategoryKey.EntertainmentSubs),
        ("YOUTUBE", "YouTube Premium", CategoryKey.EntertainmentSubs),
        ("DISNEY", "Disney+", CategoryKey.EntertainmentSubs),
        ("HBO", "HBO Go", CategoryKey.EntertainmentSubs),
        ("APPLE", "Apple", CategoryKey.EntertainmentSubs),
        ("GOOGLE PLAY", "Google Play", CategoryKey.EntertainmentSubs),
        ("STEAM", "Steam", CategoryKey.EntertainmentSubs),
        ("SM CINEMA", "SM Cinema", CategoryKey.EntertainmentSubs),

        // Transfers
        ("INSTAPAY", "InstaPay Transfer", CategoryKey.Transfers),
        ("PESONET", "PESONet Transfer", CategoryKey.Transfers),
        ("FUND TRANSFER", "Fund Transfer", CategoryKey.Transfers),
        ("ONLINE TRANSFER", "Online Transfer", CategoryKey.Transfers),

        // Housing
        ("CONDO", "Condo Dues", CategoryKey.Housing),
        ("RENT", "Rent", CategoryKey.Housing),
        ("HOMEOWNERS", "HOA Dues", CategoryKey.Housing),
        ("HOA", "HOA Dues", CategoryKey.Housing),
    ];

    public MerchantMatch? Match(string normDesc)
    {
        if (string.IsNullOrWhiteSpace(normDesc))
            return null;

        foreach (var (pattern, merchantName, category) in Rules)
        {
            if (normDesc.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return new MerchantMatch
                {
                    Category = category,
                    MerchantName = merchantName,
                    Confidence = "high",
                };
            }
        }

        return null;
    }
}
