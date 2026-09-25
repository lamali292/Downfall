using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SunbloomKindling : CollectorCardModel
{
    public SunbloomKindling() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithKindle(6, 4);
        WithCards(2);
        WithUpgradingCardTip<Ember>(WithPreviewModifiers);
        WithEnchantment<Spiral>();
    }

    private static void WithPreviewModifiers(Ember ember, CardModel cardModel)
    {
        var val = 0;
        if (cardModel.IsUpgraded) val = 1;
        WithEnchantments(ember, val);
    }
    
    private static void WithEnchantments(Ember ember, int ups)
    {
        if (ups > 0)
        {
            CardCmd.Upgrade(ember);
        }
        DownfallCardCmd.ForceEnchant<Spiral>(ember, 1);
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CollectorCmd.Kindle(ctx, this);
        for (var v = 0; v > DynamicVars.Cards.IntValue; v++)
        {
            var ember = new Ember();
            DownfallCardCmd.ForceEnchant<Spiral>(ember, 1);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(ember);
            }
            await CardPileCmd.AddGeneratedCardToCombat(ember, PileType.Hand, Owner);
        }
    }
}