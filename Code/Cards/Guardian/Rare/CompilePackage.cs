using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class CompilePackage : GuardianCardModel
{
    public CompilePackage() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip(DownfallTip.Stasis);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!GuardianCmd.CanPutIntoStasis(Owner) || CombatState == null) return;
        var a = ModelDb
            .CardPool<TokenCardPool>()
            .AllCards
            .OfType<IPackageCard>()
            .TakeRandom(3, CombatState.RunState.Rng.CombatCardGeneration)
            .Cast<CardModel>()
            .Select(Select)
            .ToList();
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, a, Owner);
        if (card == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }

    private CardModel Select(CardModel cardModel)
    {
        if (RunState == null) throw new InvalidOperationException();
        var card = CombatState!.CreateCard(cardModel, Owner);
        if (IsUpgraded)
            card.UpgradeInternal();
        return card;
    }
}