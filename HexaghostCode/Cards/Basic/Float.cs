using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Hexaghost.HexaghostCode.Cards.Basic;


[Pool(typeof(HexaghostCardPool))]
public class Float : HexaghostCardModel
{
    public Float() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithCards(1);
        WithKeyword(HexaghostKeyword.Advance, UpgradeType.Remove);
        WithTips(e => e.IsUpgraded
            ? [HoverTipFactory.FromKeyword(HexaghostKeyword.Retract),
                HoverTipFactory.FromKeyword(HexaghostKeyword.Advance)]
            :
            []);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        if (!IsUpgraded || CombatState == null) return;
        List<HexaghostCardModel> choices =
        [
            CombatState.CreateCard<FloatChoiceRetract>(Owner),
            CombatState.CreateCard<FloatChoiceAdvance>(Owner)
        ];
        var a = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        switch (a)
        {
            case FloatChoiceAdvance:
                await HexaghostCmd.Advance(ctx, Owner, this);
                break;
            case FloatChoiceRetract:
                await HexaghostCmd.Retract(ctx, Owner, this);
                break;
        }
    }
}


[Pool(typeof(TokenCardPool))]
public class FloatChoiceRetract : HexaghostCardModel
{
    public FloatChoiceRetract() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithTip(HexaghostKeyword.Retract);
    }
    public override string CustomPortraitPath =>
        $"{ModelDb.Card<Float>().Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImageAtlasPath<Core.Hexaghost>();
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<HexaghostCardPool>();
}

[Pool(typeof(TokenCardPool))]
public class FloatChoiceAdvance : HexaghostCardModel
{
    public FloatChoiceAdvance() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithTip(HexaghostKeyword.Advance);
    }
    public override string CustomPortraitPath =>
        $"{ModelDb.Card<Float>().Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImageAtlasPath<Core.Hexaghost>();
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<HexaghostCardPool>();
}
