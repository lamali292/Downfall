using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Cleanse : AutomatonCardModel
{
    public Cleanse() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(10, 4);
        WithTip(CardKeyword.Exhaust);
        WithTip(DownfallTip.Status);
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var drawPile = PileType.Draw.GetPile(Owner);
        var status = drawPile.Cards
            .Where(c => c.Type == CardType.Status)
            .OrderBy(_ => Guid.NewGuid()) // random
            .FirstOrDefault();

        if (status != null)
            await CardCmd.Exhaust(choiceContext, status);
    }
}