namespace Treatment.TestAutomation.Contract.Interfaces.Events.MessageBox
{
    using System;
    using System.Windows.Forms;

    using JetBrains.Annotations;

    [PublicAPI]
    public class MessageBoxClosed : TestElementEventBase
    {
        /// <summary>
        /// Guid of the parent window. Not sure if we can extract this information and if we need this.
        /// </summary>
        [CanBeNull]
        public Guid ParentGuid { get; set; }

        /// <summary>
        /// Result of the messagebox. Not sure if we can extract this information.
        /// </summary>
        public DialogResult DialogResult { get; set; }
    }
}
