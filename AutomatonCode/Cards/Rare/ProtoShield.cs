using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ProtoShield : AutomatonCardModel
{
    public ProtoShield() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<PlatingPower>(3, 2);
        this.WithTip<Error>();
        WithCards(1);
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    
    public override async Task AfterCardDrawn(
        PlayerChoiceContext ctx,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
        await DownfallCardCmd.GiveCards<Error>(Owner, PileType.Draw, DynamicVars.Cards.IntValue,
            CardPilePosition.Random);
    }
}