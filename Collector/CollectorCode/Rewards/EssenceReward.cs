using BaseLib.Patches.Content;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Utils.CustomReward;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Collector.CollectorCode.Rewards;

public class EssenceReward(int amount, Player player) : CustomReward(player)
{
    [CustomEnum] public static RewardType EssenceRewardType;

    protected override RewardType RewardType => EssenceRewardType;

    public override SerializableCustomReward<CustomReward> SerializeMethod => Serializer;

    private static string RewardIcon => "res://Collector/images/ui/esse.png";
    protected override string IconPath => RewardIcon;
    public override Vector2 IconPosition => new(0.0f, -5f);

    public int Amount { get; private set; } = -1;

    public override bool IsPopulated => Amount >= 0;

    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COMBAT_REWARD_ESSENCE");
            desc.Add("essence", Amount);
            return desc;
        }
    }

    public static CustomReward Serializer(SerializableReward save, Player player)
    {
        return new EssenceReward(save.GoldAmount, player);
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = EssenceRewardType,
            GoldAmount = amount
        };
    }

    public override Task Populate()
    {
        Amount = amount;
        return Task.CompletedTask;
    }

    protected override async Task<bool> OnSelect()
    {
        Player.AddEssence(Amount);
        RunManager.Instance.RewardSynchronizer.GameService()?.SendMessage(new EssenceRewardMessage
        {
            wasSkipped = false,
            Location = RunManager.Instance.RewardSynchronizer.MessageBuffer()!.CurrentLocation,
            Amount = Amount
        });
        return true;
    }

    public override void MarkContentAsSeen()
    {
    }
}