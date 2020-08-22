namespace UtilityInterface.Generic.Database
{
    public interface ISelectById<T, R>
    {
        T SelectById(R id);
    }

}
