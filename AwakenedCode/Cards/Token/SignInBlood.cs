using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SignInBlood : AwakenedCardModel
{
    public SignInBlood() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithPower<StrengthPower>(2);
        WithKeywords(CardKeyword.Exhaust);
        WithVars(new HpLossVar(2));
        WithCards(3, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_bloody_impact");
        await CreatureCmd.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CommonActions.Draw(this, ctx);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}