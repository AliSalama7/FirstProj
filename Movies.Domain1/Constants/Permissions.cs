using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Domain.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionList(string module)
        {
            return new List<string>
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }
        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            foreach (var module in Enum.GetValues(typeof(Modules)))
                allPermissions.AddRange(GeneratePermissionList(module.ToString()));
            return allPermissions;
        }

        public static class Movies
        {
            public const string View = "Permissions.Movies.View";
            public const string Create = "Permissions.Movies.Create";
            public const string Edit = "Permissions.Movies.Edit";
            public const string Delete = "Permissions.Movies.Delete";
        }

    }
}
