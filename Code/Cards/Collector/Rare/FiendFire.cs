using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class FiendFire : CollectorCardModel
{
    public FiendFire()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithTip(CardKeyword.Exhaust);
    }

    
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var list = PileType.Hand.GetPile(Owner).Cards.ToList();
        var cardCount = list.Count;
        foreach (var card2 in list)
            await CardCmd.Exhaust(ctx, card2);
        var scale = 0.8f;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(cardCount).FromCard(this).Targeting(cardPlay.Target).BeforeDamage((Func<Task>) (() =>
        {
            var child = NGroundFireVfx.Create(cardPlay.Target);
            if (child == null)
                return Task.CompletedTask;
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            child.Scale = Vector2.One * scale;
            var instance = NCombatRoom.Instance;
            instance?.CombatVfxContainer.AddChildSafely(child);
            scale += 0.1f;
            return Task.CompletedTask;
        })).Execute(ctx);
    }
    

}