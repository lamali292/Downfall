using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Basic;

[Pool(typeof(GremlinsCardPool))]
public class GremlinDance : GremlinsCardModel
{
    public GremlinDance() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithBlock(6, 3);
        WithCards(2);
        WithTips(card =>
        {
            return GremlinsCmd.GetCurrentGremlin(card._owner)?.Monster switch
            {
                ShieldGremlin => [HoverTipFactory.Static(StaticHoverTip.Block)],
                FatGremlin => [HoverTipFactory.FromPower<GremlinDancePower>()],
                WizardGremlin => [HoverTipFactory.FromPower<WizPower>()],
                GremlinNob => [HoverTipFactory.FromPower<StrengthPower>()],
                _ => []
            };
        });
    }

    private MonsterModel? CurrentGremlinMonster =>
        IsMutable ? GremlinsCmd.GetCurrentGremlin(_owner)?.Monster : null;

    public override bool GainsBlock => CurrentGremlinMonster is ShieldGremlin;

    public override TargetType TargetType =>
        CurrentGremlinMonster is not MadGremlin ? TargetType.AnyEnemy : TargetType.AllEnemies;

    private string GremlinName => CurrentGremlinMonster?.GetType().Name ?? "None";

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var current = GremlinsCmd.GetCurrentGremlin(Owner);
        if (current?.Monster is not GremlinsMonsterModel gremlin) return;

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        switch (gremlin)
        {
            case SneakGremlin:
                await CommonActions.Draw(this, ctx);
                break;
            case ShieldGremlin:
                await CommonActions.CardBlock(this, cardPlay);
                break;
            case FatGremlin:
                if (cardPlay.Target == null) return;
                await PowerCmd.Apply<GremlinDancePower>(ctx, cardPlay.Target, 2, Owner.Creature, this);
                break;
            case WizardGremlin:
                await PowerCmd.Apply<WizPower>(ctx, Owner.Creature, 2, Owner.Creature, this);
                break;
            case GremlinNob:
                await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, 2, Owner.Creature, this);
                break;
        }
    }


    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("GremlinVariant", GremlinName);
    }
}

public class GremlinDancePower : TemporaryDebuffPowerWrapper<GremlinDance, StrengthPower>
{
}