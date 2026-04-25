using Downfall.Code.Abstract;
using Downfall.Code.Cards.Guardian.Basic;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Relics.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Character;

public class Guardian : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCA5B5BFF);
    public override string CharId => "Guardian";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<CurlUp>(),
        ModelDb.Card<TwinSlam>()
    ];

    protected override IEnumerable<string> ExtraAssetPaths =>
        DownfallModelDb.AllGems.Select(g => g.IconPath);

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeGear>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GuardianCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GuardianPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GuardianRelicPool>();


    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleNormal = new AnimState("idle", true);
        var idleDefensive = new AnimState("defensive", true);


        var animator = new CreatureAnimator(idleNormal, controller);
        animator.AddAnyState("Idle", idleNormal, IsInMode<GuardianNormalMode>);
        animator.AddAnyState("Idle", idleDefensive, IsInMode<GuardianDefensiveMode>);
        return animator;

        bool IsInMode<T>() where T : GuardianModeModel
        {
            return ControllerToPlayer.TryGetValue(controller, out var player)
                   && GuardianCmd.IsInMode<T>(player);
        }
    }
}