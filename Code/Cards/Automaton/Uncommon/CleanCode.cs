using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class CleanCode : AutomatonCardModel
{
    public CleanCode() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<RemoveErrorsPower>(3);
        WithTip(DownfallTip.Encode);
        WithTip(DownfallTip.Compile);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RemoveErrorsPower>(ctx, this);
    }
}