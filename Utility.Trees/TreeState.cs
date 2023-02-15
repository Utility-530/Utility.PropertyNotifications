namespace Utility.Trees
{
    public record TreeState(int Index, State State, ITree Current,  ITree Forward, ITree Back, ITree Up, ITree Down, ITree Add, ITree Remove);
}
