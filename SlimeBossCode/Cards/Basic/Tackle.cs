using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Ancient;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class Tackle : SlimeBossCardModel, ITranscendenceCard
{
    public Tackle() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(13, 4);
        this.WithSelfDamage(3);
        WithTags(SlimeBossTag.Tackle);
    }
    
    
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<AncientDarv>();
    }

    protected override Artist Artist => Artist.Get<HalfGoblinHankins>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await MyCommonActions.SelfDamage(ctx, this);
    }
}