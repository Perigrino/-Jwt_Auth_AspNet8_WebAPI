namespace Jwt_Auth_AspNet8.API;

public static class ApplicationCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection service)
    {
        //service.AddScoped<IUserRepository, UserRepository>();
        
        return service;
    }
    
    
    

}