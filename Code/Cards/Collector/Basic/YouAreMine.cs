using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using Downfall.Code.Vfx;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class YouAreMine : CollectorCardModel
{
    public YouAreMine() : base(2, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
        WithPower<CollectorDoomPower>(6, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var vfx = DoomCurseEffect.Create(cardPlay.Target);
        if (vfx != null)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        await CommonActions.Apply<WeakPower>(cardPlay.Target, this);
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this);
        await CommonActions.Apply<CollectorDoomPower>(cardPlay.Target, this);
    }
}