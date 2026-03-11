using FleetManagement.Common.Models;

namespace FleetManagement.HQ.Interfaces;


public interface IUserInteractor
{
    Task ProcessInputAsync(string input);
}
