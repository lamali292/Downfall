using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class CleanCode : AutomatonCardModel
{
    public CleanCode() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(typeof(RemoveErrorsPower));
        WithTip(DownfallTip.Encode);
        WithTip(DownfallTip.Compile);
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RemoveErrorsPower>(Owner.Creature, 3, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}