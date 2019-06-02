namespace Treatment.Plugin.TestAutomation.UI.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract;

    internal static class FieldsHelper
    {
        /// <summary>
        /// Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter.
        /// If not matching item can be found,
        /// a null parent is being returned.</returns>
        /// <remarks>Taken from <see href="https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type" />.</remarks>
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid.
            if (parent == null)
                return null;

            T foundChild = null;

            if (parent is ContentControl cc)
            {
                if (cc.HasContent)
                {
                    if (cc.Content is T cct && cct is FrameworkElement cctf && cctf.Name == childName)
                    {
                        return cct;
                    }

                    var result = FindChild<T>(cc.Content as DependencyObject, childName);
                    if (result != null)
                        return result;
                }
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If the child is not of the request child type child
                if (!(child is T childType))
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child.
                    if (foundChild != null)
                        break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Locates and returns the instance of a field in <paramref name="parent"/> named <paramref name="fieldName"/>.
        /// </summary>
        /// <typeparam name="T">Type of the field and instance to return.</typeparam>
        /// <param name="parent">UiElement that contains the field to look for. Cannot be <c>null</c>.</param>
        /// <param name="fieldName">Name of the field. Cannot be <c>null</c> or empty.</param>
        /// <returns>Found element. Never null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is null.</exception>
        /// <exception cref="CouldNotFindFieldException">Thrown when the field cannot be found or is not of the right type.</exception>
        public static T FindFieldInUiElementByName<T>([NotNull] UIElement parent, [NotNull] string fieldName)
            where T : UIElement
        {
            Guard.NotNull(parent, nameof(parent));
            Guard.NotNullOrWhiteSpace(fieldName, nameof(fieldName));

            var fields = parent
                .GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

            var fields1 = fields
                .Where(field =>
                    field.Name == fieldName
                    &&
                    field.FieldType == typeof(T))
                .ToList();

            if (!fields1.Any())
            {
                throw new CouldNotFindFieldException(fieldName);
            }

            if (fields1.Count > 1)
            {
                throw new CouldNotFindFieldException(fieldName);
            }

            return (T)fields1.Single().GetValue(parent);
        }
    }
}
