namespace Treatment.Plugin.TestAutomation.UI.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract;

    internal static class FieldsHelper
    {
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
