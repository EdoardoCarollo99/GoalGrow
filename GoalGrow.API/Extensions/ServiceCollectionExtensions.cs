namespace GoalGrow.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Services will be registered here as we create them
            // Example:
            // services.AddScoped<IUserService, UserService>();
            
            return services;
        }
    }
}
