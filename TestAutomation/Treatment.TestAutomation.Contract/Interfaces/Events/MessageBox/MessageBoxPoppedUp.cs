namespace Treatment.TestAutomation.Contract.Interfaces.Events.MessageBox
{
    using System;
    using System.Windows.Forms;

    using JetBrains.Annotations;

    [PublicAPI]
    public class MessageBoxPoppedUp : TestElementEventBase
    {
        /// <summary>
        /// Guid of the parent window. Not sure if we can extract this information and if we need this.
        /// </summary>
        [CanBeNull]
        public Guid ParentGuid { get; set; }

        /// <summary>
        /// Title of the messagebox.
        /// </summary>
        [CanBeNull]
        public string Title { get; set; }

        /// <summary>
        /// Message inside the messagebox.
        /// </summary>
        [CanBeNull]
        public string Message { get; set; }

        /// <summary>
        /// Available buttons of the messagebox.
        /// </summary>
        public MessageBoxButtons[] Buttons { get; set; }

        /// <summary>
        /// The default button of the messagebox. Not sure if we need this.
        /// </summary>
        public MessageBoxButtons DefaultButton { get; set; }

        /// <summary>
        /// Icon of the messagebox.
        /// </summary>
        public MessageBoxIcon Icon { get; set; }
    }
}
