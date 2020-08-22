namespace UtilityInterface.Generic.Database
{
    public interface IDeleteById<R>
    {
        bool DeleteById(R id);
    }

}
