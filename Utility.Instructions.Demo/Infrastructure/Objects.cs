using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utility.Instructions.Demo.Infrastructure
{

    public enum InstructionType
    {
        InsertLast, RemoveLast, ChangeContent, RevertChangeContent
    }

    public enum ProcedureType
    {
        InsertLast, RemoveLast, ChangeContent, RevertChangeContent
    }

    public class Instruction
    {
        public InstructionType Type { get; set; }

        public object Value { get; init; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class Procedure
    {
        public ProcedureType Type { get; set; }
        public object Value { get; init; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }



    public class Generator
    {
        int count = 0;
        public Instruction OnNext(Procedure procedure)
        {
            switch (procedure.Type)
            {
                case (ProcedureType.InsertLast):
                    {
                        return new Instruction { Type = InstructionType.InsertLast, Value = procedure.Value };
                    }
                case (ProcedureType.RemoveLast):
                    {
                        return new Instruction { Type = InstructionType.RemoveLast, Value = procedure.Value };
                    }         
                case (ProcedureType.ChangeContent):
                    {
                        return new Instruction { Type = InstructionType.ChangeContent, Value = procedure.Value };
                    }
                case (ProcedureType.RevertChangeContent):
                    {
                        return new Instruction { Type = InstructionType.RevertChangeContent, Value = procedure.Value };
                    }
                default:
                    throw new Exception("3 d33 sfsd");
            }

        }

        public Instruction OnPrevious(Procedure procedure)
        {
            switch (procedure.Type)
            {
                case (ProcedureType.InsertLast):
                    {
                        return new Instruction { Type = InstructionType.RemoveLast, Value = procedure.Value };
                    }
                case (ProcedureType.RemoveLast):
                    {
                        return new Instruction { Type = InstructionType.InsertLast, Value = procedure.Value };
                    }
                case (ProcedureType.ChangeContent):
                    {
                        return new Instruction { Type = InstructionType.RevertChangeContent, Value = procedure.Value };
                    }
                case (ProcedureType.RevertChangeContent):
                    {
                        return new Instruction { Type = InstructionType.ChangeContent, Value = procedure.Value };
                    }
                default:
                    throw new Exception("3 d33 sfsd");
            }

        }
    }
}
