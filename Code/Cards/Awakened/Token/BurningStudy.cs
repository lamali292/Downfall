using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Events;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class BurningStudy : AwakenedCardModel, ISpell, IOnAwaken
{
    public BurningStudy() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithPower<StrengthPower>(1, 1);
        WithPower<WeakPower>(1, 1);
    }

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.ApplySelf<StrengthPower>(this, DynamicVars.Power<StrengthPower>().BaseValue);
        foreach (var combatStateEnemy in CombatState.Enemies)
            await CommonActions.Apply<WeakPower>(combatStateEnemy, this, DynamicVars.Power<WeakPower>().BaseValue);
    }
}