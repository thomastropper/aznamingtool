﻿using AzureNamingTool.Helpers;
using AzureNamingTool.Models;

namespace AzureNamingTool.Services
{
    /// <summary>
    /// Service for managing ResourceProjAppSvc items.
    /// </summary>
    public class ResourceProjAppSvcService
    {
        /// <summary>
        /// Retrieves a list of ResourceProjAppSvc items.
        /// </summary>
        /// <returns>A ServiceResponse containing the list of items.</returns>
        public static async Task<ServiceResponse> GetItems()
        {
            ServiceResponse serviceResponse = new();
            try
            {
                // Get list of items
                var items = await ConfigurationHelper.GetList<ResourceProjAppSvc>();
                if (GeneralHelper.IsNotNull(items))
                {
                    serviceResponse.ResponseObject = items.OrderBy(x => x.SortOrder).ToList();
                    serviceResponse.Success = true;
                }
                else
                {
                    serviceResponse.ResponseObject = "Resource Projects/Apps/Services not found!";
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
                serviceResponse.Success = false;
                serviceResponse.ResponseObject = ex;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Retrieves an item from the list of ResourceProjAppSvc items by its ID.
        /// </summary>
        /// <param name="id">The ID of the item to retrieve.</param>
        /// <returns>A ServiceResponse containing the retrieved item.</returns>
        public static async Task<ServiceResponse> GetItem(int id)
        {
            ServiceResponse serviceResponse = new();
            try
            {
                // Get list of items
                var items = await ConfigurationHelper.GetList<ResourceProjAppSvc>();
                if (GeneralHelper.IsNotNull(items))
                {
                    var item = items.Find(x => x.Id == id);
                    if (GeneralHelper.IsNotNull(item))
                    {
                        serviceResponse.ResponseObject = item;
                        serviceResponse.Success = true;
                    }
                    else
                    {
                        serviceResponse.ResponseObject = "Resource Project/App/Service not found!";
                    }
                }
                else
                {
                    serviceResponse.ResponseObject = "Resource Projects/Apps/Services not found!";
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
                serviceResponse.Success = false;
                serviceResponse.ResponseObject = ex;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Posts an item to the list of ResourceProjAppSvc items.
        /// </summary>
        /// <param name="item">The item to be posted.</param>
        /// <returns>A ServiceResponse indicating the success of the operation.</returns>
        public static async Task<ServiceResponse> PostItem(ResourceProjAppSvc item)
        {
            ServiceResponse serviceResponse = new();
            try
            {
                // Make sure the new item short name only contains letters/numbers
                if (!ValidationHelper.CheckAlphanumeric(item.ShortName))
                {
                    serviceResponse.Success = false;
                    serviceResponse.ResponseObject = "Short name must be alphanumeric.";
                    return serviceResponse;
                }

                // Get list of items
                var items = await ConfigurationHelper.GetList<ResourceProjAppSvc>();

                if (GeneralHelper.IsNotNull(items))
                {
                    // Set the new id
                    if (item.Id == 0)
                    {
                        if (items.Count > 0)
                        {
                            item.Id = items.Max(t => t.Id) + 1;
                        }
                        else
                        {
                            item.Id = 1;
                        }
                    }

                    int position = 1;
                    items = [.. items.OrderBy(x => x.SortOrder)];

                    if (item.SortOrder == 0)
                    {
                        item.SortOrder = items.Count + 1;
                    }

                    // Determine new item id
                    if (items.Count > 0)
                    {
                        // Check if the item already exists
                        if (items.Exists(x => x.Id == item.Id))
                        {
                            // Remove the updated item from the list
                            var existingitem = items.Find(x => x.Id == item.Id);
                            if (GeneralHelper.IsNotNull(existingitem))
                            {
                                int index = items.IndexOf(existingitem);
                                items.RemoveAt(index);
                            }
                        }

                        // Reset the sort order of the list
                        foreach (ResourceProjAppSvc thisitem in items.OrderBy(x => x.SortOrder).ToList())
                        {
                            thisitem.SortOrder = position;
                            position += 1;
                        }

                        // Check for the new sort order
                        if (items.Exists(x => x.SortOrder == item.SortOrder))
                        {
                            // Remove the updated item from the list
                            items.Insert(items.IndexOf(items.FirstOrDefault(x => x.SortOrder == item.SortOrder)!), item);
                        }
                        else
                        {
                            // Put the item at the end
                            item.SortOrder = position;
                            items.Add(item);
                        }
                    }
                    else
                    {
                        item.Id = 1;
                        item.SortOrder = 1;
                        items.Add(item);
                    }

                    position = 1;
                    foreach (ResourceProjAppSvc thisitem in items.OrderBy(x => x.SortOrder).ToList())
                    {
                        thisitem.SortOrder = position;
                        position += 1;
                    }

                    // Write items to file
                    await ConfigurationHelper.WriteList<ResourceProjAppSvc>(items);
                    serviceResponse.ResponseObject = "Resource Project/App/Service added/updated!";
                    serviceResponse.Success = true;
                }
                else
                {
                    serviceResponse.ResponseObject = "Resource Projects/Apps/Services not found!";
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
                serviceResponse.ResponseObject = ex;
                serviceResponse.Success = false;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Deletes an item from the list of ResourceProjAppSvc items.
        /// </summary>
        /// <param name="id">The ID of the item to be deleted.</param>
        /// <returns>A ServiceResponse indicating the success of the operation.</returns>
        public static async Task<ServiceResponse> DeleteItem(int id)
        {
            ServiceResponse serviceResponse = new();
            try
            {
                // Get list of items
                var items = await ConfigurationHelper.GetList<ResourceProjAppSvc>();
                if (GeneralHelper.IsNotNull(items))
                {
                    // Get the specified item
                    var item = items.Find(x => x.Id == id);
                    if (GeneralHelper.IsNotNull(item))
                    {
                        // Remove the item from the collection
                        items.Remove(item);

                        // Update all the sort order values to reflect the removal
                        int position = 1;
                        foreach (ResourceProjAppSvc thisitem in items.OrderBy(x => x.SortOrder).ToList())
                        {
                            thisitem.SortOrder = position;
                            position += 1;
                        }

                        // Write items to file
                        await ConfigurationHelper.WriteList<ResourceProjAppSvc>(items);
                        serviceResponse.Success = true;
                    }
                    else
                    {
                        serviceResponse.ResponseObject = "Resource Project/App/Service not found!";
                    }
                }
                else
                {
                    serviceResponse.ResponseObject = "Resource Projects/Apps/Services not found!";
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
                serviceResponse.ResponseObject = ex;
                serviceResponse.Success = false;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Posts the configuration for a list of ResourceProjAppSvc items.
        /// </summary>
        /// <param name="items">The list of ResourceProjAppSvc items to be configured.</param>
        /// <returns>A ServiceResponse indicating the success of the operation.</returns>
        public static async Task<ServiceResponse> PostConfig(List<ResourceProjAppSvc> items)
        {
            ServiceResponse serviceResponse = new();
            try
            {
                // Get list of items
                var newitems = new List<ResourceProjAppSvc>();
                int i = 1;

                // Determine new item id
                foreach (ResourceProjAppSvc item in items)
                {
                    // Make sure the new item short name only contains letters/numbers
                    if (!ValidationHelper.CheckAlphanumeric(item.ShortName))
                    {
                        serviceResponse.Success = false;
                        serviceResponse.ResponseObject = "Short name must be alphanumeric.";
                        return serviceResponse;
                    }

                    item.Id = i;
                    item.SortOrder = i;
                    newitems.Add(item);
                    i += 1;
                }

                // Write items to file
                await ConfigurationHelper.WriteList<ResourceProjAppSvc>(newitems);
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
                serviceResponse.ResponseObject = ex;
                serviceResponse.Success = false;
            }
            return serviceResponse;
        }
    }
}
