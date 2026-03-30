using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SongOfSorrow : AwakenedCardModel
{
    public SongOfSorrow() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<SongOfSorrowPower>(7);
        WithTip(typeof(Void));
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SongOfSorrowPower>(this, DynamicVars.Power<SongOfSorrowPower>().BaseValue);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<SongOfSorrowPower>().UpgradeValueBy(3);
    }
}