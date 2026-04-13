using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Spellshield : AwakenedCardModel
{
    public Spellshield() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithPower<SpellshieldPower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpellshieldPower>(this, DynamicVars.Power<SpellshieldPower>().BaseValue);
    }
}