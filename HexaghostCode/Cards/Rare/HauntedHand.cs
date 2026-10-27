using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class HauntedHand : HexaghostCardModel, IHasAfterlifeEffect
{
    public HauntedHand() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithAfterlife();
        WithCards(3, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthereal)
    {
        var drawn = await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        if (!wasExhausted || !causedByEthereal) return;
        foreach (var card in drawn) card.GiveSingleTurnRetain();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay, false, false);
    }
}