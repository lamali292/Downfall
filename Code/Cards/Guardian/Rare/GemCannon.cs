using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Vfx.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Cards.Guardian.Rare;

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
        if (LocalContext.IsMe(Owner))
        {
            var from = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.VfxSpawnPosition ?? Vector2.Zero;
            var target = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target)?.VfxSpawnPosition ?? Vector2.Zero;

            for (var i = 0; i < gems.Count; i++)
            {
                var effect = NGemShootEffect.Create(gems[i], i, from, target, gems.Count);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(effect);
            }
        }
    
        await Cmd.Wait(0.3f); 
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    
        foreach (var gem in gems)
            await gem.OnPlay(ctx, cardPlay);
    }
}