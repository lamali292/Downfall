using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Cards.Ancient;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace Champ.ChampCode.Cards.Basic;

[Pool(typeof(ChampCardPool))]
public class Execute : ChampCardModel, ITranscendenceCard
{
    public Execute() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        this.WithFinisher();
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Execution>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2, "vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .WithAttackerAnim(Core.Champ.GetJumpAnimIfApplicable(Owner.Character), Core.Champ.GetJumpAttackDelayIfApplicable(Owner.Character))
            .Execute(ctx);
    }
}