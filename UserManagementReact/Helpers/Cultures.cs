using System.Collections.Generic;
using System.Globalization;

namespace UserManagementReact.Helpers
{
	public static class Cultures
	{
		public struct CulturePair
		{
			public string Code { get; set; }
			public string LocalName { get; set; }
		}
		public static List<CulturePair> CulturePairs = new List<CulturePair>()
		{
            new CulturePair() { Code = "en", LocalName = "English" },
            new CulturePair() { Code = "el", LocalName = "Ελληνικά" }
		};

		public static string DefaultCulture = "el";

		public static List<CultureInfo> SupportedCultures
		{
			get
			{
				List<CultureInfo> returnList = new List<CultureInfo>();

				foreach (CulturePair culturePair in CulturePairs)
				{
					returnList.Add(new CultureInfo(culturePair.Code));
				}
				return returnList;
			}
		}
	}
}
