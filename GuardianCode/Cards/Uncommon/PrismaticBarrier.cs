using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class PrismaticBarrier : GuardianCardModel, IGemSocketCard
{
    public PrismaticBarrier() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(2, 1);
        WithCalculatedVar("Repeat", 0, CalcRepeat);
        WithTip(GuardianKeyword.Gem);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public int GemSlots => 3;

    private static decimal CalcRepeat(CardModel card, Creature? arg2)
    {
        return card is IGemSocketCard gc ? gc.GemCount : 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = (int)((CalculatedVar)DynamicVars["Repeat"]).Calculate(cardPlay.Target);
        for (var i = 0; i < repeat; i++) await CommonActions.CardBlock(this, cardPlay);
    }
}