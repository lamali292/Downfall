using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ThunderWave : AutomatonCardModel
{
    public ThunderWave() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(18, 10);
        this.WithPower<BufferPower>(1, false);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await CommonActions.ApplySelf<BufferPower>(ctx, this);
    }
}