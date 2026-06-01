using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Basic;

[Pool(typeof(AwakenedCardPool))]
public class Hymn : AwakenedCardModel
{
    public Hymn() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(3, 3);
        this.WithTip<Ceremony>();
        this.WithDrained(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);

        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);

        var card = CombatState.CreateCard<Ceremony>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}