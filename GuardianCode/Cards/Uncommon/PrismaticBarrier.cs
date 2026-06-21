using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class PrismaticBarrier : GuardianCardModel, IGemSocketCard
{
    public PrismaticBarrier() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedBlock(0, 2, CalcBlock, ValueProp.Move, 0, 1);
        WithTip(GuardianKeyword.Gem);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public int GemSlots => 3;

    private static decimal CalcBlock(CardModel card, Creature? arg2)
    {
        return card is IGemSocketCard gc ? gc.GemCount : 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}