using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Events;
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
        WithPower<VulnerablePower>(1, 1);
        WithPower<ManaburnPower>(4, 2);
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
        await CommonActions.Apply<VulnerablePower>(ctx,cardPlay.Target, this);
        await CommonActions.Apply<ManaburnPower>(ctx,cardPlay.Target, this);
    }
}