using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using Downfall.Code.Vfx;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Deathcoil : AwakenedCardModel
{
    public Deathcoil() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(8, 3);
        WithEnergyTip();
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
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

        await CommonActions.Apply<ManaburnPower>(cardPlay.Target, this, DynamicVars.Power<ManaburnPower>().BaseValue);
        await CommonActions.ApplySelf<DrainedPower>(this, 1);
    }
}