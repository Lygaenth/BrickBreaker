namespace Cassebrique.Services
{
    /// <summary>
    /// Interface dor Authentication token manager
    /// </summary>
    public interface IAuthenticationTokenService
    {
        /// <summary>
        /// Get authentication token
        /// </summary>
        /// <returns></returns>
        string GetToken();
    }
}