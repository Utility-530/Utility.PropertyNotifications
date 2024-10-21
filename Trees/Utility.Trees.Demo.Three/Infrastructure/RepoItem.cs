using System;

namespace Utility.Trees.Demo.MVVM
{
    public record RepoItem(
        Guid Guid, 
        RepoItemType ItemType, 
        string? Name = default,
        Type? Type = default, 
        int? Index = default,
        string? TableName = default,
        Guid? ParentGuid = default,
        Type? ParentType = default) 
    {
    }

    public record RepoResult(RepoItem RepoItem, RepoResultType ResultType)
    {


  
    }

    public record RepoResult2X(RepoItem RepoItem,Func<object> Func, RepoResultType ResultType) : RepoResult(RepoItem, ResultType)
    {

    }


    public record RepoResultX(RepoItem RepoItem, RepoResultType ResultType, Table Table): RepoResult(RepoItem, ResultType)
    {

    }


    public enum RepoItemType
    {
        Get, Find, SelectKeys,

    }
    public enum RepoResultType
    {
        Get, Find, SelectKeys,
        Special
    }
}

