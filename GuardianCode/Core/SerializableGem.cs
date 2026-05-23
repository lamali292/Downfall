using System.Text.Json;
using System.Text.Json.Serialization;
using Guardian.GuardianCode.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public class SerializableGem
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";


    public static SerializableGem FromGem(GemModel gem)
    {
        return new SerializableGem
        {
            Id = gem.Id.ToString()
        };
    }

    public GemModel ToGem(GuardianCardModel? owner = null)
    {
        var gem = ModelDb.GetById<GemModel>(ModelId.Deserialize(Id));
        if (owner != null)
            gem.Card = owner;
        return gem;
    }
}