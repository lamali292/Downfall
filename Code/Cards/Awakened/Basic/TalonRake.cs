using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Awakened.Ancient;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Awakened.Basic;

[Pool(typeof(AwakenedCardPool))]
public class TalonRake : AwakenedCardModel, ITranscendenceCard
{
    public TalonRake() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithTip(DownfallTip.Conjure);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<TalonRend>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}