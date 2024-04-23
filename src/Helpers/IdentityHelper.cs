﻿using AzureNamingTool.Models;
using AzureNamingTool.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;

namespace AzureNamingTool.Helpers
{
    /// <summary>
    /// Helper class for identity-related operations.
    /// </summary>
    public class IdentityHelper
    {
        /// <summary>
        /// Checks if the user is an admin user.
        /// </summary>
        /// <param name="state">The state container.</param>
        /// <param name="session">The protected session storage.</param>
        /// <param name="name">The username.</param>
        /// <returns>True if the user is an admin user, otherwise false.</returns>
        public static async Task<bool> IsAdminUser(StateContainer state, ProtectedSessionStorage session, string name)
        {
            bool result = false;
            try
            {
                // Check if the username is in the list of Admin Users
                ServiceResponse serviceResponse = await AdminUserService.GetItems();
                if (serviceResponse.Success)
                {
                    if (GeneralHelper.IsNotNull(serviceResponse.ResponseObject))
                    {
                        List<AdminUser> adminusers = serviceResponse.ResponseObject!;
                        if (adminusers.Exists(x => x.Name.ToLower() == name.ToLower()))
                        {
                            state.SetAdmin(true);
                            await session.SetAsync("admin", true);
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
            }
            return result;
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="session">The protected session storage.</param>
        /// <returns>The current user.</returns>
        public static async Task<string> GetCurrentUser(ProtectedSessionStorage session)
        {
            string currentuser = "System";
            try
            {
                var currentuservalue = await session.GetAsync<string>("currentuser");
                if (!String.IsNullOrEmpty(currentuservalue.Value))
                {
                    currentuser = currentuservalue.Value;
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
            }
            return currentuser;
        }
    }
}
