using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class YouAreMine : CollectorCardModel
{
    public YouAreMine() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithPower<WeakPower>(3, 2);
        WithPower<FrailPower>(2, 3);
        WithPower<VulnerablePower>(2, 3);
        WithPower<MiasmaPower>(3, 2);
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var vfx = DoomCurseEffect.Create(CombatState!.HittableEnemies);
        if (vfx != null)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<FrailPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
    }
}