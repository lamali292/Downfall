using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ForLoop : AutomatonCardModel
{
    public ForLoop() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(DownfallKeyword.Encode);
        WithTip(typeof(MergePower));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded)
            x += 1;
        await PowerCmd.Apply<MergePower>(Owner.Creature, x, Owner.Creature, this);
    }
}