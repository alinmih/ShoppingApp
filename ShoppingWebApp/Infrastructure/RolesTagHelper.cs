using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ShoppingWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{
    //target actual html element wit user-role custom tag helper
    [HtmlTargetElement("td", Attributes ="user-role")]
    public class RolesTagHelper:TagHelper
    {
        private readonly RoleManager<IdentityRole> role_Manager;
        private readonly UserManager<AppUser> userManager;
        public RolesTagHelper(RoleManager<IdentityRole> role_Manager,
                               UserManager<AppUser> userManager)
        {
            this.role_Manager = role_Manager;
            this.userManager = userManager;
        }

        [HtmlAttributeName("user-role")]
        public string RoleId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> names = new List<string>();
            IdentityRole role = await role_Manager.FindByIdAsync(RoleId);
            
            if (role!=null)
            {
                foreach (var user in userManager.Users)
                {
                    if (user!=null&& await userManager.IsInRoleAsync(user, role.Name))
                    {
                        names.Add(user.UserName);
                    }
                }
            }

            output.Content.SetContent(names.Count == 0 ? "No users" : string.Join(", ", names));
        }

    }
}
