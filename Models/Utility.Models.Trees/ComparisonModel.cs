using System;

namespace Utility.Models.Trees
{
    public class ComparisonModel : Model<Enum>
    {
        private Enum _value;
        private ComparisonType type;

        public ComparisonModel()
        {

        }

        public ComparisonType Type
        {
            get => type; set
            {
                type = value;
                switch (value)
                {
                    case ComparisonType.Default:
                        _value = null; break;
                    case ComparisonType.String:
                        _value = CustomStringComparison.EqualTo; break;
                    case ComparisonType.Number:
                        _value = NumberComparison.EqualTo; break;
                    case ComparisonType.Boolean:
                        _value = BooleanComparison.EqualTo; break;
                    case ComparisonType.Type:
                        _value = TypeComparison.EqualTo; break;
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Value));
            }
        }

        internal bool Compare(object value1, object value2)
        {
            switch (Type)
            {
                case ComparisonType.String:
                    switch ((CustomStringComparison)Value)
                    {
                        case CustomStringComparison.Contains:
                            return value1.ToString().Contains(value2.ToString());
                        case CustomStringComparison.EqualTo:
                            return value1.ToString().Equals(value2.ToString());
                        case CustomStringComparison.NotEqualTo:
                            return value1.ToString().Equals(value2.ToString()) == false;
                        case CustomStringComparison.DoesNotContain:
                            return value1.ToString().Contains(value2.ToString()) == false;
                    }
                    break;
                case ComparisonType.Number:
                    bool success1 = int.TryParse(value1.ToString(), out int int1);
                    bool success2 = int.TryParse(value1.ToString(), out int int2);
                    if (success1 && success2)
                        switch ((NumberComparison)Value)
                        {
                            case NumberComparison.GreaterThanOrEqualTo:
                                return int1 >= int2;
                            case NumberComparison.GreaterThan:
                                return int1 > int2;
                            case NumberComparison.LessThan:
                                return int1 < int2;
                            case NumberComparison.EqualTo:
                                return int1 == int2;
                            case NumberComparison.NotEqualTo:
                                return int1 != int2;
                            case NumberComparison.LessThanOrEqualTo:
                                return int1 <= int2;
                        }
                    return false;
                    break;
                case ComparisonType.Boolean:
                    switch ((BooleanComparison)Value)
                    {
                        case BooleanComparison.EqualTo:
                            return value1.ToString().Equals(value2.ToString());
                        case BooleanComparison.NotEqualTo:
                            return value1.ToString().Equals(value2.ToString()) == false;

                    }
                    break;
            }
            throw new NotImplementedException("f 33 dfd33");
        }
    }
}
