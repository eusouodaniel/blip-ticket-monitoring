using System.Reflection;

namespace take.desk.core.Extensions
{
	public static class ObjectExtensions
	{
		public static T InitializeObjectWithEmptyStringProps<T>(this T data)
		{
			PropertyInfo[] properties = data.GetType().GetProperties();
			foreach (var propertyInfo in properties)
			{
				if (propertyInfo.PropertyType == typeof(string))
				{
					propertyInfo.SetValue(data, string.Empty, null);
				}
			}
			return data;
		}
	}
}
