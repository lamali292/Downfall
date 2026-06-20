using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize : AutomatonCardModel, IEncodable
{
    public Philosophize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(1, 1);
        this.WithPower<PhilosophizePower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<PhilosophizePower>(ctx, this);
    }
}