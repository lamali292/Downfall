using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Thunderbolt() : AwakenedCardModel(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy), ISpell
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}