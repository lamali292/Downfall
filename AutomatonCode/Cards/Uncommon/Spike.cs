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
public class Spike : AutomatonCardModel, IEncodable
{
    public Spike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 1);
        this.WithPower<SpikePower>(3, 2, false);
        this.WithTip<ThornsPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<SpikePower>(ctx, this);
    }
}