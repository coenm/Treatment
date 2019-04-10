namespace Treatment.TestAutomation.Contract.Interfaces.Framework.Extensions
{
    using global::Treatment.Helpers.Guards;
    using JetBrains.Annotations;

    public static class KeyboardExtensions
    {
        public static IKeyboard EnterText([NotNull] this IKeyboard keyboard, [NotNull] string text)
        {
            Guard.NotNull(keyboard, nameof(keyboard));
            Guard.NotNull(text, nameof(text));
            Guard.MustBeGreaterThan(text.Length, 0, nameof(text));

            foreach (var c in text)
            {
                keyboard.KeyPress(c);
            }

            return keyboard;
        }
    }
}
