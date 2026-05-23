using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Premonition : HexaghostCardModel
{
    public Premonition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var cardType = await GetCardType(ctx);
        if (cardType == null) return;
        var card = Owner.GetDraw(e => e.Type == cardType)
            .TakeRandom(1, CombatState.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (card == null) return;
        await CardCmd.AutoPlay(ctx, card, null);
    }

    private async Task<CardType?> GetCardType(PlayerChoiceContext ctx)
    {
        var choices = Owner.GetDraw().Select(c => c.Type).Distinct()
            .Select(f => PremonitionChoice.Create(f, Owner))
            .ToList();
        switch (choices.Count)
        {
            case 0:
                return null;
            case 1:
                return choices[0].Type;
            case > 1:
            {
                var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
                if (chosen is PremonitionChoice { Type : var cardType }) return cardType;
                return null;
            }
            default:
                return null;
        }
    }
}

[Pool(typeof(HexaghostChoiceCardPool))]
public class PremonitionChoice : HexaghostCardModel
{
    public PremonitionChoice() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    public override CardType Type => MyType;
    private CardType MyType { get; set; } = CardType.Skill;


    public override string CustomPortraitPath => ModelDb.Card<Premonition>().CustomPortraitPath;


    public static PremonitionChoice Create(CardType cardType, Player owner)
    {
        var card = owner.Creature.CombatState!.CreateCard<PremonitionChoice>(owner);
        card.MyType = cardType;
        return card;
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Type", MyType.ToLocString());
    }
}