using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using Awakened.AwakenedCode.Relics;
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
public class Darkleech : AwakenedCardModel, ISpell, IOnAwaken
{
    public Darkleech() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(1, 1);
        WithPower<ManaburnPower>(4, 2);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithTags(AwakenedTag.Spell);
    }

    // Here I don't follow my own rules regarding modularity and hardcoding of models in other models.
    // But would be too extreme to make a hook for this.
    public override TargetType TargetType =>
        _owner == null || Owner.GetRelic<EyeOfTheOccult>() == null ? TargetType.AnyEnemy : TargetType.AllEnemies;

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await CommonActions.Apply<ManaburnPower>(ctx, this, cardPlay);
    }
}