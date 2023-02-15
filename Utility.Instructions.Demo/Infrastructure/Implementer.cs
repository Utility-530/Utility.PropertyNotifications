using System;
using System.Linq;
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

        public void OnNext(Instruction instruction)
        {
            switch (instruction.Type)
            {
                case (InstructionType.InsertLast):
                    {
                        InsertLast(instruction);
                        break;
                    }
                case (InstructionType.RemoveLast):
                    {
                        RemoveLast(instruction);
                        break;

                    }
                case (InstructionType.ChangeContent):
                    {
                        ChangeContent();
                        break;

                    }
                case (InstructionType.RevertChangeContent):
                    {
                        RevertChangeContent();
                        break;
                    }
                default:
                    throw new Exception("3 dsfsd");
            }
        }
           
        public void OnPrevious(Instruction instruction)
        {
            switch (instruction.Type)
            {
                case (InstructionType.InsertLast):
                    {
                        RemoveLast(instruction);
                        break;
                    }
                case (InstructionType.RemoveLast):
                    {
                        InsertLast(instruction);
                        break;

                    }
                case (InstructionType.ChangeContent):
                    {
                        RevertChangeContent();
                        break;

                    }
                case (InstructionType.RevertChangeContent):
                    {
                        ChangeContent();
                        break;
                    }
                default:
                    throw new Exception("3 dsfsd");
            }
        }

        private void RevertChangeContent()
        {
            var remove = Tree["root"].Items.Last();
            Tree["root"].Remove(remove);
            count--;
        }

        private void ChangeContent()
        {
            count++;
            Tree.Add(new Tree<string>(character));
        }

        private void RemoveLast(Instruction instruction)
        {
            var remove = instruction.Value;
            Tree[character].Remove(remove);
        }

        private void InsertLast(Instruction instruction)
        {
            var add = instruction.Value.ToString();
            var tree = Tree["root"][character];
            tree.Add(add);
        }

        const string chars = "abcdefghijklmnopqrstuvwxyz";

        public static char AlphabetCharacter(int index)
        {
            return chars[index % 26];
        }

    }
}
