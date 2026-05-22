using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BronzeArmor : AutomatonCardModel, IEncodable,
    ICompilableError
{
    public BronzeArmor() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(13, 4);
        WithStash(2);
        WithTip(typeof(Error));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await AutomatonCmd.Stash<Error>(Owner, DynamicVars["Stash"].IntValue);
    }
}