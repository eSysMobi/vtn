using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Common
{
    /// <summary>
    /// пользовательское исключение для обозначения ошибок, связанных с вводом некорректных данных
    /// </summary>
    public class IncorrectDataException : Exception
    {
        public IncorrectDataException()
            : base()
        {
        }
        
        public IncorrectDataException(String message)
            : base(message)
        {
        }

        public IncorrectDataException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

	/// <summary>
	/// пользовательское исключение для обозначения ошибок, связанных с пропуском ввода обязательных полей
	/// </summary>
	public class EmptyRequiredFieldException : Exception
	{
		public EmptyRequiredFieldException()
			: base()
		{
		}

		public EmptyRequiredFieldException(String message)
			: base(message)
		{
		}

		public EmptyRequiredFieldException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	/// <summary>
	/// пользовательское исключение для обозначения ошибок, связанных с ККМ
	/// </summary>
	public class KKMException : Exception
	{
		public KKMException()
			: base()
		{
		}

		public KKMException(String message)
			: base(message)
		{
		}

		public KKMException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
