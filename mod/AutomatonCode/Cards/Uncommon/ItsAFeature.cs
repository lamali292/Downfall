using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ItsAFeature : AutomatonCardModel
{
    public ItsAFeature() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<ItsAFeaturePower>(1, 1, false);
        WithTip<StrengthPower>();
        WithTip<DexterityPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ItsAFeaturePower>(ctx, this);
    }
}