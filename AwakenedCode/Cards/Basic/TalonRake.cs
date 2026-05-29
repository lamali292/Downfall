using Awakened.AwakenedCode.Cards.Ancient;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Basic;

[Pool(typeof(AwakenedCardPool))]
public class TalonRake : AwakenedCardModel, ITranscendenceCard
{
    public TalonRake() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        this.WithConjure();
    }


    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<TalonRend>();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        if (CombatState == null) return;
        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}