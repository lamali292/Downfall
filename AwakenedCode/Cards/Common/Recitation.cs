using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Recitation : AwakenedCardModel
{
    public Recitation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null || cardPlay.Target == null) return;
        var context = await AttackCommand.CreateContextAsync(CombatState, ctx, cardPlay);
        var list1 = await CreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.Damage.BaseValue,
            DynamicVars.Damage.Props, this, cardPlay);
        context.AddHit(list1);
        await ChantCmd.Chant(cardPlay, async () =>
        {
            var list2 = await CreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.Damage.BaseValue,
                DynamicVars.Damage.Props, this, cardPlay);
            context.AddHit(list2);
        });
        
        await context.DisposeAsync();
    }
}