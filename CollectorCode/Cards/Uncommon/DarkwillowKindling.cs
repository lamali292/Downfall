using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class DarkwillowKindling : CollectorCardModel, ISkipReplayOnSelfExhaust
{
    public DarkwillowKindling() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithKindle(1, 3);
        WithCards(2);
        WithCardTip<Ember>(WithPreviewModifiers);
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

        DownfallCardCmd.Enchant<Spiral>(ember, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await TorchheadCmd.Kindle(ctx, this);
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, DynamicVars.Cards.IntValue,
                CardPilePosition.Bottom, IsUpgraded,
                action: ember => DownfallCardCmd.Enchant<Spiral>(ember, 1));
        }
    }
}