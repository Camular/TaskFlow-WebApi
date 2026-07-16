namespace TaskFlow.WebApi.Core.Authorization
{
    public static class SystemPermissions
    {
        public static class Workspace
        {
            public const string MemberAdd = "workspace.member.add";
            public static readonly Guid MemberAddId = Guid.Parse("02B08E56-E99F-4EF3-A99E-8EC70754194F");

            public const string View = "workspace.view";
            public static readonly Guid ViewId = Guid.Parse("8C9A2C14-6516-493E-9B21-56C113A86C13");

            public const string MemberRemove = "workspace.member.remove";
            public static readonly Guid MemberRemoveId = Guid.Parse("953CD531-6149-4E63-BD5D-862F41053D32");

            public const string RoleAssign = "workspace.role.assign";
            public static readonly Guid RoleAssignId = Guid.Parse("601059F0-0212-498F-9BF4-CDEE28E4BA2C");

            public const string RoleCreate = "workspace.role.create";
            public static readonly Guid RoleCreateId = Guid.Parse("6B08ED62-B54E-41B5-8F10-0E52062150DF");

            public const string RoleUpdate = "workspace.role.update";
            public static readonly Guid RoleUpdateId = Guid.Parse("0592DF3C-6B6B-4B1D-BFF2-5D779755A890");

            public const string RoleDelete = "workspace.role.delete";
            public static readonly Guid RoleDeleteId = Guid.Parse("E885FD39-00D9-4A16-B94B-BCF65B60845B");

            public const string Update = "workspace.update";
            public static readonly Guid UpdateId = Guid.Parse("DE5201A9-501F-4B61-9BB5-AFD6E7342C1A");

            public const string Delete = "workspace.delete";
            public static readonly Guid DeleteId = Guid.Parse("1A7B0835-2203-4305-B9BC-17633440D35C");
        }

        public static class TaskItem
        {
            public const string Create = "taskitem.create";
            public static readonly Guid CreateId = Guid.Parse("AF49DE62-068E-467E-8F17-9BA4CCF33473");

            public const string Read = "taskitem.read";
            public static readonly Guid ReadId = Guid.Parse("07A67E69-1D76-4BA4-A644-F04863A29C90");

            public const string Update = "taskitem.update";
            public static readonly Guid UpdateId = Guid.Parse("A2A7E721-5DC3-403A-BDD5-628887602839");

            public const string Delete = "taskitem.delete";
            public static readonly Guid DeleteId = Guid.Parse("753469FE-C14A-4E8A-914E-786F3E78322D");

            public const string StatusUpdate = "taskitem.status.update";
            public static readonly Guid StatusUpdateId = Guid.Parse("365CCFA9-D0CE-4A7A-8E07-A7EAA4238474");

            public const string DeadlineUpdate = "taskitem.deadline.update";
            public static readonly Guid DeadlineUpdateId = Guid.Parse("912C74BC-4F5C-4F1C-8753-05CFAD084BFD");

            public const string Assign = "taskitem.assign";
            public static readonly Guid AssignId = Guid.Parse("5193BC28-EA67-43D7-A105-7026D2D668E3");
        }

        public static class ActivityLog
        {
            public const string Read = "activitylog.read";
            public static readonly Guid ReadId = Guid.Parse("23C879F0-49CA-402A-9610-574677F4F70D");
        }

        public static class Comments
        {
            public const string Create = "comments.create";
            public static readonly Guid CreateId = Guid.Parse("93ECADF9-CE30-4843-AD61-236D39E7184B");

            public const string Read = "comments.read";
            public static readonly Guid ReadId = Guid.Parse("11AFB428-B2D3-40D3-8008-B5C7AECBA8B4");

            public const string Update = "comments.update";
            public static readonly Guid UpdateId = Guid.Parse("CD0C59F8-AB0F-45CC-97BC-98C6AC13344F");

            public const string Delete = "comments.delete";
            public static readonly Guid DeleteId = Guid.Parse("31E13039-CB2F-497C-8FB9-1D98D480AA96");
        }

        public static class Projects
        {
            public const string Create = "projects.create";
            public static readonly Guid CreateId = Guid.Parse("6151859B-1C5C-4BD3-8202-656C02AE189D");

            public const string Read = "projects.read";
            public static readonly Guid ReadId = Guid.Parse("66F5F8BC-A5CF-4851-8C01-79DD50CFA4B6");

            public const string Update = "projects.update";
            public static readonly Guid UpdateId = Guid.Parse("F5A6D297-A279-4563-967A-F8CD37AF9732");

            public const string Delete = "projects.delete";
            public static readonly Guid DeleteId = Guid.Parse("D5F4D12D-1533-4143-98F3-EF0F9D4C92ED");
        }

        public static class Attachments
        {
            public const string Create = "attachments.create";
            public static readonly Guid CreateId = Guid.Parse("B1CAED1D-4B83-47D2-839D-DC5A9B9B3617");

            public const string Read = "attachments.read";
            public static readonly Guid ReadId = Guid.Parse("B0670926-DF3F-41E3-A5D8-9A2CB70967E7");

            public const string Update = "attachments.update";
            public static readonly Guid UpdateId = Guid.Parse("FFC869ED-7030-4D5A-9EAB-9957D13EFE8C");

            public const string Delete = "attachments.delete";
            public static readonly Guid DeleteId = Guid.Parse("C3E756B7-E7C8-4373-8741-A3328E5413DD");
        }
    }
}
