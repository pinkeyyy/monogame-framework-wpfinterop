using System.Windows;

namespace MonoGame.Framework.WpfInterop.Internals
{
	public static class LogicalTreeHelperEx
	{
		/// <summary>
		/// Gets the parent of a specific type that hosts the specific child.
		/// Returns null if no match is found
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="child"></param>
		/// <returns></returns>
		public static T FindParent<T>(DependencyObject child) where T : DependencyObject
		{
			while (true)
			{
				//get parent item
				DependencyObject parentObject = LogicalTreeHelper.GetParent(child);

				//we've reached the end of the tree
				if (parentObject == null)
					return null;

				//check if the parent matches the type we're looking for
				var parent = parentObject as T;
				if (parent != null)
					return parent;
				child = parentObject;
			}
		}
	}
}