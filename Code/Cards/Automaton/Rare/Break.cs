using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Break : AutomatonCardModel, IEncodable,
    ICompilableError
{
    public Break() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(15);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
        WithTip(typeof(Burn));
        WithTip(typeof(Void));
        WithTip(typeof(Dazed));
        WithTip(typeof(Slimed));
        WithTip(typeof(Wound));
    }

    public async Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        var combatState = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        List<CardModel> burns =
        [
            combatState.CreateCard<Dazed>(Owner),
            combatState.CreateCard<Slimed>(Owner),
            combatState.CreateCard<Wound>(Owner),
            combatState.CreateCard<Burn>(Owner),
            combatState.CreateCard<Void>(Owner)
        ];
        await CardPileCmd.AddGeneratedCardsToCombat(burns, PileType.Hand, true);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}