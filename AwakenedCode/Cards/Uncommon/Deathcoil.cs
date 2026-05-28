using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Deathcoil : AwakenedCardModel
{
    public Deathcoil() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(8, 3);
        this.WithDrained(1);
    }
    protected override Artist Artist => Artist.Get<Eudaimonia>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var ownerNode = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
        var targetNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
        if (ownerNode != null && targetNode != null)
        {
            // Start at the player's center, end at the enemy's center
            var start = ownerNode.VfxSpawnPosition;
            var target = targetNode.VfxSpawnPosition;

            // Fire the effect!
            NHemokinesisEffect.Spawn(start, target);
        }

        await CommonActions.Apply<ManaburnPower>(ctx, cardPlay.Target, this);
        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}