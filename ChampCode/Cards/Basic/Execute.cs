using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Cards.Ancient;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Basic;

[Pool(typeof(ChampCardPool))]
public class Execute : ChampCardModel, ITranscendenceCard
{
    public Execute() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithFinisher();
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Execution>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(2).Execute(ctx);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}