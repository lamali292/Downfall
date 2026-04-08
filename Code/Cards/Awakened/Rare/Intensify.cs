using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Intensify : AwakenedCardModel
{
    public Intensify() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<IntensifyPower>(1);
        WithPower<BurnoutPower>(1);
        WithTip(DownfallKeyword.Conjure);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await CommonActions.ApplySelf<IntensifyPower>(this);
        await CommonActions.ApplySelf<BurnoutPower>(this);
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}