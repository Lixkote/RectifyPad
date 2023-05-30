using Windows.Storage.AccessCache;
using System.Collections.Generic;
using System.Linq;
using RectifyPad.Helpers;
using System.Threading.Tasks;
using System;
using Windows.Storage;

public static class RecentlyUsedHelper
{
    private const string MRU_LIST_TOKEN = "mruListToken";
    private const int MAX_MRU_ENTRIES = 5;

    public static async Task<List<RecentlyUsedViewModel>> GetRecentlyUsedItems()
    {
        var mru = StorageApplicationPermissions.MostRecentlyUsedList;
        var mruEntries = await Task.WhenAll(mru.Entries.Select(async entry =>
        {
            try
            {
                var file = await mru.GetFileAsync(entry.Token);
                return new RecentlyUsedViewModel { Name = file.Name, Path = file.Path, OriginalFile = file, Token = entry.Token };
            }
            catch (Exception)
            {
                return null;
            }
        }));
        return mruEntries.Where(entry => entry != null).ToList();
    }

    public static async Task AddToMostRecentlyUsedList(StorageFile file)
    {
        var mru = StorageApplicationPermissions.MostRecentlyUsedList;
        if (mru.Entries.Count >= MAX_MRU_ENTRIES)
        {
            mru.Remove(mru.Entries.Last().Token);
        }
        var token = Guid.NewGuid().ToString();
        mru.Add(file, token);
    }
}
