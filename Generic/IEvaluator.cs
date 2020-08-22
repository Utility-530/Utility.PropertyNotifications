namespace UtilityInterface.Generic
{
    public interface IEvaluator<T, R>
    {
        R Evaluator(T t);
    }

}
