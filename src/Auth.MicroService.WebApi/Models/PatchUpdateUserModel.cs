namespace Auth.MicroService.WebApi.Models
{
    /// <summary>
    /// The patch update user model.
    /// </summary>
    public class PatchUpdateUserModel
    {
        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; init; }

        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; init; }
    }
}
