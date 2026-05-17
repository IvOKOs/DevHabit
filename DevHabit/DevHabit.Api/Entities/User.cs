namespace DevHabit.Api.Entities;

public sealed class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAtUtc {  get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public string IdentityId { get; set; }// the id we get from the identity provider;
                                          // will represent the identity of the user within the identity provider system
                                          // and also allows to switch identity providers by assigning the correct identity id
}
