using System;

namespace Utility.Persists.Infrastructure;

public static class DatabaseEx
{
    public static string GetConnectionString(string providerName, bool thrw = false)
    {
        //foreach (ConnectionStringSettings c in System.Configuration.ConfigurationManager.ConnectionStrings)
        //    if (c.ProviderName == providerName)
        //        return c.ConnectionString;

        // defaults to bin directory
        if (!thrw)
            return string.Empty;
        else
            throw new Exception($"add connection-string with a provider-name of '{providerName}' to the app config file or amend provider-name");
    }

    //public static async System.Threading.Tasks.Task<IObservable<T>> FromDbAsync<T>(System.Threading.Tasks.Task<List<T>> t)
    //{
    //    var x = await t;
    //    return x.ToObservable();

    //}
}
