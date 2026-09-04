namespace UrlShortener.Core.Entity.Error;

public static class DomainErrors
{
    public static class Url
    {
        public static readonly Error NotFound = new(
            "Url.NotFound",
            "Url not found"
        );

        public static readonly Error AlreadyExists = new(
            "Url.AlreadyExists",
            "Such a short URL already exists"
        );
    }

    public static class Auth
    {
        public static readonly Error Forbidden = new(
            "Auth.Forbidden",
            "Forbidden"
        );

        public static readonly Error Unauthorized = new(
            "Auth.Unauthorized",
            "User is not authenticated"
        );
        
        public static readonly Error InvalidCredentials = new(
            "Auth.InvalidCredentials",
            "Invalid email or password"
        );
        
        public static readonly Error EmailAlreadyInUse = new(
            "Auth.EmailAlreadyInUse",
            "Email is already in use"
        );
        
        public static readonly Error RegistrationFailed = new(
            "Auth.RegistrationFailed",
            "User registration failed"
        );
    }

    public static class User
    {
        public static readonly Error UserNotFound = new(
            "User.NotFound",
            "User not found"
        );
        
        public static readonly Error EmailNotAvailable = new(
            "Auth.EmailNotAvailable", 
            "Current user email is not available"
        );

        public static readonly Error IdNotAvailable = new(
            "Auth.IdNotAvailable", 
            "Current user ID is not available"
        );

        public static readonly Error RoleNotAvailable = new(
            "Auth.RoleNotAvailable", 
            "Current user role is not available"
        );
        
        public static readonly Error UserAlreadyExists = new(
            "User.AlreadyExists",
            "User with this email already exists"
        );
        
        public static readonly Error FailedDeletingUser = new(
            "User.FailedDeleting",
            "Failed to delete user"
        );
    }

    public static class Operation
    {
        public static readonly Error DeletingFailed = new(
            "Operation.DeletingFailed",
            "Failed to delete the entity"
        );
    }
}
