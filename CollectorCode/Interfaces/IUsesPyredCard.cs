using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Interfaces;

public interface IUsesPyredCard
{
    CardModel? PyredCard { get; set; }
}