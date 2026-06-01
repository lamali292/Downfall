using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
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
        this.WithUpgradedCardTip<Fragment>();
        WithEnergy(1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded) x += 1;

        for (var i = 0; i < x; i++)
        {
            var fragment = Owner.Creature.CombatState!.CreateCard<Fragment>(Owner);
            fragment.UpgradeInternal();
            await AutomatonCmd.EncodeCard(fragment, ctx);
        }

        await PlayerCmd.GainEnergy(1, Owner);
    }
}