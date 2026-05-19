using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class CultistStrike : AutomatonCardModel,
    IEncodable, ICompilable
{
    public CultistStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVar("Increase", 1);
        WithCostUpgradeBy(-1);
    }

    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        if (!forGameplay) return;

        var deckCard = DeckVersion ?? this;
        deckCard.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        deckCard.DynamicVars.FinalizeUpgrade();

        await Cmd.Wait(0.5f);
        Owner.RunState.CurrentMapPointHistoryEntry?
            .GetEntry(Owner.NetId).UpgradedCards.Add(deckCard.Id);

        if (LocalContext.IsMine(deckCard))
        {
            var vfx = NCardSmithVfx.Create([deckCard]);
            if (vfx != null)
                NCombatRoom.Instance?.Ui.CardPreviewContainer
                    .AddChildSafely(vfx);
        }
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}