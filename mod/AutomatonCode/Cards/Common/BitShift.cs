using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class BitShift : AutomatonCardModel
{
    public BitShift() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(1, 3);
        WithTip(AutomatonTip.Stash);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cards = Owner.DrawPile;
        if (cards.Count == 0) return;
        await StashCmd.Stash(ctx, cards[0]);
    }
}