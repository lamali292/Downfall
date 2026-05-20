using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class BurningStudy : AwakenedCardModel, ISpell, IOnAwaken
{
    public BurningStudy() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithPower<StrengthPower>(1, 1);
        WithPower<WeakPower>(1, 1);
        WithTags(AwakenedTag.Spell);
    }

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        foreach (var combatStateEnemy in CombatState.Enemies)
            await CommonActions.Apply<WeakPower>(ctx, combatStateEnemy, this);
    }
}