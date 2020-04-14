using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraCharts;

namespace mgWinChart.Helpers
{
    public abstract class ssChartPredicate
    {
        virtual public bool Evaluate(SeriesPoint point)
        {
            throw new NotImplementedException(string.Format("{0} does not override and implement Evaluate method.", this));
        }
    }

    public class ssSimpleChartPredicate : ssChartPredicate
    {

        #region Private Variables

        private int _comparisonPoint = 0;
        private PredicateComparisonType _comparisonType = PredicateComparisonType.Equals;
        private PredicateArgumentType _argumentType = PredicateArgumentType.Numeric;
        private string _stringArgument = string.Empty;
        private DateTime _datetimeArgument = DateTime.MinValue;
        private double _numericArgument = 0.0;

        #endregion Private Variables

        #region Constructors

        /// <summary>
        /// Creates a new simple predicate for filtering series points based on their string arguments.
        /// </summary>
        /// <param name="comparisonType">How to compare each argument to the value.</param>
        /// <param name="value">The value to compare each argument against.</param>
        public ssSimpleChartPredicate(PredicateComparisonType comparisonType, string value)
        {
            _comparisonPoint = -1;
            _comparisonType = comparisonType;
            _argumentType = PredicateArgumentType.String;
            _stringArgument = value;
        }

        /// <summary>
        /// Creates a new simple predicate for filtering series points based on a DateTime argument or value.
        /// </summary>
        /// <param name="comparisonPoint">The value from SeriesPoint.DateTimeValues to compare.  A comparison point of less than 0 compares the SeriesPoint.DateTimeArgument instead.</param>
        /// <param name="comparisonType">How to compare each point against this predicate's value.</param>
        /// <param name="value">The value to compare each point against.</param>
        public ssSimpleChartPredicate(int comparisonPoint, PredicateComparisonType comparisonType, DateTime value)
        {
            _comparisonPoint = comparisonPoint;
            if (_comparisonPoint < 0)
            {
                _comparisonPoint = -1;
            }
            _comparisonType = comparisonType;
            _argumentType = PredicateArgumentType.DateTime;
            _datetimeArgument = value;
        }

