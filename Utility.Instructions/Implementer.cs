using System;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Trees;

namespace Utility.Instructions.Demo.Infrastructure
{
    //public class Implementer
    //{
    //    public ITree<string> Tree { get; }


    //    public Implementer()
    //    {
    //        Tree = new Tree<string>("root");
    //    }

    //    public void OnNext(Instruction instruction)
    //    {
    //        switch (instruction.Type)
    //        {
    //            case (InstructionType.InsertLast):
    //                {
    //                    InsertLast(instruction);
    //                    break;
    //                }
    //            case (InstructionType.RemoveLast):
    //                {
    //                    RemoveLast(instruction);
    //                    break;

    //                }
    //            case (InstructionType.ChangeContent):
    //                {
    //                    ChangeContent(instruction);
    //                    break;

    //                }
    //            case (InstructionType.RevertChangeContent):
    //                {
    //                    RevertChangeContent(instruction);
    //                    break;
    //                }
    //            default:
    //                throw new Exception("3 dsfsd");
    //        }
    //    }
           
    //    public void OnPrevious(Instruction instruction)
    //    {
    //        switch (instruction.Type)
    //        {
    //            case (InstructionType.InsertLast):
    //                {
    //                    RemoveLast(instruction);
    //                    break;
    //                }
    //            case (InstructionType.RemoveLast):
    //                {
    //                    InsertLast(instruction);
    //                    break;

    //                }
    //            case (InstructionType.ChangeContent):
    //                {
    //                    RevertChangeContent(instruction);
    //                    break;

    //                }
    //            case (InstructionType.RevertChangeContent):
    //                {
    //                    ChangeContent(instruction);
    //                    break;
    //                }
    //            default:
    //                throw new Exception("3 dsfsd");
    //        }
    //    }

    //    public void RevertChangeContent(Instruction instruction)
    //    {
    //        Tree["root"].Remove(instruction.Value.ToString());
    //    }

    //    public void ChangeContent(Instruction instruction)
    //    {
    //        Tree["root"].Add(new Tree<string>(instruction.Value.ToString()) { });
    //    }

    //    public void RemoveLast(Instruction instruction)
    //    {
    //        Tree[instruction.Key].Remove(instruction.Value);
    //    }

    //    public void InsertLast(Instruction instruction)
    //    {
    //        Tree[instruction.Key].Add(instruction.Value);
    //    }
    //}
}
