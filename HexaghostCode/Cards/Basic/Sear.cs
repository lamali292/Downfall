using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Cards.Ancient;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Basic;

[Pool(typeof(HexaghostCardPool))]
public class Sear : HexaghostCardModel, ITranscendenceCard, IHasAfterlifeEffect
{
    public Sear() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        this.WithAfterlife();
        WithDamage(5, 2);
        WithPower<SoulBurnPower>(5, 2);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Apocryphra>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await AfterlifeEffect(ctx, cardPlay);
    }
}