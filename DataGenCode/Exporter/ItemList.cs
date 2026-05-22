using System.Text.Json.Serialization;

namespace DataGen.DataGenCode.Exporter;

public class ItemList
{

    [JsonInclude] [JsonPropertyName("cards")]
    public List<CardExport> Cards = [];

    [JsonInclude] [JsonPropertyName("enchantments")]
    public List<EnchantmentExport> Enchantments = [];
    
    [JsonInclude] [JsonPropertyName("keywords")]
    public List<KeywordExport> Keywords = [];

    [JsonInclude] [JsonPropertyName("mod")]
    public ModExport? Mod;

    [JsonInclude] [JsonPropertyName("potions")]
    public List<PotionExport> Potions = [];

    [JsonInclude] [JsonPropertyName("relics")]
    public List<RelicExport> Relics = [];

    public ItemList()
    {
    }

    public ItemList(ModExport mod) : this()
    {
        Mod = mod;
    }

    public void Add(ItemExport item)
    {
        switch (item)
        {
            case CardExport c:
                Cards.Add(c);
                break;
            case RelicExport r:
                Relics.Add(r);
                break;
            case PotionExport p:
                Potions.Add(p);
                break;
            case KeywordExport k:
                Keywords.Add(k);
                break;
            case EnchantmentExport ench:
                Enchantments.Add(ench);
                break;
        }
    }

    public void RemoveIf(Func<ItemExport, bool> predicate)
    {
        Func<ItemExport, bool> p = c => !predicate(c);
        Cards = [..Cards.Where(p).Cast<CardExport>()];
        Relics = [..Relics.Where(p).Cast<RelicExport>()];
        Potions = [..Potions.Where(p).Cast<PotionExport>()];
        Keywords = [..Keywords.Where(p).Cast<KeywordExport>()];
        Enchantments = [..Enchantments.Where(p).Cast<EnchantmentExport>()];
    }

    public void FindAll()
    {
        Cards.AddRange(CardExport.FindAll());
        Relics.AddRange(RelicExport.FindAll());
        Potions.AddRange(PotionExport.FindAll());
        Keywords.AddRange(KeywordExport.FindAll());
        Enchantments.AddRange(EnchantmentExport.FindAll());
    }

    public List<ItemExport> All()
    {
        return [..Cards, ..Relics, ..Potions, ..Keywords, ..Enchantments];
    }
}