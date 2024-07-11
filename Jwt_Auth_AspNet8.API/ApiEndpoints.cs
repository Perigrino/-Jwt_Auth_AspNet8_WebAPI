namespace Jwt_Auth_AspNet8.API;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Auth
    {
        private const string Base = $"{ApiBase}/Auth";
        
        public const string SeedRoles = $"{Base}/seed-roles";
        //public const string GetWallet = $"{Base}/wallets/{{id:guid}}";
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
        public const string MakeAdmin = $"{Base}/make_user_admin";
        public const string MakeOwner = $"{Base}/make_user_owner";
        public const string RemoveOwnerRole = $"{Base}/remove_owner_role";
        public const string RemoveAdminRole = $"{Base}/remove_admin_role";
    }
}