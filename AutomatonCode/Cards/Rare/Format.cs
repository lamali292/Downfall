using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Format : AutomatonCardModel
{
    public Format() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(AutomatonTip.Encode);
        WithTip(typeof(Fragment));
        WithEnergy(1);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded) x += 1;

        for (var i = 0; i < x; i++)
        {
            var fragment = Owner.Creature.CombatState!.CreateCard<Fragment>(Owner);
            if (fragment is not IEncodable encodable) continue;
            await encodable.Encode(ctx, cardPlay);
        }

        await PlayerCmd.GainEnergy(1, Owner);
    }
}