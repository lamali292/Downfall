using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Darkleech : AwakenedCardModel, ISpell
{
    public Darkleech() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(1);
        WithPower<ManaburnPower>(4);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this, DynamicVars["VulnerablePower"].BaseValue);
        await CommonActions.Apply<ManaburnPower>(cardPlay.Target, this, DynamicVars["ManaburnPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
        DynamicVars["ManaburnPower"].UpgradeValueBy(2);
    }
}