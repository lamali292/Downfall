using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Flail : AutomatonCardModel
{
    public Flail() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(6, 3);
        WithKeywords(CardKeyword.Exhaust);
        this.WithTip<WeakPower>();
        this.WithTip<FrailPower>();
        this.WithTip<VulnerablePower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await PowerCmd.Remove<WeakPower>(Owner.Creature);
        await PowerCmd.Remove<FrailPower>(Owner.Creature);
        await PowerCmd.Remove<VulnerablePower>(Owner.Creature);
        PlayerCmd.EndTurn(Owner, false);
    }
}