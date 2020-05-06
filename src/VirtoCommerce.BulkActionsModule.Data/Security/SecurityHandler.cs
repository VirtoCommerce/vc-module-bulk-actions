namespace VirtoCommerce.BulkActionsModule.Data.Security
{
    using System.Linq;

    using VirtoCommerce.BulkActionsModule.Core.Security;
    using VirtoCommerce.Platform.Core.Security;

    public class SecurityHandler : ISecurityHandler
    {
        private readonly string[] _permissions;

        private readonly ISecurityService _securityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHandler"/> class.
        /// </summary>
        /// <param name="securityService">
        /// The security service.
        /// </param>
        /// <param name="permissions">
        /// The permissions.
        /// </param>
        public SecurityHandler(ISecurityService securityService, params string[] permissions)
        {
            _securityService = securityService;
            _permissions = permissions;
        }

        public bool Authorize(string userName)
        {
            return _permissions.All(permission => _securityService.UserHasAnyPermission(userName, null, permission));
        }
    }
}