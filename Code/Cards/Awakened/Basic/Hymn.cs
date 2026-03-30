using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Basic;

[Pool(typeof(AwakenedCardPool))]
public class Hymn : AwakenedCardModel
{
    public Hymn() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(3);
        WithTip(typeof(Ceremony));
        WithTip(typeof(DrainedPower));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);

        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);

        var card = CombatState.CreateCard<Ceremony>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        await CommonActions.ApplySelf<DrainedPower>(this, 1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}