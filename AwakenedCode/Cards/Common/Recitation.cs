using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Recitation : AwakenedCardModel, IChantable
{
    public Recitation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public bool HasChanted { get; set; } = false;

    // The actual attack (both hits, when chanting) is dealt from OnPlayInternal as a single
    // AttackCommand so buffs like Vigor - which are consumed after one AttackCommand.Execute -
    // apply to every hit instead of only the first. AwakenedCmd.Chant still calls this afterward
    // for its bookkeeping (ChantEntry, HasChanted, IOnChant), so it must not attack again itself.
    public Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay) => Task.CompletedTask;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var willChant = AwakenedCmd.WasLastCardPlayedPower(cardPlay) || HasChanted;
        await CommonActions.CardAttack(this, cardPlay, willChant ? 2 : 1).Execute(ctx);
    }
}