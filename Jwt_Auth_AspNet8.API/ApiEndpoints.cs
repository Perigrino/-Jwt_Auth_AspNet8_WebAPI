namespace Jwt_Auth_AspNet8.API;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class User
    {
        private const string Base = $"{ApiBase}/user";
        
        public const string Get = $"{Base}/{{id:guid}}";
        //public const string GetWallet = $"{Base}/wallets/{{id:guid}}";
        public const string GetAll = Base;
        public const string Create = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }
    public static class ReferralCode
    {
        private const string Base = $"{ApiBase}/referralCode";
        
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string Create = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }
    
}