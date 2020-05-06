namespace VirtoCommerce.BulkActionsModule.Data.Security
{
    using VirtoCommerce.BulkActionsModule.Core.Security;
    using VirtoCommerce.Platform.Core.Security;

    public class SecurityHandlerFactory : ISecurityHandlerFactory
    {
        private readonly ISecurityService _securityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHandlerFactory"/> class.
        /// </summary>
        /// <param name="securityService">
        /// The security service.
        /// </param>
        public SecurityHandlerFactory(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public ISecurityHandler Create(params string[] permissions)
        {
            return new SecurityHandler(_securityService, permissions);
        }
    }
}