        /// <summary>
        /// Creates a new simple predicated for filtering series points based on a numerical argument or value.
        /// </summary>
        /// <param name="comparisonPoint">The value from SeriesPoint.Values to compare.  A comparison point of less than 0 compares the SeriesPoint.NumericalArgument instead.</param>
        /// <param name="comparisonType">How to compare each point against this predicate's value.</param>
        /// <param name="value">The value to compare each point against.</param>
        public ssSimpleChartPredicate(int comparisonPoint, PredicateComparisonType comparisonType, double value)
        {
            _comparisonPoint = comparisonPoint;
            if (_comparisonPoint < 0)
            {
                _comparisonPoint = -1;
            }
            _comparisonType = comparisonType;
            _argumentType = PredicateArgumentType.Numeric;
            _numericArgument = value;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Which value in the point this predicate evaluates.  The point's argument is -1.
        /// </summary>
        public int ComparisonPoint
        {
            get
            {
                return _comparisonPoint;
            }
        }

        /// <summary>
        /// What type of comparison to perform.
        /// </summary>
        public PredicateComparisonType ComparisonType
        {
            get
            {
                return _comparisonType;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether or not a series point satisfies this predicate.  
        /// </summary>
        /// <param name="point">The point to evaluate.</param>
        /// <returns>A boolean value indicating whether or not the point satisfies the condition.</returns>
        public override bool Evaluate(SeriesPoint point)
        {
            switch (_argumentType)
            {
                case PredicateArgumentType.Numeric:
                    double doubleValue = 0.0;
                    if (this.ComparisonPoint < 0)
                    {
                        doubleValue = point.NumericalArgument;
                    }
                    else if (this.ComparisonPoint >= point.Values.Length)
                    {
                        doubleValue = 0.0;
                    }
                    else
                    {
                        doubleValue = point.Values[this.ComparisonPoint];
                    }

                    switch (this.ComparisonType)
                    {
                        case PredicateComparisonType.DoesNotEqual:
                            return !(doubleValue == _numericArgument);
                        case PredicateComparisonType.Equals:
                            return (doubleValue == _numericArgument);
                        case PredicateComparisonType.IsGreaterThan:
                            return (doubleValue > _numericArgument);
                        case PredicateComparisonType.IsGreaterThanOrEqualTo:
                            return (doubleValue >= _numericArgument);
                        case PredicateComparisonType.IsLessThan:
                            return (doubleValue < _numericArgument);
                        case PredicateComparisonType.IsLessThanOrEqualTo:
                            return (doubleValue <= _numericArgument);
                        case PredicateComparisonType.IsLike:
                            return (Convert.ToInt32(doubleValue) == Convert.ToInt32(_numericArgument));
                    }
                    break;
                case PredicateArgumentType.DateTime:
                    DateTime dateValue = DateTime.MinValue;
                    if (this.ComparisonPoint < 0)
                    {
                        dateValue = point.DateTimeArgument;
                    }
                    else if (this.ComparisonPoint >= point.DateTimeValues.Length)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        dateValue = point.DateTimeValues[this.ComparisonPoint];
                    }

                    switch (this.ComparisonType)
                    {
                        case PredicateComparisonType.DoesNotEqual:
                            return !(dateValue == _datetimeArgument);
                        case PredicateComparisonType.Equals:
                            return (dateValue == _datetimeArgument);
                        case PredicateComparisonType.IsGreaterThan:
                            return (dateValue > _datetimeArgument);
                        case PredicateComparisonType.IsGreaterThanOrEqualTo:
                            return (dateValue >= _datetimeArgument);
                        case PredicateComparisonType.IsLessThan:
                            return (dateValue < _datetimeArgument);
                        case PredicateComparisonType.IsLessThanOrEqualTo:
                            return (dateValue <= _datetimeArgument);
                        case PredicateComparisonType.IsLike:
                            return dateValue.ToShortDateString().Equals(_datetimeArgument.ToShortDateString());
                    }
                    break;
                case PredicateArgumentType.String:
                    if (this.ComparisonType == PredicateComparisonType.IsLike)
                    {
                        return point.Argument.StartsWith(_stringArgument);
                    }
                    else
                    {
                        switch (point.Argument.CompareTo(_stringArgument))
                        {
                            case -1:
                                return (new PredicateComparisonType[] { PredicateComparisonType.DoesNotEqual, 
                                PredicateComparisonType.IsLessThan, PredicateComparisonType.IsLessThanOrEqualTo }
                                    ).Contains(this.ComparisonType);
                            case 0:
                                return (new PredicateComparisonType[] { PredicateComparisonType.Equals, 
                                PredicateComparisonType.IsGreaterThanOrEqualTo, PredicateComparisonType.IsLessThanOrEqualTo  }
                                    ).Contains(this.ComparisonType);
                            case 1:
                                return (new PredicateComparisonType[] { PredicateComparisonType.DoesNotEqual, 
                                PredicateComparisonType.IsGreaterThan, PredicateComparisonType.IsGreaterThanOrEqualTo }
                                    ).Contains(this.ComparisonType);
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        public override string ToString()
        {
            string argumentAsString = string.Empty;
            switch (_argumentType)
            {
                case PredicateArgumentType.DateTime:
                    argumentAsString = _datetimeArgument.ToString();
                    break;
                case PredicateArgumentType.Numeric:
                    argumentAsString = _numericArgument.ToString();
                    break;
                case PredicateArgumentType.String:
                    argumentAsString = _stringArgument;
                    break;
                default:
                    break;
            }

            string comparisonString = "Argument";
            if (this.ComparisonPoint >= 0)
            {
                comparisonString = string.Format("Value[{0}]", this.ComparisonPoint);
            }

            return string.Format("{0} {1} {2}", comparisonString, this.ComparisonType, argumentAsString);
            
        }

        #endregion Public Methods

    }

    public class ssCompoundChartPredicate : ssChartPredicate
    {

        #region Private Variables

        private PredicateConjunction _conjunction = PredicateConjunction.And;
        private List<ssChartPredicate> _subpredicates = new List<ssChartPredicate>();

        #endregion Private Variables

        #region Constructors

        /// <summary>
        /// Creates a new predicate that groups multiple other predicates under a conjunction.
        /// </summary>
        /// <param name="conjunction"></param>
        public ssCompoundChartPredicate(PredicateConjunction conjunction)
        {
            _conjunction = conjunction;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// The conjunction used to combine the subpredicates.
        /// </summary>
        public PredicateConjunction Conjunction
        {
            get
            {
                return _conjunction;
            }
            set
            {
                _conjunction = value;
            }
        }

        /// <summary>
        /// The predicates which this predicate combines.  
        /// </summary>
        public List<ssChartPredicate> Subpredicates
        {
            get
            {
                return _subpredicates;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Evaluates the point according to this predicate.
        /// </summary>
        /// <param name="point">The SeriesPoint object to compare to the predicate.</param>
        /// <returns>A boolean value indicating whether or not the </returns>
        public override bool Evaluate(SeriesPoint point)
        {
            switch (this.Conjunction)
            {
                case PredicateConjunction.And:
                    foreach (ssChartPredicate subpredicate in this.Subpredicates)
                    {
                        if (subpredicate.Evaluate(point) == false)
                        {
                            return false;
                        }
                    }
                    return true;
                case PredicateConjunction.NotAnd:
                    foreach (ssChartPredicate subpredicate in this.Subpredicates)
                    {
                        if (subpredicate.Evaluate(point) == false)
                        {
                            return true;
                        }
                    }
                    return false;
                case PredicateConjunction.NotOr:
                    foreach (ssChartPredicate subpredicate in this.Subpredicates)
                    {
                        if (subpredicate.Evaluate(point) == true)
                        {
                            return false;
                        }
                    }
                    return true;
                case PredicateConjunction.Or:
                    foreach (ssChartPredicate subpredicate in this.Subpredicates)
                    {
                        if (subpredicate.Evaluate(point) == true)
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            // If there are no subpredicates, return a simple boolean value.
            if (this.Subpredicates.Count == 0)
            {
                if (this.Conjunction == PredicateConjunction.And || this.Conjunction == PredicateConjunction.NotOr)
                {
                    return "TRUE";
                }
                else
                {
                    return "FALSE";
                }
            }

            // If there is just one, return it (with "NOT" if applicable).  
            if (this.Subpredicates.Count == 1)
            {
                if (this.Conjunction == PredicateConjunction.And || this.Conjunction == PredicateConjunction.Or)
                {
                    return this.Subpredicates[0].ToString();
                }
                else
                {
                    return string.Format("NOT {0}", this.Subpredicates[0].ToString());
                }
            }

            // If there are two or more, we have to build it one step at a time.
            StringBuilder sb = new StringBuilder();
            if (this.Conjunction == PredicateConjunction.NotAnd || this.Conjunction == PredicateConjunction.NotOr)
            {
                sb.Append("NOT ");
            }

            bool pastFirst = false;

            string conjunctionString = " AND ";
            if (this.Conjunction == PredicateConjunction.NotOr || this.Conjunction == PredicateConjunction.Or)
            {
                conjunctionString = " OR ";
            }

            foreach (ssChartPredicate subpredicate in this.Subpredicates)
            {
                if (pastFirst)
                {
                    sb.Append(conjunctionString);
                }
                else
                {
                    pastFirst = true;
                }

                sb.AppendFormat("({0})", subpredicate.ToString());
            }

            return sb.ToString();
        }

        #endregion Public Methods

    }

    #region Enumerations

    public enum PredicateArgumentType
    {
        Numeric,
        DateTime,
        String
    }

    public enum PredicateComparisonType
    {
        IsGreaterThan,
        IsGreaterThanOrEqualTo,
        IsLessThan,
        IsLessThanOrEqualTo,
        Equals,
        DoesNotEqual,
        IsLike
    }

    public enum PredicateConjunction
    {
        And,
        Or,
        NotAnd,
        NotOr
    }

    #endregion Enumerations

}
