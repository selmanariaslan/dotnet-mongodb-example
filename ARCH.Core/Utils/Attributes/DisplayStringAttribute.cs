using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayStringAttribute : Attribute
    {
        /// <summary>
        /// The default value for the attribute <c>DisplayStringAttribute</c>, which is an empty string
        /// </summary>
        public static readonly DisplayStringAttribute Default = new DisplayStringAttribute();

        private readonly string _DisplayString;
        /// <summary>
        /// The value of this attribute
        /// </summary>
        public string DisplayString
        {
            get { return _DisplayString; }
        }

        /// <summary>
        /// Initializes a new instance of the class <c>DisplayStringAttribute</c> with default value (empty string)
        /// </summary>
        public DisplayStringAttribute()
            : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the class <c>DisplayStringAttribute</c> with specified value
        /// </summary>
        /// <param name="displayString">The value of this attribute</param>
        public DisplayStringAttribute(string displayString)
        {
            _DisplayString = displayString;
        }

        public override bool Equals(object obj)
        {
            var dsaObj = obj as DisplayStringAttribute;
            if (dsaObj == null)
                return false;

            return _DisplayString.Equals(dsaObj._DisplayString);
        }

        public override int GetHashCode()
        {
            return _DisplayString.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }
    }
}
