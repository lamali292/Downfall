using BaseLib.Utils;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class GemCannon : GuardianCardModel
{
    public GemCannon() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var gems = GuardianCmd.GetAllCombatGems(Owner).ToList();
        var from = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.VfxSpawnPosition ?? Vector2.Zero;
        var target = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target)?.VfxSpawnPosition ?? Vector2.Zero;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        for (var i = 0; i < gems.Count; i++)
        {
            var effect = NGemShootEffect.Create(gems[i], i, from, target, gems.Count);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(effect);
        }
        foreach (var gem in gems)
        {
            await Cmd.Wait(0.2f);
            await gem.OnPlayWrapper(ctx, cardPlay);
        }
    }
}