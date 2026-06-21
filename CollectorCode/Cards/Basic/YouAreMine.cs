using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Cards.Basic;

[Pool(typeof(CollectorCardPool))]
public class YouAreMine : CollectorCardModel
{
    public YouAreMine() : base(2, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
        WithPower<CollectorDoomPower>(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var vfx = DoomCurseEffect.Create(cardPlay.Target);
        if (vfx != null)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
        await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
        await CommonActions.Apply<CollectorDoomPower>(ctx, cardPlay.Target, this);
    }
}