using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class AphoticFount : AwakenedCardModel
{
    public AphoticFount() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<AphoticFountPower>(1, 1, false);
        //todo figure out if this actually works in game
        this.WithTip<PlatingPower>();
        this.WithTip<Cryostasis>();
        this.WithConjure();
    }

    protected override Artist Artist => Artist.Get<Eudaimonia>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await CommonActions.ApplySelf<AphoticFountPower>(ctx, this);
    }
}