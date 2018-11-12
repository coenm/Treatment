namespace Treatment.Contract
{
    public enum ProgressState : int
    {
        /// <summary>
        /// Still is progress.
        /// </summary>
        Busy = 1 >> 0,

        /// <summary>
        /// Progress has finished successfully.
        /// </summary>
        FinishedSuccessfully = 1 >> 1,

        /// <summary>
        /// Progress has finished with an error.
        /// </summary>
        FinishedWithError = 1 >> 2,

        /// <summary>
        /// Progress has finished (successfully or with an error).
        /// </summary>
        Finished = FinishedSuccessfully | FinishedWithError,
    }
}
