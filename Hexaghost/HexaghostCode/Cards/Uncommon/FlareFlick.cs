using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class FlareFlick : HexaghostCardModel
{
    public FlareFlick() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(HexaghostKeyword.Advance, UpgradeType.Remove);
        WithDamage(10, 4);
        WithTips(c => c.IsUpgraded ? [
            HoverTipFactory.FromKeyword(HexaghostKeyword.Advance), 
            HoverTipFactory.FromKeyword(HexaghostKeyword.Retract)
        ] : []);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await HexaghostCmd.Ignite(ctx, Owner);
        if (IsUpgraded)
        {
            var choices = new[] { HexaghostKeyword.Advance, HexaghostKeyword.Retract }
                .Select(f => FlareFlickChoice.Create(f, Owner))
                .ToList();
            var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (chosen is not FlareFlickChoice {Keyword : var keyword } ) return;
            if (keyword == HexaghostKeyword.Advance)
                await HexaghostCmd.Advance(ctx, Owner, this);
            else if (keyword == HexaghostKeyword.Retract)
                await HexaghostCmd.Retract(ctx, Owner, this);
        }
    }
}

  
[Pool(typeof(HexaghostChoiceCardPool))]
public class FlareFlickChoice : HexaghostCardModel
{

    public FlareFlickChoice() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithTips(c => c is FlareFlickChoice { Keyword: var keyword } ? [HoverTipFactory.FromKeyword(keyword)] : []);
    }


    public CardKeyword Keyword { get; private set; }

    public static FlareFlickChoice Create(CardKeyword flame, Player owner)
    {
        var card = owner.Creature.CombatState!.CreateCard<FlareFlickChoice>(owner);
        card.Keyword = flame;
        return card;
    }
    
    
    
    public override string CustomPortraitPath => ModelDb.Card<FlareFlick>().CustomPortraitPath;
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Keyword",Keyword.GetTitle() );
    }
}