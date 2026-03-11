using FleetManagement.Common.Enums;
using FleetManagement.Common.Models;

namespace FleetManagement.HQ.Interfaces;

public interface IRequestBuilder
{
    Request Build(string shipId, CommandType commandType, string parametersStr);
}
