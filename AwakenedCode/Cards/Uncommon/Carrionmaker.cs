using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Carrionmaker : AwakenedCardModel
{
    public Carrionmaker() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        WithDamage(9, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var extra = CombatManager.Instance.History.CardPlaysStarted.Count(s =>
            s.HappenedThisTurn(CombatState) && s.CardPlay.Card is ISpell);
        await CommonActions.CardAttack(this, cardPlay, 1 + extra).Execute(ctx);
    }
}