using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Iterate : AutomatonCardModel
{
    public Iterate() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4);
        this.WithRepeat(2,1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (Pile != null && Pile.Type == StashPile.Stash)
        {
            if (player == Owner)
            {
                await CardCmd.AutoPlay(choiceContext, this, null);
            }
        }
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        DynamicVars.Repeat.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
    }
}