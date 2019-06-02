namespace Treatment.TestAutomation.Contract
{
    using System;

    [Serializable]
    public class CouldNotFindFieldException : Exception
    {
        public CouldNotFindFieldException(string fieldName)
        {
        }
    }
}
