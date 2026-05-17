using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SongOfSorrow : AwakenedCardModel
{
    public SongOfSorrow() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<SongOfSorrowPower>(7, 3, false);
        WithTip(typeof(Void));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SongOfSorrowPower>(ctx, this);
    }
}