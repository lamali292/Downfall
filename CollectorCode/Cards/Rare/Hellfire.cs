using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Hellfire : CollectorCardModel
{
    public Hellfire() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Megapyre);
        WithTip(CollectorTip.Pyred);
        WithKeyword(CardKeyword.Exhaust);
        WithTorchheadDamage(5, 3);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var pyred = await PyreCmd.MegaPyre(ctx, this);
        if (pyred.Count == 0) return;
        for (var i = 0; i < pyred.Count; i++)
        {
            await TorchheadCmd.AttackOrKindle(ctx, this, cardPlay, attack => attack.WithHitFx("vfx/vfx_attack_slash"));
        }
    }


}