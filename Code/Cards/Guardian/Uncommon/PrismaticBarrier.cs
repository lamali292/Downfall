using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class PrismaticBarrier : GuardianCardModel
{
    public PrismaticBarrier() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedBlock(0, 2, CalcBlock, ValueProp.Move, 0, 1);
        WithTip(DownfallKeywords.Gem);
    }

    public override int GemSlots => 3;

    private static decimal CalcBlock(CardModel card, Creature? arg2)
    {
        return card is GuardianCardModel gc ? gc.Gems.Count : 0;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}