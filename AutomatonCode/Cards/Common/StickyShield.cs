using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class StickyShield : AutomatonCardModel
{
    public StickyShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(11, 3);
        WithKeywords(CardKeyword.Retain);
        WithTip(typeof(Slimed));
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await DownfallCardCmd.GiveCard<Slimed>(Owner, PileType.Draw);
    }
}