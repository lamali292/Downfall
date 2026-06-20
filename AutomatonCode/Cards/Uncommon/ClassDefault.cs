using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ClassDefault : AutomatonCardModel
{
    public ClassDefault() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(AutomatonTip.Encode);
        this.WithPower<ClassDefaultPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ClassDefaultPower>(ctx, this);
    }
}