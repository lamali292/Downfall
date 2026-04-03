using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Events;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Darkleech : AwakenedCardModel, ISpell, IOnAwaken
{
    public Darkleech() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(1);
        WithPower<ManaburnPower>(4);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this,
            DynamicVars.Power<VulnerablePower>().BaseValue);
        await CommonActions.Apply<ManaburnPower>(cardPlay.Target, this, DynamicVars.Power<ManaburnPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<VulnerablePower>().UpgradeValueBy(1);
        DynamicVars.Power<ManaburnPower>().UpgradeValueBy(2);
    }
}