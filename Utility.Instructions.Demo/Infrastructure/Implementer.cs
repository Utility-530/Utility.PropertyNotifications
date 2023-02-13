using System;
using Utility.Trees;

namespace Utility.Instructions.Demo.Infrastructure
{
    public class Implementer
    {
        public ITree<string> Tree { get; } 

        int count = -1;

        public string character => AlphabetCharacter(count).ToString();

        public Implementer()
        {
            Tree = new Tree<string>("root");
        }

        public Procedure OnNext(Instruction instruction)
        {
            switch (instruction.Type)
            {
                case (InstructionType.InsertLast):
                    {
                        var add = instruction.Value.ToString();
                        var tree = Tree["root"][character];
                        tree.Add(add);
                        return new Procedure { Type = ProcedureType.InsertLast, Value = add };
                    }
                case (InstructionType.RemoveLast):
                    {
                        var remove = Tree[character].Items[^1];
                        Tree[character].Items.Remove(remove);
                        return new Procedure { Type = ProcedureType.RemoveLast, Value = remove };
                    }
                case (InstructionType.ChangeContent):
                    {
                        count++;
                        Tree.Add(new Tree<string>(character));
                        return new Procedure { Type = ProcedureType.ChangeContent };
                    }            
                case (InstructionType.RevertChangeContent):
                    {
                        var remove = Tree["root"].Items[^1];
                        Tree["root"].Items.Remove(remove);
                        count--;

                        return new Procedure { Type = ProcedureType.RevertChangeContent };
                    }
                default:
                    throw new Exception("3 dsfsd");
            }
        }


        const string chars = "abcdefghijklmnopqrstuvwxyz";

        public static char AlphabetCharacter(int index)
        {
            return chars[index % 26];
        }

    }
}
