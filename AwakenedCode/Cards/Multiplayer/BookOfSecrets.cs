using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class BookOfSecrets : AwakenedCardModel
{
    public BookOfSecrets() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithConjure();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithBlock(6);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (CombatState == null) return;
        var spell = AwakenedCmd.GetSpellbook(Owner);
        var nextSpell = spell?.NextSpell;
        if (nextSpell == null) return;
        foreach (var creature in CombatState.GetTeammatesOf(Owner.Creature)
                     .Where(c => c is { IsAlive: true, IsPlayer: true }))
        {
            var player = creature.Player;
            if (player == null || player == Owner) continue;
            var a = nextSpell.CreateClone();
            a._owner = player;
            await CardPileCmd.Add(a, PileType.Hand);
        }
    }
}