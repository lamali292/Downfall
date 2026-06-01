using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class TomeOfPortalmancy : AwakenedRelicModel
{
    public TomeOfPortalmancy() : base(RelicRarity.Common)
    {
        WithPower<ManaburnPower>(2);
        WithTip(typeof(Void));
    }

    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? creator)
    {
        var combatState = Owner.Creature.CombatState;
        if (creator != Owner || card is not Void) return;
        Flash();
        await PowerCmd.Apply<ManaburnPower>(ctx,
            combatState!.HittableEnemies, 2, Owner.Creature, null);
    }
}