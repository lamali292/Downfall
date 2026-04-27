using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class CrookedStrike : ChampCardModel
{
    public CrookedStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(ChampTag.Finisher);
        WithDamage(6, 3);
        WithTip(ChampTip.Finisher);
        WithTags(CardTag.Strike);
    }


    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        decimal a = Owner.Creature.GetPowerAmount<VigorPower>();
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        // Todo: Not consuming Vigor power needs a lot of annoying patching.
        if (a > 0)
            await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, a, Owner.Creature, this, true);

        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}