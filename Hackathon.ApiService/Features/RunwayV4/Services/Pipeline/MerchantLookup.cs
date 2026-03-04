using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IMerchantLookup
{
    MerchantMatch? Match(string normDesc);
}

public class MerchantLookup : IMerchantLookup
{
<<<<<<< Updated upstream
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
=======
    private static readonly (string Pattern, string Merchant, CategoryKey Category)[] SeedList;

    static MerchantLookup()
    {
        var entries = new List<(string Pattern, string Merchant, CategoryKey Category)>
        {
            // ── Food & Dining (Discretionary) ─────────────────────────────
            ("GRAB*FOOD", "Grab Food", CategoryKey.FoodDining),
            ("GRABFOOD", "Grab Food", CategoryKey.FoodDining),
            ("GRAB FOOD", "Grab Food", CategoryKey.FoodDining),
            ("FOODPANDA", "foodpanda", CategoryKey.FoodDining),
            ("MCDONALDS", "McDonald's", CategoryKey.FoodDining),
            ("MC DONALDS", "McDonald's", CategoryKey.FoodDining),
            ("JOLLIBEE", "Jollibee", CategoryKey.FoodDining),
            ("CHOWKING", "Chowking", CategoryKey.FoodDining),
            ("MANG INASAL", "Mang Inasal", CategoryKey.FoodDining),
            ("GREENWICH", "Greenwich", CategoryKey.FoodDining),
            ("RED RIBBON", "Red Ribbon", CategoryKey.FoodDining),
            ("STARBUCKS", "Starbucks", CategoryKey.FoodDining),
            ("BOS COFFEE", "Bo's Coffee", CategoryKey.FoodDining),
            ("SHAKEYS", "Shakey's", CategoryKey.FoodDining),
            ("PIZZA HUT", "Pizza Hut", CategoryKey.FoodDining),
            ("DOMINOS", "Domino's", CategoryKey.FoodDining),
            ("ARMY NAVY", "Army Navy", CategoryKey.FoodDining),
            ("ZARKS", "Zark's Burgers", CategoryKey.FoodDining),
            ("BONCHON", "BonChon", CategoryKey.FoodDining),
            ("MAX RESTAURANT", "Max's Restaurant", CategoryKey.FoodDining),
            ("YELLOW CAB", "Yellow Cab", CategoryKey.FoodDining),
            ("SWEET CORN", "Sweet Corn", CategoryKey.FoodDining),
            ("TROPICAL HUT", "Tropical Hut", CategoryKey.FoodDining),
            ("GOLDILOCKS", "Goldilocks", CategoryKey.FoodDining),
            ("ANGELS PIZZA", "Angel's Pizza", CategoryKey.FoodDining),
            ("SBARRO", "Sbarro", CategoryKey.FoodDining),
            ("TOKYO TOKYO", "Tokyo Tokyo", CategoryKey.FoodDining),
            ("PANCAKE HOUSE", "Pancake House", CategoryKey.FoodDining),
            ("COUNTRY STYLE", "Country Style", CategoryKey.FoodDining),
            ("KFC", "KFC", CategoryKey.FoodDining),

            // ── Groceries & Market (Essential) ────────────────────────────
            ("ROBINSONS SUPERMARKET", "Robinsons Supermarket", CategoryKey.Groceries),
            ("SM SUPERMARKET", "SM Supermarket", CategoryKey.Groceries),
            ("SM HYPERMARKET", "SM Hypermarket", CategoryKey.Groceries),
            ("PUREGOLD", "Puregold", CategoryKey.Groceries),
            ("S&R", "S&R Membership Shopping", CategoryKey.Groceries),
            ("SNR", "S&R Membership Shopping", CategoryKey.Groceries),
            ("WALTERMART", "WalterMart", CategoryKey.Groceries),
            ("METRO GAISANO", "Metro Gaisano", CategoryKey.Groceries),
            ("SHOPWISE", "Shopwise", CategoryKey.Groceries),
            ("RUSTANS SUPERMARKET", "Rustan's Supermarket", CategoryKey.Groceries),
            ("LANDERS", "Landers Superstore", CategoryKey.Groceries),
            ("ALLDAY SUPERMARKET", "AllDay Supermarket", CategoryKey.Groceries),
            ("7-ELEVEN", "7-Eleven", CategoryKey.Groceries),
            ("7 ELEVEN", "7-Eleven", CategoryKey.Groceries),
            ("7ELEVEN", "7-Eleven", CategoryKey.Groceries),
            ("MINISTOP", "Ministop", CategoryKey.Groceries),
            ("FAMILYMART", "FamilyMart", CategoryKey.Groceries),
            ("FAMILY MART", "FamilyMart", CategoryKey.Groceries),
            ("ALFAMART", "Alfamart", CategoryKey.Groceries),
            ("THE LANDMARK FOOD", "The Landmark Supermarket", CategoryKey.Groceries),
            ("LANDMARK SUPERMARKET", "The Landmark Supermarket", CategoryKey.Groceries),
            ("MERKADO", "Merkado", CategoryKey.Groceries),
            ("LINIS", "Linis.ph", CategoryKey.Groceries),
            ("GRAB MART", "GrabMart", CategoryKey.Groceries),
            ("GRABMART", "GrabMart", CategoryKey.Groceries),
            ("PANDAMART", "PandaMart", CategoryKey.Groceries),
            ("LAZMART", "LazMart", CategoryKey.Groceries),
            ("HONEST BEE", "honestbee", CategoryKey.Groceries),

            // ── Bills & Utilities (Essential) ─────────────────────────────
            ("MERALCO", "Meralco", CategoryKey.BillsUtilities),
            ("MECO", "Meralco", CategoryKey.BillsUtilities),
            ("MANILA WATER", "Manila Water", CategoryKey.BillsUtilities),
            ("MAYNILAD", "Maynilad", CategoryKey.BillsUtilities),
            ("GLOBE TELECOM", "Globe", CategoryKey.BillsUtilities),
            ("GLOBE*", "Globe", CategoryKey.BillsUtilities),
            ("SMART COMMUNICATIONS", "Smart", CategoryKey.BillsUtilities),
            ("SMART*", "Smart", CategoryKey.BillsUtilities),
            ("DITO TELECOMMUNITY", "Dito", CategoryKey.BillsUtilities),
            ("PLDT", "PLDT", CategoryKey.BillsUtilities),
            ("CONVERGE", "Converge ICT", CategoryKey.BillsUtilities),
            ("SKYCABLE", "Sky Cable", CategoryKey.BillsUtilities),
            ("SKY CABLE", "Sky Cable", CategoryKey.BillsUtilities),
            ("CIGNAL", "Cignal", CategoryKey.BillsUtilities),
            ("NETFLIX", "Netflix", CategoryKey.BillsUtilities),
            ("SPOTIFY", "Spotify", CategoryKey.BillsUtilities),
            ("YOUTUBE PREMIUM", "YouTube Premium", CategoryKey.BillsUtilities),
            ("GOOGLE*", "Google", CategoryKey.BillsUtilities),
            ("APPLE.COM/BILL", "Apple Subscriptions", CategoryKey.BillsUtilities),
            ("BAYAD CENTER", "Bayad Center", CategoryKey.BillsUtilities),
            ("BAYAD*", "Bayad Center", CategoryKey.BillsUtilities),
            ("ECPAY", "ECPay", CategoryKey.BillsUtilities),
            ("PAYNAMICS", "Paynamics", CategoryKey.BillsUtilities),
            ("TRUEMONEY", "TrueMoney", CategoryKey.BillsUtilities),

            // ── Transport (Essential) ─────────────────────────────────────
            ("GRAB*TRANS", "Grab Car", CategoryKey.Transport),
            ("GRAB CAR", "Grab Car", CategoryKey.Transport),
            ("GRABCAR", "Grab Car", CategoryKey.Transport),
            ("GRAB*EXPRESS", "Grab Express", CategoryKey.Transport),
            ("GRAB BIKE", "Grab Bike", CategoryKey.Transport),
            ("ANGKAS", "Angkas", CategoryKey.Transport),
            ("MOVE IT", "Move It", CategoryKey.Transport),
            ("MOVEIT", "Move It", CategoryKey.Transport),
            ("AF PAYMENTS", "Beep Card (MRT/LRT)", CategoryKey.Transport),
            ("BEEP", "Beep Card", CategoryKey.Transport),
            ("PETRON", "Petron", CategoryKey.Transport),
            ("SHELL", "Shell", CategoryKey.Transport),
            ("CALTEX", "Caltex", CategoryKey.Transport),
            ("PHOENIX PETROLEUM", "Phoenix Fuels", CategoryKey.Transport),
            ("SEAOIL", "Seaoil", CategoryKey.Transport),
            ("JOYRIDE", "JoyRide", CategoryKey.Transport),
            ("OWTO", "Owto", CategoryKey.Transport),
            ("HYPE", "Hype", CategoryKey.Transport),
            ("LTFRB", "LTFRB", CategoryKey.Transport),
            ("TOLL", "Toll Fee", CategoryKey.Transport),
            ("EASYTRIP", "EasyTrip", CategoryKey.Transport),
            ("AUTOSWEEP", "AutoSweep", CategoryKey.Transport),

            // ── Shopping (Discretionary) ──────────────────────────────────
            ("LAZADA", "Lazada", CategoryKey.Shopping),
            ("SHOPEE", "Shopee", CategoryKey.Shopping),
            ("ZALORA", "Zalora", CategoryKey.Shopping),
            ("SHEIN", "Shein", CategoryKey.Shopping),
            ("TEMU", "Temu", CategoryKey.Shopping),
            ("SM DEPARTMENT", "SM Store (Dept)", CategoryKey.Shopping),
            ("SM STORE", "SM Store (Dept)", CategoryKey.Shopping),
            ("ROBINSONS DEPT", "Robinsons Dept Store", CategoryKey.Shopping),
            ("THE LANDMARK", "The Landmark", CategoryKey.Shopping),
            ("H&M", "H&M", CategoryKey.Shopping),
            ("ZARA", "Zara", CategoryKey.Shopping),
            ("UNIQLO", "Uniqlo", CategoryKey.Shopping),
            ("NATIONAL BOOK STORE", "National Book Store", CategoryKey.Shopping),
            ("NBS", "National Book Store", CategoryKey.Shopping),
            ("FULLY BOOKED", "Fully Booked", CategoryKey.Shopping),
            ("TRUE VALUE", "True Value", CategoryKey.Shopping),
            ("ACE HARDWARE", "Ace Hardware", CategoryKey.Shopping),
            ("HANDYMAN", "Handyman", CategoryKey.Shopping),
            ("ISTORE", "iStore", CategoryKey.Shopping),
            ("BEYOND THE BOX", "Beyond the Box", CategoryKey.Shopping),
            ("SILICON VALLEY", "Silicon Valley", CategoryKey.Shopping),
            ("ABENSON", "Abenson", CategoryKey.Shopping),
            ("CD-R KING", "CD-R King", CategoryKey.Shopping),
            ("MEMORIZE", "Memo Xpress", CategoryKey.Shopping),
            ("MEMO EXPRESS", "Memo Xpress", CategoryKey.Shopping),

            // ── Health & Wellness (Essential) ─────────────────────────────
            ("MERCURY DRUG", "Mercury Drug", CategoryKey.HealthWellness),
            ("MERCURYDRUG", "Mercury Drug", CategoryKey.HealthWellness),
            ("WATSONS", "Watsons", CategoryKey.HealthWellness),
            ("ROSE PHARMACY", "Rose Pharmacy", CategoryKey.HealthWellness),
            ("GENERIKA", "Generika", CategoryKey.HealthWellness),
            ("SOUTHSTAR DRUG", "Southstar Drug", CategoryKey.HealthWellness),
            ("THE GENERICS PHARMACY", "The Generics Pharmacy", CategoryKey.HealthWellness),
            ("TGP", "The Generics Pharmacy", CategoryKey.HealthWellness),
            ("ST LUKES", "St. Luke's Medical", CategoryKey.HealthWellness),
            ("SAINT LUKES", "St. Luke's Medical", CategoryKey.HealthWellness),
            ("MAKATI MEDICAL", "Makati Medical Center", CategoryKey.HealthWellness),
            ("MAKATI MED", "Makati Medical Center", CategoryKey.HealthWellness),
            ("THE MEDICAL CITY", "The Medical City", CategoryKey.HealthWellness),
            ("MEDICAL CITY", "The Medical City", CategoryKey.HealthWellness),
            ("ASIAN HOSPITAL", "Asian Hospital", CategoryKey.HealthWellness),
            ("MAXICARE", "Maxicare", CategoryKey.HealthWellness),
            ("PHILCARE", "PhilCare", CategoryKey.HealthWellness),
            ("MEDICARD", "MediCard", CategoryKey.HealthWellness),
            ("INTELLICARE", "Intellicare", CategoryKey.HealthWellness),
            ("ANYTIME FITNESS", "Anytime Fitness", CategoryKey.HealthWellness),
            ("FITNESS FIRST", "Fitness First", CategoryKey.HealthWellness),
            ("GOLDS GYM", "Gold's Gym", CategoryKey.HealthWellness),
            ("F45", "F45 Training", CategoryKey.HealthWellness),
            ("CULT FITNESS", "Cult.fit", CategoryKey.HealthWellness),
            ("SNAP FITNESS", "Snap Fitness", CategoryKey.HealthWellness),
            ("SLIMMERS WORLD", "Slimmer's World", CategoryKey.HealthWellness),

            // ── Housing (Essential) ───────────────────────────────────────
            ("AYALA PROPERTY", "Ayala Land", CategoryKey.Housing),
            ("SMDC", "SMDC", CategoryKey.Housing),
            ("MEGAWORLD", "Megaworld", CategoryKey.Housing),
            ("ROBINSONS LAND", "Robinsons Land", CategoryKey.Housing),
            ("DMCI HOMES", "DMCI Homes", CategoryKey.Housing),
            ("DMCI", "DMCI Homes", CategoryKey.Housing),
            ("VISTA LAND", "Vista Land", CategoryKey.Housing),
            ("FED LAND", "Federal Land", CategoryKey.Housing),
            ("FEDERAL LAND", "Federal Land", CategoryKey.Housing),
            ("CONDO DUES", "Condo Association Dues", CategoryKey.Housing),
            ("ASSOCIATION DUES", "Association Dues", CategoryKey.Housing),
            ("HOA DUES", "HOA Dues", CategoryKey.Housing),
            ("RENTAL PAYMENT", "Rent", CategoryKey.Housing),
            ("RENT", "Rent", CategoryKey.Housing),

            // ── Transfers & Remittances (Committed) ───────────────────────
            ("GCASH SEND MONEY", "GCash Transfer", CategoryKey.Transfers),
            ("GCASH TRANSFER", "GCash Transfer", CategoryKey.Transfers),
            ("SEND MONEY", "GCash Transfer", CategoryKey.Transfers),
            ("INSTAPAY", "InstaPay Transfer", CategoryKey.Transfers),
            ("PESONET", "PESONet Transfer", CategoryKey.Transfers),
            ("PALAWAN EXPRESS", "Palawan Express", CategoryKey.Transfers),
            ("PALAWAN PAWNSHOP", "Palawan Pawnshop", CategoryKey.Transfers),
            ("LBC EXPRESS", "LBC Express", CategoryKey.Transfers),
            ("WESTERN UNION", "Western Union", CategoryKey.Transfers),
            ("MONEYGRAM", "MoneyGram", CategoryKey.Transfers),
            ("REMITLY", "Remitly", CategoryKey.Transfers),
            ("M LHUILLIER", "M Lhuillier", CategoryKey.Transfers),
            ("MLHUILLIER", "M Lhuillier", CategoryKey.Transfers),
            ("CEBUANA LHUILLIER", "Cebuana Lhuillier", CategoryKey.Transfers),
            ("CEBUANA", "Cebuana Lhuillier", CategoryKey.Transfers),
            ("COINS PH", "Coins.ph", CategoryKey.Transfers),
            ("WIRE TRANSFER", "Wire Transfer", CategoryKey.Transfers),
            ("SWIFT", "SWIFT Transfer", CategoryKey.Transfers),

            // ── Entertainment & Subscriptions (Discretionary) ─────────────
            ("DISNEY PLUS", "Disney+", CategoryKey.EntertainmentSubs),
            ("DISNEYPLUS", "Disney+", CategoryKey.EntertainmentSubs),
            ("DISNEY+", "Disney+", CategoryKey.EntertainmentSubs),
            ("HBO GO", "HBO Go", CategoryKey.EntertainmentSubs),
            ("VIVAMAX", "Vivamax", CategoryKey.EntertainmentSubs),
            ("IQIYI", "iQIYI", CategoryKey.EntertainmentSubs),
            ("AMAZON PRIME", "Amazon Prime", CategoryKey.EntertainmentSubs),
            ("PRIME VIDEO", "Amazon Prime", CategoryKey.EntertainmentSubs),
            ("STEAM", "Steam", CategoryKey.EntertainmentSubs),
            ("PLAYSTATION STORE", "PlayStation Store", CategoryKey.EntertainmentSubs),
            ("PS STORE", "PlayStation Store", CategoryKey.EntertainmentSubs),
            ("XBOX", "Xbox Game Pass", CategoryKey.EntertainmentSubs),
            ("NINTENDO", "Nintendo eShop", CategoryKey.EntertainmentSubs),
            ("MOBILE LEGENDS", "Mobile Legends", CategoryKey.EntertainmentSubs),
            ("MOONTON", "Mobile Legends", CategoryKey.EntertainmentSubs),
            ("SUPERCELL", "Clash of Clans", CategoryKey.EntertainmentSubs),
            ("GARENA", "Garena (Free Fire)", CategoryKey.EntertainmentSubs),
            ("SM CINEMA", "SM Cinema", CategoryKey.EntertainmentSubs),
            ("SM CINEMAS", "SM Cinema", CategoryKey.EntertainmentSubs),
            ("AYALA CINEMAS", "Ayala Malls Cinema", CategoryKey.EntertainmentSubs),
            ("TICKETNET", "TicketNet", CategoryKey.EntertainmentSubs),
            ("SMTICKETS", "SM Tickets", CategoryKey.EntertainmentSubs),
            ("TICKET2ME", "Ticket2Me", CategoryKey.EntertainmentSubs),
            ("KLOOK", "Klook", CategoryKey.EntertainmentSubs),
            ("AGODA", "Agoda", CategoryKey.EntertainmentSubs),
            ("CANVA", "Canva", CategoryKey.EntertainmentSubs),
            ("ADOBE", "Adobe", CategoryKey.EntertainmentSubs),
            ("MAX", "Max (HBO)", CategoryKey.EntertainmentSubs),

            // ── Government Deductions (Committed) ───────────────────────────
            ("SSS CONTRIBUTION", "SSS", CategoryKey.GovernmentDeductions),
            ("SSS PAYMENT", "SSS", CategoryKey.GovernmentDeductions),
            ("SSS", "SSS", CategoryKey.GovernmentDeductions),
            ("PHILHEALTH", "PhilHealth", CategoryKey.GovernmentDeductions),
            ("PHIL HEALTH", "PhilHealth", CategoryKey.GovernmentDeductions),
            ("PAG-IBIG", "Pag-IBIG", CategoryKey.GovernmentDeductions),
            ("PAGIBIG", "Pag-IBIG", CategoryKey.GovernmentDeductions),
            ("PAG IBIG", "Pag-IBIG", CategoryKey.GovernmentDeductions),
            ("HDMF", "Pag-IBIG", CategoryKey.GovernmentDeductions),
            ("BIR", "BIR", CategoryKey.GovernmentDeductions),
            ("WITHHOLDING TAX", "Withholding Tax", CategoryKey.GovernmentDeductions),

            // ── Miscellaneous (Catch-all) ─────────────────────────────────
            ("ATM", "ATM Withdrawal", CategoryKey.Misc),
            ("WITHDRAWAL", "Cash Withdrawal", CategoryKey.Misc),
            ("POS PURCHASE", "POS Purchase", CategoryKey.Misc),
            ("DEBIT PURCHASE", "Debit Purchase", CategoryKey.Misc),
            ("ONLINE PURCHASE", "Online Purchase", CategoryKey.Misc),
            ("PAYMENT", "Payment", CategoryKey.Misc),
            ("GCASH QR", "GCash QR Payment", CategoryKey.Misc),
        };

        // Sort by pattern length descending — longest match wins
        SeedList = entries.OrderByDescending(e => e.Pattern.Length).ToArray();
    }

    public MerchantMatch? Match(string normDesc)
    {
        if (string.IsNullOrWhiteSpace(normDesc)) return null;

        var upper = normDesc.ToUpperInvariant();

        foreach (var (pattern, merchant, category) in SeedList)
        {
            // Handle wildcard patterns (e.g. "GRAB*FOOD" matches "GRAB WHATEVER FOOD")
            if (pattern.Contains('*'))
            {
                var parts = pattern.Split('*', 2);
                int startIdx = upper.IndexOf(parts[0], StringComparison.OrdinalIgnoreCase);
                if (startIdx >= 0)
                {
                    int searchFrom = startIdx + parts[0].Length;
                    if (parts[1].Length == 0 || upper.IndexOf(parts[1], searchFrom, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return new MerchantMatch { Category = category, MerchantName = merchant };
                    }
                }
            }
            else
            {
                if (upper.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return new MerchantMatch { Category = category, MerchantName = merchant };
                }
>>>>>>> Stashed changes
            }
        }

        return null;
    }
}
