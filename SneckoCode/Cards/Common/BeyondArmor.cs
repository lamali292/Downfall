using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class BeyondArmor : SneckoCardModel, IHasGift
{
    public BeyondArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(5, 3);
        WithCards(2);
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
        WithTip(DownfallTip.Offclass);
    }

    public Gift? Gift { get; set; }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cards = Owner.GetDraw()
            .Where(DownfallCmd.IsOffclass)
            .TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardSelection);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}