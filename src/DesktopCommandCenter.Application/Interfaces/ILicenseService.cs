using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface ILicenseService
{
    /// <summary>
    /// Checks if the current authenticated user has an active PRO subscription.
    /// </summary>
    Task<bool> IsProUserAsync();
    
    /// <summary>
    /// Checks the exact plan from Firestore ("free" or "pro").
    /// </summary>
    Task<string> GetCurrentPlanAsync();

    Task<bool> HasUsedTrialAsync();
}
