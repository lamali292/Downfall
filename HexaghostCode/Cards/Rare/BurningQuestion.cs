using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class BurningQuestion : HexaghostCardModel
{
    
    public BurningQuestion() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<IntensityPower>(3, 1);
        WithPower<MetallicizePower>(6, 2);
        WithPower<RoyaltiesPower>(30, 5);
    }
    public override bool CanBeGeneratedInCombat => false;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        CardModel[] choices =
        [
            BurningQuestionChoice1.Create(Owner),
            BurningQuestionChoice2.Create(Owner),
            BurningQuestionChoice3.Create(Owner)
        ];
        if (IsUpgraded)
            foreach (var cardModel in choices)
                cardModel.UpgradeInternal();

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        if (chosen is not BurningQuestionChoiceBase choice) return;
        await choice.OnSelect(ctx, cardPlay);
    }
}

[Pool(typeof(TokenCardPool))]
public abstract class BurningQuestionChoiceBase()
    : HexaghostCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<HexaghostCardPool>();
    public override string CustomPortraitPath => ModelDb.Card<BurningQuestion>().CustomPortraitPath;
    public abstract Task OnSelect(PlayerChoiceContext ctx, CardPlay cardPlay);
}

public class BurningQuestionChoice1 : BurningQuestionChoiceBase
{
    public BurningQuestionChoice1()
    {
        WithPower<IntensityPower>(3, 1);
    }

    public static BurningQuestionChoice1 Create(Player owner)
    {
        return owner.Creature.CombatState!.CreateCard<BurningQuestionChoice1>(owner);
    }

    public override Task OnSelect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<IntensityPower>(ctx, this);
    }
}

public class BurningQuestionChoice2 : BurningQuestionChoiceBase
{
    public BurningQuestionChoice2()
    {
        WithPower<MetallicizePower>(6, 2);
    }

    public static BurningQuestionChoice2 Create(Player owner)
    {
        return owner.Creature.CombatState!.CreateCard<BurningQuestionChoice2>(owner);
    }


    public override Task OnSelect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<MetallicizePower>(ctx, this);
    }
}

public class BurningQuestionChoice3 : BurningQuestionChoiceBase
{
    public BurningQuestionChoice3()
    {
        WithPower<RoyaltiesPower>(30, 5);
    }

    public static BurningQuestionChoice3 Create(Player owner)
    {
        return owner.Creature.CombatState!.CreateCard<BurningQuestionChoice3>(owner);
    }

    public override Task OnSelect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<RoyaltiesPower>(ctx, this);
    }
}