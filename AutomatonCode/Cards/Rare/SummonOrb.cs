using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]

//todo should probably use the same wording as Nostalgia
public class SummonOrb : AutomatonCardModel
{
    public SummonOrb() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<SummonOrbPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(AutomatonTip.Stash);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SummonOrbPower>(ctx, this);
    }
}