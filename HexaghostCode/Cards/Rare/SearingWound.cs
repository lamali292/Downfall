using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class SearingWound : HexaghostCardModel
{
    public SearingWound() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip<SoulBurnPower>();
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var scale = 1f;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            var amount = enemy.GetPowerAmount<SoulBurnPower>();
            if (amount <= 0) continue;
            await HexaghostCmd.SoulburnEffect(enemy, scale);
            scale *= 0.9f;
            await CompatibilityCreatureCmd.Damage(ctx, enemy, amount,
                DamageProps.cardHpLoss,
                Owner.Creature, this, cardPlay);
        }
    }
}