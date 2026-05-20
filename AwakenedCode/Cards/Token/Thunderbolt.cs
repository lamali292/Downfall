using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Thunderbolt : AwakenedCardModel, ISpell, IOnAwaken
{
    public Thunderbolt() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(12, 6);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithTags(AwakenedTag.Spell);
    }

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(ctx);
    }
}