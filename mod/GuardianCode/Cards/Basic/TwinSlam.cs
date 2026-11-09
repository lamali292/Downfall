using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Cards.Ancient;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class TwinSlam : GuardianCardModel, ITranscendenceCard, IGemSocketCard
{
    public TwinSlam() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithVar("GemReplay", 2);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public int GemSlots => IsUpgraded ? 2 : 1;
    public int GemReplayCount => DynamicVars["GemReplay"].IntValue;

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<BaubleBurst>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2).Execute(ctx);
    }
}
