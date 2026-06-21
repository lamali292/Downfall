using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class ShieldOfNight : HexaghostCardModel
{
    public ShieldOfNight() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(12, 3);
        this.WithScry(3, 1);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Zhen>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var result = await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
        foreach (var cardModel in result.Discarded.Where(card => card.Keywords.Contains(CardKeyword.Ethereal)))
            await CardCmd.Exhaust(ctx, cardModel);
    }
}