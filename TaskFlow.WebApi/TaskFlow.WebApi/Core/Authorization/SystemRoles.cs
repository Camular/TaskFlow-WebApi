namespace TaskFlow.WebApi.Core.Authorization
{
    public static class SystemRoles
    {
        public const string OwnerName = "Owner";
        public static readonly Guid OwnerId = Guid.Parse("87F54898-67B8-42C9-B3BF-DF33CD1D0C6A");

        public const string AdministratorName = "Administrator";
        public static readonly Guid AdministratorId = Guid.Parse("C89567A9-7C63-4C62-80DE-DD54FC3A1C9E");

        public const string MemberName = "Member";
        public static readonly Guid MemberId = Guid.Parse("ECB6C928-CC9B-404D-B04B-A9612C377CCE");

        public const string ViewerName = "Viewer";
        public static readonly Guid ViewerId = Guid.Parse("D191A4B1-1CE3-43ED-8EFA-3439CE5D6A9C");
    }
}
