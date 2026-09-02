using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ProtoShield : AutomatonCardModel
{
    public ProtoShield() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(11, 3);
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    public override async Task AfterCardDrawn(
        PlayerChoiceContext ctx,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this) return;
        await DownfallCreatureCmd.GainBlock(Owner.Creature, this);
    }
}