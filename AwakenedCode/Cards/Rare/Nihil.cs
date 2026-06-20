using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Nihil : AwakenedCardModel, IChantable
{
    public Nihil() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(13, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public bool HasChanted { get; set; } = false;

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var combatStateEnemy in CombatState.Enemies)
        {
            var a = combatStateEnemy.GetPowerAmount<ManaburnPower>();
            if (a <= 0) continue;
            await CreatureCmd.Damage(
                ctx,
                combatStateEnemy,
                a,
                ValueProp.Unpowered | ValueProp.Unblockable,
                this);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<ManaburnPower>(ctx, this, cardPlay);
    }
}