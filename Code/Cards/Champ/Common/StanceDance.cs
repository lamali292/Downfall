using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Champ.Basic;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards.Mocks;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class StanceDance : ChampCardModel
{
    public StanceDance() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var choices = new List<CardModel>
        {
            Owner.Creature.CombatState!.CreateCard<StanceDanceBerserker>(Owner),
            Owner.Creature.CombatState!.CreateCard<StanceDanceDefensive>(Owner)
        };

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        if (chosen == null) return;
        switch (chosen)
        {
            case StanceDanceBerserker:
                await ChampCmd.EnterBerserkerStance(ctx, Owner);
                break;
            case StanceDanceDefensive:
                await ChampCmd.EnterDefensiveStance(ctx, Owner);
                break;
        }
    }


    protected override void OnUpgrade()
    {
    }
}


[Pool(typeof(TokenCardPool))]
public class StanceDanceBerserker() : ChampCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override string CustomPortraitPath => ModelDb.Card<BerserkersShout>().CustomPortraitPath;
}


[Pool(typeof(TokenCardPool))]
public class StanceDanceDefensive() : ChampCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override string CustomPortraitPath => ModelDb.Card<DefensiveShout>().CustomPortraitPath;
}


