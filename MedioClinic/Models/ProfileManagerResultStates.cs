namespace MedioClinic.Models
{
    public enum GetProfileResultState
    {
        UserNotFound,
        UserFound
    }

    public enum PostProfileResultState
    {
        UserNotFound,
        UserNotMapped,
        UserNotUpdated,
        UserUpdated
    }
}