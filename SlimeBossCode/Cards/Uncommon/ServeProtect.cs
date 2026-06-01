using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class ServeProtect : SlimeBossCardModel
{
    public ServeProtect() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedBlock(0, 10, Calc, ValueProp.Move, 0, 5);
        WithCalculatedVar("Blur", 0, Calc);
        this.WithTip<BlurPower>();
        WithKeyword(CardKeyword.Exhaust);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return SlimeQueue.GetCount(card.Owner);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var a = ((CalculatedVar)DynamicVars["Blur"]).Calculate(null);
        await CommonActions.ApplySelf<BlurPower>(ctx, this, a);
        await SlimeBossCmd.AbsorbAll(ctx, this);
    }
}