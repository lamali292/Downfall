using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class BookOfSecrets : AwakenedCardModel
{
    public BookOfSecrets() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithConjure();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithBlock(6);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (CombatState == null) return;
        var spellbook = AwakenedCmd.GetSpellbook(Owner);
        var nextSpell = spellbook.NextSpell;
        if (nextSpell == null) return;
        foreach (var player in Owner.OtherTeammates)
        {
            var a = nextSpell.CreateClone();
            a._owner = player;
            await CardPileCmd.Add(a, PileType.Hand);
        }
    }
}