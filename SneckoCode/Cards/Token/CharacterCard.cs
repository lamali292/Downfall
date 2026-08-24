using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Cards;

[Pool(typeof(TokenCardPool))]
#pragma warning disable
public class CharacterCard : ConstructedCardModel,
    IModfyCardDescription
#pragma warning restore
{
    
    public  CharacterCard() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithTip(SneckoTip.Gift);
    }
    
    internal CharacterModel? CharacterModel;
    private string? _portraitPath;

    public override CardPoolModel VisualCardPool => CharacterModel?.CardPool ?? base.VisualCardPool;
    public override string PortraitPath => _portraitPath ?? base.PortraitPath;
    protected override bool IsPlayable => false;

    public override string Title => CharacterModel == null
        ? "???"
        : new LocString("characters", CharacterModel.CharacterSelectTitle)
            .GetFormattedText();
    

    public LocString ModifyDescription(LocString oldLocString)
    {
        if (CharacterModel == null) return oldLocString;

        var desc = ModelDb.Relic<SneckoChoice>().Description;
        desc.Add("borrowed", CharacterModel?.Title.GetFormattedText() ?? "???");
        return desc;
    }

    public static CharacterCard Create(CharacterModel characterModel)
    {
        var a = ModelDb.Card<CharacterCard>().ToMutable();
        if (a is not CharacterCard characterCard) throw new Exception("CharacterCard model is not a CharacterCard");
        characterCard.CharacterModel = characterModel;
        characterCard._portraitPath = Rng.Chaotic.NextItem(characterModel.CardPool.AllCards.Where(e => e.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare))!.PortraitPath;
        NCard.FindOnTable(characterCard)?.Reload();
        return characterCard;
    }
}