using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class CutThrough : AutomatonCardModel
{
    public CutThrough() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        this.WithScry( 1, 1);
        WithTip(AutomatonTip.Stash);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
        await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
        var cards = Owner.GetDraw();
        if (cards.Count == 0) return;
        await StashCmd.Stash(cards[0]);
    }
}