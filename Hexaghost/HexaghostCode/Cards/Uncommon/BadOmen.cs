using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Ghostflames;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class BadOmen : HexaghostCardModel
{
    public BadOmen() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await SelectGhostflame(ctx, Owner);
    }

    private static async Task SelectGhostflame(PlayerChoiceContext ctx, Player owner)
    {
        var choices = HexaghostModelDb.AllGhostflames
            .Select(f => BadOmenChoice.Create(f, owner))
            .ToList();
        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, owner);
        if (chosen is not BadOmenChoice { GhostflameModel : { } ghostflame }) return;
        HexaghostCmd.SetCurrentGhostflame(owner, ghostflame);
    }
}

[Pool(typeof(HexaghostChoiceCardPool))]
public class BadOmenChoice : HexaghostCardModel
{
    public BadOmenChoice() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithTips(c => c is BadOmenChoice { GhostflameModel: { } ghostflameModel } ? [ghostflameModel.HoverTip] : []);
    }


    public GhostflameModel? GhostflameModel { get; private set; }


    public override string CustomPortraitPath => ModelDb.Card<BadOmen>().CustomPortraitPath;

    public static BadOmenChoice Create(GhostflameModel flame, Player owner)
    {
        var card = owner.Creature.CombatState!.CreateCard<BadOmenChoice>(owner);
        card.GhostflameModel = flame;
        return card;
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Ghostflame", GhostflameModel?.Title ?? HexaghostModelDb.Ghostflame<InfernoGhostflame>().Title);
    }
}