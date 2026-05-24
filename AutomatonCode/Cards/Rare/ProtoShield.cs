using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ProtoShield : AutomatonCardModel
{
    public ProtoShield() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(6, 2);
        WithPower<PlatingPower>(2, 1);
        WithTip(typeof(Error));
        WithCards(2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
        await DownfallCardCmd.GiveCards<Error>(Owner, PileType.Draw, DynamicVars.Cards.IntValue);
    }
}