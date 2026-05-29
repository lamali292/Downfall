using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class NullPointer : AutomatonCardModel,
    IEncodable
{
    public NullPointer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 3);
        WithBlock(12, 3);
        WithTip(CardKeyword.Unplayable);
        this.WithPower<NullPointerPower>(1, false);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<NullPointerPower>(ctx, this);
    }
}